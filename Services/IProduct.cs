namespace Summary.Shopfa.Services
{
    using Microsoft.Extensions.Options;
    using Core.Data;
    using Core.Workflows;
    using Summary.Shopfa.Settings;
    using Summary.Shopfa.Workflows.Task.Product.Update;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IProductService
    {
        Task UpdateProductAsync(string apiAddress,
            string username,
            string password,
            string sku,
            int price,
            int quantity,
            SearchBy search_by
        );
    }

    public class ProductService : BaseService, IProductService
    {
        private readonly HttpRequestClient _client;
        private readonly ShopfaSettings _options;

        public ProductService(HttpRequestClient client,
            IOptions<ShopfaSettings> options) : base(options, client)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task UpdateProductAsync(string apiAddress,
            string username,
            string password,
            string sku,
            int price,
            int quantity,
            SearchBy search_by)
        {
            if (string.IsNullOrWhiteSpace(sku)) return;

            var token = await GenerateNewTokenAsync(apiAddress,
                username,
                password
            );

            sku = sku.Trim();

            string field = "warehouse_code";

            if (search_by == SearchBy.Extended)
            {
                field = sku.Substring(0, sku.IndexOf("="));
                sku = sku.Substring(field.Length + 1, sku.Length - (field.Length + 1));
            }

            apiAddress ??= _options.ApiAddress;

            int oldPrice = 0;
            int pageIndex = 1;
            bool doWhile = true;
            var products = new List<InventoryProductInfo>();

            do
            {
                var filter = new MultipartFormDataContent
                {
                    { new StringContent("1000"), "limit" },
                    { new StringContent(pageIndex++.ToString()), "page" }
                };

                var result = await _client.SendPostRequestAsync<ResponseInventoryInfo>(
                    $"{apiAddress}/api/shop/warehouse/products",
                    filter,
                    new[] { new KeyValuePair<string, string>("Private-Key", $"{token}") }
                );

                if (result.Error_Code.HasValue) throw new NotImplementedException(result.Error);

                products.AddRange(result.Items.Where(x =>
                    x.Status >= 0 &&
                    ((search_by == SearchBy.SKU && x.Warehouse_Code == sku) ||
                        (search_by == SearchBy.Extended && x.Extended.TryGetValue(field, out var value) && value?.ToString() == sku)))
                );

                doWhile = result.Items.Count > 0;
            }
            while (doWhile);

            if (products.Any() is false) return;

            foreach (var product in products)
            {
                if (product.Old_Price != price) oldPrice = price;
                else
                {
                    oldPrice = product.Old_Price;
                    price = product.Price;
                }
            }

            var simple_products = products.Where(x => x.Variant_Id == 0);
            var variant_products = products.Where(x => x.Variant_Id != 0);

            if (simple_products.Any())
            {
                var simple_model = new
                {
                    product_status = simple_products.ToDictionary(x => x.Product_Id, x => quantity > 0 ? "1" : "0"),
                    product_quantity = simple_products.ToDictionary(x => x.Product_Id, x => quantity),
                    product_price = simple_products.ToDictionary(x => x.Product_Id, x => price),
                    product_old_price = simple_products.ToDictionary(x => x.Product_Id, x => oldPrice)
                };

                var response = await _client.SendPostRequestAsync<object>(
                    $"{apiAddress}/api/shop/warehouse/do",
                    simple_model,
                    new KeyValuePair<string, string>("private-key", $"{token}"),
                    "application/json"
                );
            }

            if (variant_products.Any())
            {
                foreach (var id in variant_products)
                {
                    var product_model = new MultipartFormDataContent
                    {
                        { new StringContent(id.Product_Id.ToString()), "id" },
                        { new StringContent("id,type,title,price,old_price,weight,product_status,unit,upc,sku,date,special,status,variants,variant_options"), "fields" }
                    };

                    var product_variants = await _client.SendPostRequestAsync<RpeVariantProductInfo>(
                        $"{apiAddress}/api/shop/product/list",
                        product_model,
                        new KeyValuePair<string, string>("private-key", $"{token}")
                    );

                    var product_variant = product_variants.Items.First();

                    var titles = product_variant.Variants
                        .Select(x => x.Title?.Trim()
                            .Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(y => y.Trim())
                            .Where(y => !string.IsNullOrWhiteSpace(y))
                            .Distinct(StringComparer.Ordinal)
                            .ToArray() ?? Array.Empty<string>())
                        .Where(x => x.Length > 0)
                        .ToList();

                    var color = titles
                        .Select(x => x.ElementAtOrDefault(0))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.Ordinal)
                        .ToList();

                    var size = titles
                        .Select(x => x.ElementAtOrDefault(1))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.Ordinal)
                        .ToList();

                    var other = titles
                        .Select(x => x.ElementAtOrDefault(2))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct(StringComparer.Ordinal)
                        .ToList();

                    var variant_model = new
                    {
                        id = product_variant.Id,
                        variant_options = product_variant.Variant_Options.Select(x => x.Value),
                        variant_options_vals = new[] { color, size, other },
                        variants = product_variant.Variants.Select(x => new
                        {
                            id = x.Id,
                            option = x.Title.Split("/").Select(x => x.Trim()),
                            old_price = x.Id == id.Variant_Id ? oldPrice : x.Old_Price,
                            price = x.Id == id.Variant_Id ? price : x.Price,
                            quantity = x.Id == id.Variant_Id ? quantity : x.Quantity,
                            status = "on",
                            product_status = (x.Id == id.Variant_Id ? quantity : x.Quantity) > 0 ? "1" : "0"
                        }),
                    };

                    var update_info = await _client.SendPostRequestAsync<object>(
                        $"{apiAddress}/api/shop/product/save",
                        variant_model,
                        new KeyValuePair<string, string>("private-key", $"{token}"),
                        "application/json"
                    );
                }
            }
        }
    }
}
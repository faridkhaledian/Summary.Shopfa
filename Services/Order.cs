using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Summary.Shopfa.Settings;
using Newtonsoft.Json;
using Core.Workflows;

namespace Summary.Shopfa.Services
{
    public interface IOrderService
    {
        Task<Basket> GetDetailsByIdAsync(
            string id
        );
    }

    public class OrderService : BaseService, IOrderService
    {
        private readonly HttpRequestClient _client;
        private readonly ShopfaSettings _options;

        public OrderService(HttpRequestClient client,
            IOptions<ShopfaSettings> options) : base(options, client)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<Basket> GetDetailsByIdAsync(
            string id)
        {
            var api = _options.ApiAddress;
            var username = _options.Username;
            var password = _options.Password;

            var token = await GenerateNewTokenAsync(
                api,
                username,
                password
            );

            var url = $"{api}/api/shop/orders/details";

            var body = new
            {
                id = id
            };

            var order = await _client.SendPostRequestAsync<GetOrdersResponseInfo>(
                url,
                body,
                new KeyValuePair<string, string>("Private-Key", token),
                "application/json",
                true
            );

            var log = new
            {
                Body = body,
                Token = token
            };

            if (order.Error_Code.HasValue) throw new WorkflowException(
                order.Error,
                null,
                JsonConvert.SerializeObject(log),
                "کارشناس پشتیبانی؛ تیکت را به سطح بعدی ارجاع دهید."
            );

            return order.Baskets.First();
        }
    }
}
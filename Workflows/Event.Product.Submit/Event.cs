using Core.Workflows.Abstractions.Models;
using Core.Workflows.Activities;
using Core.Workflows.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Summary.Shopfa.Services;

namespace Summary.Shopfa.Workflows.Event.Product.Submit
{
    public class SubmitProductEventInShopfa : EventActivity
    {
        private readonly IStringLocalizer<SubmitProductEventInShopfa> T;
        private readonly IProductService _product;

        public SubmitProductEventInShopfa(
            IStringLocalizer<SubmitProductEventInShopfa> t,
            IProductService product)
        {
            _product = product;
            T = t;
        }

        public override string Name => nameof(SubmitProductEventInShopfa);

        public override LocalizedString DisplayText => T[Shopfa.Localize.SOfSubmitProduct];

        public override LocalizedString Category => T[Shopfa.Public.Category];

        public override IEnumerable<Outcome> GetPossibleOutcomes(
            WorkflowExecutionContext workflowContext,
            ActivityContext activityContext)
        {
            return Outcomes(T[Shopfa.Workflows.Done]);
        }

        public override async Task<ActivityExecutionResult> ResumeAsync(
            WorkflowExecutionContext workflowContext,
            ActivityContext activityContext)
        {
            var raw = workflowContext.GetInputOrDefault("Shopfa.Product.Ids");

            var product_ids = JsonConvert.DeserializeObject<List<string>>(raw);

            int p = 0;

            foreach (var product_id in product_ids)
            {
                var product = await _product.GetDetailsByIdAsync(product_id);

                workflowContext.Input[$"Shopfa.Product.{p}.Id"] = product.Id;
                workflowContext.Input[$"Shopfa.Product.{p}.Title"] = product.Title;
                workflowContext.Input[$"Shopfa.Product.{p}.Quantity"] = product.Quantity;
                workflowContext.Input[$"Shopfa.Product.{p}.Price"] = product.Price;
                workflowContext.Input[$"Shopfa.Product.{p}.OldPrice"] = product.Old_Price;
                workflowContext.Input[$"Shopfa.Product.{p}.Weight"] = product.Weight;
                workflowContext.Input[$"Shopfa.Product.{p}.Status"] = product.Product_Status;
                workflowContext.Input[$"Shopfa.Product.{p}.WarehouseCode"] = product.Warehouse_Code;
                workflowContext.Input[$"Shopfa.Product.{p}.Unit"] = product.Unit;
                workflowContext.Input[$"Shopfa.Product.{p}.IsVariant"] = product.Variant;

                if (product.Variant)
                {
                    int v = 0;

                    foreach (var variant in product.Variants)
                    {
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v}.Id"] = variant.Id;
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v}.Title"] = variant.Title;
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v}.Price"] = variant.Price;
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v}.OldPrice"] = variant.Old_Price;
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v}.Quantity"] = variant.Quantity;
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v}.Status"] = variant.Status;
                        workflowContext.Input[$"Shopfa.Product.{p}.Variant.{v++}.Weight"] = variant.Weight;
                    }
                }

                p++;
            }

            return Outcomes(Shopfa.Workflows.Done);
        }
    }
}
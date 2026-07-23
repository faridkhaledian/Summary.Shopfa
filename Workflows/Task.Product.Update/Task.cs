namespace Summary.Shopfa.Workflows.Task.Product.Update
{
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Core.Mvc.Utilities;
    using Core.Workflows.Abstractions.Models;
    using Core.Workflows.Activities;
    using Core.Workflows.Models;
    using Settings;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UpdateProductInShopfaTask : TaskActivity
    {
        private readonly IStringLocalizer<UpdateProductInShopfaTask> T;
        private readonly ILogger<UpdateProductInShopfaTask> _logger;
        private readonly IProductService _product;

        public UpdateProductInShopfaTask(IStringLocalizer<UpdateProductInShopfaTask> t,
            ILogger<UpdateProductInShopfaTask> logger,
            IProductService product)
        {
            T = t;
            _logger = logger;
            _product = product;
        }

        public override string Name => nameof(UpdateProductInShopfaTask);

        public override LocalizedString DisplayText => T[Shopfa.Localize.SubjectOfUpdateProduct];

        public override LocalizedString Category => T[Shopfa.Public.Category];

        public WorkflowExpression<string> ApiAddress
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> Username
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> Password
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<bool> SKU
        {
            get => GetProperty(() => new WorkflowExpression<bool>());
            set => SetProperty(value);
        }

        public WorkflowExpression<string> Price
        {
            get => GetProperty(() => new WorkflowExpression<string>());
            set => SetProperty(value);
        }

        public WorkflowExpression<bool> Quantity
        {
            get => GetProperty(() => new WorkflowExpression<bool>());
            set => SetProperty(value);
        }

        public SearchBy? Search_By
        {
            get => GetProperty(() => default(SearchBy));
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetPossibleOutcomes(WorkflowExecutionContext workflowContext,
            ActivityContext activityContext)
        {
            return Outcomes(T[Shopfa.Workflows.Done]);
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(WorkflowExecutionContext workflowContext,
            ActivityContext activityContext)
        {
            var apiAddress = ApiAddress.Expression;
            var username = Username.Expression;
            var password = Password.Expression;
            var sku = workflowContext.GetInputOrDefault(SKU.Expression);
            var price = workflowContext.GetInputOrDefault(Price.Expression).ConvertTo<int>().Value;
            var quantity = workflowContext.GetInputOrDefault(Quantity.Expression).ConvertTo<int>().Value;
            var search_by = Search_By ?? SearchBy.SKU;

            await _product.UpdateProductAsync(apiAddress,
                username,
                password,
                sku,
                price,
                quantity,
                search_by
            );

            return Outcomes(Shopfa.Workflows.Done);
        }
    }
}
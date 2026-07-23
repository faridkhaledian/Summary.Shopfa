namespace Summary.Shopfa.Workflows.Task.Product.Update
{
    using Core.Workflows.Display;
    using Core.Workflows.Models;

    public class UpdateProductInShopfaTaskDisplay : ActivityDisplayDriver<UpdateProductInShopfaTask,
        UpdateProductInShopfaTaskViewModel>
    {
        protected override void EditActivity(UpdateProductInShopfaTask activity,
            UpdateProductInShopfaTaskViewModel model)
        {
            model.ApiAddress = activity.ApiAddress.Expression;
            model.Username = activity.Username.Expression;
            model.Password = activity.Password.Expression;
            model.SKU = activity.SKU.Expression;
            model.Price = activity.Price.Expression;
            model.Quantity = activity.Quantity.Expression;
            model.SearchBy = activity.Search_By;
        }

        protected override void UpdateActivity(UpdateProductInShopfaTaskViewModel model,
            UpdateProductInShopfaTask activity)
        {
            activity.ApiAddress = new WorkflowExpression<string>(model.ApiAddress);
            activity.Username = new WorkflowExpression<string>(model.Username);
            activity.Password = new WorkflowExpression<string>(model.Password);
            activity.SKU = new WorkflowExpression<bool>(model.SKU);
            activity.Price = new WorkflowExpression<string>(model.Price);
            activity.Quantity = new WorkflowExpression<bool>(model.Quantity);
            activity.Search_By = model.SearchBy;
        }
    }
}
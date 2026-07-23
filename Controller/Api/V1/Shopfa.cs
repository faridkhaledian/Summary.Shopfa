namespace Summary.Shopfa.Controller.Api.V1
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Core.Workflows.Services;
    using Summary.Shopfa.Services;
    using Summary.Shopfa.Workflows.Event.Order.WebhookHandler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = "Api")]
    [EnableCors("Everywhere")]
    [Route("api/v1/shopfa/order")]
    public class ShopfaWebhook : ControllerBase
    {
        private readonly IWorkflowManager _workflowManager;

        public ShopfaWebhook(
            IWorkflowManager workflowManager)
        {
            _workflowManager = workflowManager;
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Submit(string id)
        {
            await RaiseSubmitOrder(id);
            return Ok();
        }

        [HttpPost, Route("[action]")]
        public async Task<IActionResult> Submit([FromBody] OrderSubmitModel model)
        {
            if (model.Data.Object_Type == "Order") await RaiseSubmitOrder(model.Data.Items.First());

            return Ok();
        }

        private async Task RaiseSubmitOrder(string order_id)
        {
            var inputs = new Dictionary<string, object>
            {
                { "Shopfa.Order.Id", order_id }
            };

            await _workflowManager.TriggerIntoDBAsync(
                nameof(SubmitOrderEventInShopfa),
                inputs
            );
        }
    }
}
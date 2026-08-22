using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Core.Workflows.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Summary.Shopfa.Workflows.Event.Order.Create;
using Summary.Shopfa.Workflows.Event.Order.Update;
using Summary.Shopfa.Workflows.Event.Order.ChangeStatus;
using Summary.Shopfa.Workflows.Event.Product.Submit;

namespace Summary.Shopfa.Controller.Api.V1
{
    [ApiController]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = "Api")]
    [Route("api/v1/shopfa/submit")]
    public class SubmitController : ControllerBase
    {
        private readonly IWorkflowManager _workflowManager;

        public SubmitController(
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
        public async Task<IActionResult> Submit([FromBody] ShopfaSubmitModel model)
        {
            switch (model.Data.Object_Type.ToLower())
            {
                case "order":
                    await OrderSubmit(model);
                    break;

                case "product":
                    await ProductSubmit(model);
                    break;
            }

            return Ok();
        }

        private async Task OrderSubmit(ShopfaSubmitModel model)
        {
            string eventName = "";
            switch (model.Event_Type.ToLower())
            {
                case "order:create":
                    eventName = nameof(CreateOrderEventInShopfa);
                    break;
                case "order:update":
                    eventName = nameof(UpdateOrderEventInShopfa);
                    break;
                case "order:status_changed":
                    eventName = nameof(ChangeStatusOrderEventInShopfa);
                    break;
            }

            var inputs = new Dictionary<string, object>
            {
                { "Shopfa.Order.Id", model.Data.Items.First() }
            };

            await _workflowManager.TriggerIntoDBAsync(eventName, inputs);
        }

        private async Task ProductSubmit(ShopfaSubmitModel model)
        {
            var inputs = new Dictionary<string, object>
            {
                { "Shopfa.Product.Ids", model.Data.Items }
            };

            await _workflowManager.TriggerIntoDBAsync(
                nameof(SubmitProductEventInShopfa),
                inputs
            );
        }
    }
}
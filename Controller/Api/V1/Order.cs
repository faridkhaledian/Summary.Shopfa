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

namespace Summary.Shopfa.Controller.Api.V1
{
    [ApiController]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = "Api")]
    [Route("api/v1/shopfa/order")]
    public class OrderController : ControllerBase
    {
        private readonly IWorkflowManager _workflowManager;

        public OrderController(
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
            string eventName = "";
            switch (model.Event_Type.ToLower())
            {
                case "order:create":
                    eventName = nameof(CreateOrderEventInShopfaTask);
                    break;
                case "order:update":
                    eventName = nameof(UpdateOrderEventInShopfaTask);
                    break;
                case "order:status_changed":
                    eventName = nameof(ChangeStatusOrderEventInShopfaTask);
                    break;
            }

            var inputs = new Dictionary<string, object>
            {
                { "Shopfa.Order.Id", model.Data.Items.First() }
            };

            await _workflowManager.TriggerIntoDBAsync(
                eventName,
                inputs
            );

            return Ok();
        }
    }
}
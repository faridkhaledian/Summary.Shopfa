using Core.Workflows.Abstractions.Models;
using Core.Workflows.Activities;
using Core.Workflows.Models;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using Summary.Shopfa.Services;

namespace Summary.Shopfa.Workflows.Event.Order.ChangeStatus
{
    public class ChangeStatusOrderEventInShopfa : EventActivity
    {
        private readonly IStringLocalizer<ChangeStatusOrderEventInShopfa> T;
        private readonly IOrderService _order;

        public ChangeStatusOrderEventInShopfa(
            IStringLocalizer<ChangeStatusOrderEventInShopfa> t,
            IOrderService order)
        {
            _order = order;
            T = t;
        }

        public override string Name => nameof(ChangeStatusOrderEventInShopfa);

        public override LocalizedString DisplayText => T[Shopfa.Localize.SOfChangeStatusOrder];

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
            var order_id = workflowContext.GetInputOrDefault("Shopfa.Order.Id");

            var order = await _order.GetDetailsByIdAsync(order_id);

            workflowContext.Input["Shopfa.Order.Id"] = order.Id;
            workflowContext.Input["Shopfa.Order.Status"] = order.Status;
            workflowContext.Input["Shopfa.Order.Status.Title"] = order.Status_Title;
            workflowContext.Input["Shopfa.Order.Session"] = order.Session;
            workflowContext.Input["Shopfa.Order.User.Id"] = order.User_Id;
            workflowContext.Input["Shopfa.Order.State.Id"] = order.State_Id;
            workflowContext.Input["Shopfa.Order.City.Id"] = order.City_Id;
            workflowContext.Input["Shopfa.Order.Date"] = order.Date;
            workflowContext.Input["Shopfa.Order.Update"] = order.Update;
            workflowContext.Input["Shopfa.Order.Payment.Date"] = order.Payment_Date;
            workflowContext.Input["Shopfa.Order.Payment.Method"] = order.Payment_Method;
            workflowContext.Input["Shopfa.Order.Payment.Method.Title"] = order.Payment_Method_Title;
            workflowContext.Input["Shopfa.Order.Post.Method"] = order.Post_Method;
            workflowContext.Input["Shopfa.Order.Post.Method.Title"] = order.Post_Method_Title;
            workflowContext.Input["Shopfa.Order.Post.Price"] = order.Post_Price;
            workflowContext.Input["Shopfa.Order.Address"] = order.Address;
            workflowContext.Input["Shopfa.Order.Address.Id"] = order.Address_Id;
            workflowContext.Input["Shopfa.Order.Name"] = order.Name;
            workflowContext.Input["Shopfa.Order.Family"] = order.Family;
            workflowContext.Input["Shopfa.Order.PostalCode"] = order.Postalcode;
            workflowContext.Input["Shopfa.Order.Email"] = order.Email;
            workflowContext.Input["Shopfa.Order.MelliCode"] = order.Mellicode;
            workflowContext.Input["Shopfa.Order.Mobile"] = order.Mobile;
            workflowContext.Input["Shopfa.Order.Tel"] = order.Tel;
            workflowContext.Input["Shopfa.Order.Message"] = order.Message;
            workflowContext.Input["Shopfa.Order.Weight"] = order.Weight;
            workflowContext.Input["Shopfa.Order.Service.Price"] = order.Service_Price;
            workflowContext.Input["Shopfa.Order.Tax.Price"] = order.Tax_Price;
            workflowContext.Input["Shopfa.Order.Item.Price"] = order.Item_Price;
            workflowContext.Input["Shopfa.Order.Sum.Price"] = order.Sum_Price;
            workflowContext.Input["Shopfa.Order.Delivery.DateTime"] = order.Delivery_Time;

            int i = 0;

            foreach (var item in order.Items)
            {
                workflowContext.Input[$"Shopfa.Order.Item.{i}.Code"] = item.Product_Id;
                workflowContext.Input[$"Shopfa.Order.Item.{i}.Count"] = item.Count;
                workflowContext.Input[$"Shopfa.Order.Item.{i}.BaseUnitPrice"] = item.Price;
                workflowContext.Input[$"Shopfa.Order.Item.{i}.TotalUnitPrice"] = item.Price * item.Count;
                workflowContext.Input[$"Shopfa.Order.Item.{i}.TotalDiscount"] = (item.Old_Price - item.Price) * item.Count;
                workflowContext.Input[$"Shopfa.Order.Item.{i}.TotalVat"] = 0;
                workflowContext.Input[$"Shopfa.Order.Item.{i}.TotalToll"] = 0;
                workflowContext.Input[$"Shopfa.Order.Item.{i++}.FinalUnitPrice"] = item.Sum_Price;
            }

            return Outcomes(Shopfa.Workflows.Done);
        }
    }
}
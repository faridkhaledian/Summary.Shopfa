namespace Summary.Shopfa
{
    using Core.DisplayManagement.Handlers;
    using Core.Workflows.Helpers;
    using Core.Modules;
    using Core.Navigation;
    using Core.Security.Permissions;
    using Core.Settings;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Services;
    using Summary.Shopfa.Settings;
    using Summary.Shopfa.Workflows.Event.Order.WebhookHandler;
    using Summary.Shopfa.Workflows.Task.Product.Update;

    [Feature(Shopfa.Features.Shopfa)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<INavigationProvider, Menu>();
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IDisplayDriver<ISite>, ShopfaSettingsDisplayDriver>();

            services.AddActivity<SubmitOrderEventInShopfa, OrderWebhookHandlerEventDisplay>();
            services.AddActivity<UpdateProductInShopfaTask, UpdateProductInShopfaTaskDisplay>();

            services.AddTransient<IConfigureOptions<ShopfaSettings>, ShopfaSettingsConfiguration>();
        }
    }
}
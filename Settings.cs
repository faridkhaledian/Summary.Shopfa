namespace Summary.Shopfa.Settings
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Core.DisplayManagement.Entities;
    using Core.DisplayManagement.Handlers;
    using Core.DisplayManagement.Views;
    using Core.Entities;
    using Core.Environment.Shell;
    using Core.Settings;

    public class ShopfaSettings
    {
        public string ApiAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class ShopfaSettingsDisplayDriver : SectionDisplayDriver<ISite, 
        ShopfaSettings>
    {
        private readonly IShellHost _host;
        private readonly ShellSettings _shell;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IAuthorizationService _authorize;

        public ShopfaSettingsDisplayDriver(IShellHost host,
            ShellSettings settings,
            IHttpContextAccessor httpContext,
            IAuthorizationService authorize)
        {
            _host = host;
            _shell = settings;
            _httpAccessor = httpContext;
            _authorize = authorize;
        }

        public override async Task<IDisplayResult> EditAsync(ShopfaSettings settings,
            BuildEditorContext context)
        {
            var user = _httpAccessor.HttpContext?.User;
            if (user is null || !await _authorize.AuthorizeAsync(user, Permissions.ManageShopfaSettings))
            {
                return null;
            }

            var init = Initialize<ShopfaSettings>("ShopfaSettings_Edit", model =>
            {
                model.ApiAddress = settings.ApiAddress;
                model.Username = settings.Username;
                model.Password = settings.Password;
            });

            return init.Location("Content:5").OnGroup("Shopfa");
        }

        public override async Task<IDisplayResult> UpdateAsync(ShopfaSettings settings,
            BuildEditorContext context)
        {
            var user = _httpAccessor.HttpContext?.User;
            if (user is null || !await _authorize.AuthorizeAsync(user, Permissions.ManageShopfaSettings))
            {
                return null;
            }
            if (context.GroupId == "Shopfa")
            {
                await context.Updater.TryUpdateModelAsync(settings, Prefix);
                await _host.ReloadShellContextAsync(_shell);
            }
            return await EditAsync(settings, context);
        }
    }

    public class ShopfaSettingsConfiguration : IConfigureOptions<ShopfaSettings>
    {
        private readonly ISiteService _site;
        private readonly ILogger<ShopfaSettingsConfiguration> _logger;

        public ShopfaSettingsConfiguration(ISiteService site,
            ILogger<ShopfaSettingsConfiguration> logger)
        {
            _site = site;
            _logger = logger;
        }

        public void Configure(ShopfaSettings options)
        {
            var settings = _site.GetSiteSettingsAsync().GetAwaiter().GetResult().As<ShopfaSettings>();
            options.ApiAddress = settings.ApiAddress;
            options.Username = settings.Username;
            options.Password = settings.Password;
        }
    }
}
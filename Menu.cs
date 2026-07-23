namespace Summary.Shopfa
{
    using Microsoft.Extensions.Localization;
    using Core.Navigation;
    using System;
    using System.Threading.Tasks;

    public class Menu : INavigationProvider
    {
        private readonly IStringLocalizer<Menu> T;
        public Menu(IStringLocalizer<Menu> localizer)
        {
            T = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase)) 
                return Task.CompletedTask;

            builder.Add(T["Configuration"], configuration =>
            {
                configuration.Add(T["Settings"], settings =>
                {
                    settings.Add(T["شاپفا"], T["شاپفا"], itemBuilder =>
                    {
                        itemBuilder.Action("Index", "Admin", new { area = "Core.Settings", groupId = "Shopfa" })
                            .Permission(Permissions.ManageShopfaSettings)
                            .LocalNav();
                    });
                });
            });

            return Task.CompletedTask;
        }
        
    }
}
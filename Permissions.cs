namespace Summary.Shopfa
{
    using Core.Security.Permissions;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class Permissions : IPermissionProvider
    {
        internal static Permission ManageShopfaSettings =
            new Permission(nameof(ManageShopfaSettings), "Manage Shopfa Settings");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[] { ManageShopfaSettings }.AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new []{ ManageShopfaSettings }
                }
            };
        }
        
    }
}
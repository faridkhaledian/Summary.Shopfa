using Core.Modules.Manifest;
using Summary.Shopfa;

[assembly: Feature(
    Id = Shopfa.Features.Shopfa,
    Name = Shopfa.Localize.SubjectOfShopfa,
    Description =Shopfa.Localize.DescriptionOfShopfa,
    Category = Shopfa.Public.Category,
    Dependencies = new[] { Shopfa.Features.Workflows },
    Version = "1.2.0"
)]

namespace Summary.Shopfa.Workflows.Task.Product.Update
{
    using System.ComponentModel.DataAnnotations;

    public class UpdateProductInShopfaTaskViewModel
    {
        public string ApiAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string SKU { get; set; }
        public SearchBy? SearchBy { get; set; }
        public string Price { get; set; } = null;
        public string Quantity { get; set; } = null;
    }

    public enum SearchBy
    {
        SKU,
        Extended
    }
}
using Pronia.Models;

namespace Pronia.ViewModels.ShopViewModels
{
    public class ShopVM
    {
        
        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<Product>? Products { get; set; }
    }
}

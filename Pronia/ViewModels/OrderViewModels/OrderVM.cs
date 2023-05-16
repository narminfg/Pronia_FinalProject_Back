using Pronia.Models;
using Pronia.ViewModels.BasketViewModels;

namespace Pronia.ViewModels.OrderViewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public IEnumerable<BasketVM> BasketVMs { get; set; }
    }
}

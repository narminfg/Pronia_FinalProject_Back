using Pronia.ViewModels.BasketViewModels;
using Pronia.ViewModels.WishListViewModels;

namespace Pronia.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettings();
        Task<IEnumerable<BasketVM>> GetBaskets();
        Task<IEnumerable<WishListVM>> GetWishList();
    }
}

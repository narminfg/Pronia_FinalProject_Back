using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DataAccessLayer;
using Pronia.Interfaces;
using Pronia.Models;
using Pronia.ViewModels.BasketViewModels;
using Pronia.ViewModels.WishListViewModels;

namespace Pronia.Services
{
    public class LayoutService: ILayoutService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LayoutService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BasketVM>> GetBaskets()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;
            if (!string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);
                    if (product != null)
                    {

                        basketVM.Price = product.Price;
                        basketVM.Title = product.Title;
                        basketVM.Image = product.MainImage;
                    }
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }
            return basketVMs;
        }

        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _appDbContext.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            return settings;
        }
        public async Task<IEnumerable<WishListVM>> GetWishList()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["wishList"];

            List<WishListVM> wishListVMs = null;

            if (!string.IsNullOrWhiteSpace(basket))
            {
                wishListVMs = JsonConvert.DeserializeObject<List<WishListVM>>(basket);

                foreach (WishListVM wishListVM in wishListVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p => p.Id == wishListVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        wishListVM.Price = product.Price;
                        wishListVM.Title = product.Title;
                        wishListVM.Image = product.MainImage;
                        wishListVM.Count = product.Count;
                    }
                }
            }
            else
            {
                wishListVMs = new List<WishListVM>();
            }

            return wishListVMs;
        }
    }
}

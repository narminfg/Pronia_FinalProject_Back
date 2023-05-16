using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels.WishListViewModels;

namespace Pronia.Controllers
{
    public class WishListController : Controller
    {
        private readonly AppDbContext _context;
        public WishListController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddWishList(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id && p.IsDeleted == false)) return BadRequest();

            string wishList = HttpContext.Request.Cookies["wishList"];

            List<WishListVM> wishListVMs = null;

            if (string.IsNullOrWhiteSpace(wishList))
            {
                wishListVMs = new List<WishListVM>
                {
                        new WishListVM{ Id = (int)id }
                };
            }
            else
            {
                wishListVMs = JsonConvert.DeserializeObject<List<WishListVM>>(wishList);
                if (!wishListVMs.Exists(b => b.Id == id))
                {
                    wishListVMs.Add(new WishListVM { Id = (int)id });
                }
            }
            wishList = JsonConvert.SerializeObject(wishListVMs);
            HttpContext.Response.Cookies.Append("wishList", wishList);

            foreach (WishListVM basketVM in wishListVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.Title = product.Title;
                    basketVM.Price = product.Price;
                    basketVM.Image = product.MainImage;

                }
            }
            return PartialView("_WishListPartial", wishListVMs);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteWishList(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) { return NotFound(); }

            string wishList = HttpContext.Request.Cookies["wishList"];

            List<WishListVM> wishListVMs = JsonConvert.DeserializeObject<List<WishListVM>>(wishList);

            foreach (WishListVM wishListVM in wishListVMs)
            {
                if (wishListVM.Id == id)
                {
                    wishListVMs.Remove(wishListVM);
                    break;
                }
            }
            foreach (WishListVM wishListVM in wishListVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == wishListVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    wishListVM.Title = product.Title;
                    wishListVM.Price = product.Price;
                    wishListVM.Image = product.MainImage;

                }
            }
            wishList = JsonConvert.SerializeObject(wishListVMs);

            HttpContext.Response.Cookies.Append("wishList", wishList);

            return PartialView("_WishListPartial", wishListVMs);
        }

    }
}

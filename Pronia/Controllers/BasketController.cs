using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels.BasketViewModels;

namespace Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager = null)
        {
            _context = context;
            _userManager = userManager;
        }

       
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) { return NotFound(); }

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = new List<BasketVM> {

                    new BasketVM {Id=(int)id,Count=1}
                };

            }
            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                if (basketVMs.Exists(b => b.Id == id))
                {
                    basketVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    basketVMs.Add(new BasketVM { Id = (int)id, Count = 1 });
                }
            }
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (appUser.Baskets.Any(b => b.ProductId == id))
                {
                    appUser.Baskets.FirstOrDefault(b => b.ProductId == id).Count = basketVMs.FirstOrDefault(b => b.Id == id).Count;
                }
                else
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

                    Basket dbBasket = new Basket
                    {
                        ProductId = (int)id,
                        Count = basketVMs.FirstOrDefault(b => b.Id == id).Count,
                        Image = product.MainImage,
                        Price = product.Price,
                        Title = product.Title
                    };

                    appUser.Baskets.Add(dbBasket);
                }
                await _context.SaveChangesAsync();

            };

            basket = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", basket);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);
                if (product != null)
                {

                    basketVM.Price = product.Price;
                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                }
            }

            return PartialView("_BasketPartial", basketVMs);
        }
        public async Task<IActionResult> GetBasket()
        {

            return Json(JsonConvert.DeserializeObject<List<BasketVM>>(HttpContext.Request.Cookies["basket"]));
        }

        [HttpGet]
        public IActionResult GetBasketCount()
        {
            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                return Json(0);
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            int count = basketVMs.Select(b => b.Id).Distinct().Count();

            return Json(count);
        }

        public async Task<IActionResult> DeleteBasket(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (string.IsNullOrWhiteSpace(basket)) { return BadRequest(); }

            else
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                if (basketVMs.Exists(b => b.Id == id))
                {
                    BasketVM basketVM = basketVMs.Find(b => b.Id == id);
                    basketVMs.Remove(basketVM);
                    basket = JsonConvert.SerializeObject(basketVMs);
                    HttpContext.Response.Cookies.Append("basket", basket);
                }
                else
                {
                    return NotFound();
                }
            }
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.Price = product.Price;
                    basketVM.Title = product.Title;
                    basketVM.Image = product.MainImage;
                }
            }

            return PartialView("_BasketPartial", basketVMs);
        }

        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            string basket = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            
            return View(basketVMs);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCart(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) { return NotFound(); }

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

            foreach (BasketVM basketVM in basketVMs)
            {
                if (basketVM.Id == id)
                {
                    basketVMs.Remove(basketVM);
                    break;
                }
            }
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    basketVM.Title = product.Title;
                    basketVM.Price = product.Price;
                    basketVM.Image = product.MainImage;

                }
            }
            basket = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", basket);

            return PartialView("_CartPartial", basketVMs);
        }


        public async Task<IActionResult> IncreaseCount(int? productId)
        {

            Product product=await _context.Products.FirstOrDefaultAsync(p=>p.Id== productId);



            if (productId == null)
            {

                return BadRequest();
            }

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);



            if (!basketVMs.Any(b => b.Id == productId))
            {
                return NotFound();
            }

            if(basketVMs.FirstOrDefault(b => b.Id == productId).Count < product.Count)
            {
                basketVMs.FirstOrDefault(b => b.Id == productId).Count += 1;
            }

            
            basket = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", basket);
          
            foreach (BasketVM basketVM in basketVMs)
            {
                Product product1 = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);
                if (product1 != null)
                {

                    basketVM.Price = product1.Price;
                    basketVM.Title = product1.Title;
                    basketVM.Image = product1.MainImage;
                }
               
            }

            return PartialView("_CartPartial", basketVMs);


        }


        public async Task<IActionResult> DecreaseCount(int? productId)
        {


            if (productId == null)
            {

                return BadRequest();
            }

            string basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);



            if (!basketVMs.Any(b => b.Id == productId))
            {
                return NotFound();
            }

            if (basketVMs.FirstOrDefault(b => b.Id == productId).Count > 1)
            {
                basketVMs.FirstOrDefault(b => b.Id == productId).Count -= 1;
            }

            
            basket = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", basket);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product1 = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);
                if (product1 != null)
                {

                    basketVM.Price = product1.Price;
                    basketVM.Title = product1.Title;
                    basketVM.Image = product1.MainImage;
                }
            }

            return PartialView("_CartPartial", basketVMs);





        }


    }
}

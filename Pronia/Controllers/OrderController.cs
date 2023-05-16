using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels.BasketViewModels;
using Pronia.ViewModels.OrderViewModels;

namespace Pronia.Controllers
{
    [Authorize(Roles ="Member")]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public OrderController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {

            string cookie = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(cookie))
            {
                return RedirectToAction("index", "shop");
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.Price;
                basketVM.Title = product.Title;
            }

            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false && a.IsMain))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            Address address = appUser.Addresses?.FirstOrDefault();

            if(address == null)
            {
                 return RedirectToAction("profile", "account");
            
            }

            Order order = new Order
            {
                Name = appUser.Name,
                SurnName = appUser.SurName,
                Email = appUser.Email,
                PhoneNumber = appUser.Addresses?.FirstOrDefault().PhoneNumber,
                Street = appUser.Addresses?.FirstOrDefault().Street,
                Country = appUser.Addresses?.FirstOrDefault().Country,
                City = appUser.Addresses?.FirstOrDefault().City,
                ZipCode = appUser.Addresses?.FirstOrDefault().ZipCode,
            };

            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs,
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders)
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false && a.IsMain))
                 .Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            string cookie = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(cookie))
            {
                return RedirectToAction("index", "shop");
            }

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.Price;
                basketVM.Title = product.Title;
            }
            OrderVM orderVM = new OrderVM
            {
                Order = order,
                BasketVMs = basketVMs,
            };

            if (!ModelState.IsValid)
            {
                return View(orderVM);
            }

            List<OrderItem> orderItems = new List<OrderItem>();

            foreach (BasketVM basketVM in basketVMs)
            {
                OrderItem orderItem = new OrderItem
                {
                    Count = basketVM.Count,
                    ProductId = basketVM.Id,
                    Price = basketVM.Price,
                    CreatedAt = DateTime.UtcNow.AddHours(4),
                    CreatedBy = $"{appUser.Name} {appUser.SurName}"
                };

                orderItems.Add(orderItem);
            }
            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;
            }

            HttpContext.Response.Cookies.Append("basket", "");
            order.IsDeleted = false;
            order.UserId = appUser.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.OrderItems = orderItems;
            order.No = appUser.Orders != null && appUser.Orders.Count() > 0 ? appUser.Orders.Last().No + 1 : 1;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("index", "home");
        }
    }
}

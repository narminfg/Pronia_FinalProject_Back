using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels.HomeViewModels;

namespace Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> IsFeaturedProducts = await _context.Products.Include(p=>p.Reviews.Where(r=>r.IsDeleted==false)).Where(p => p.IsDeleted == false && p.IsFeatured).ToListAsync();
            IEnumerable<Product> BestSellerProducts = await _context.Products.Include(p => p.Reviews.Where(r => r.IsDeleted == false)).Where(p => p.IsDeleted == false && p.IsBestSeller).ToListAsync();
            IEnumerable<Product> LatestProducts = await _context.Products.Include(p => p.Reviews.Where(r => r.IsDeleted == false)).Where(p => p.IsDeleted == false && p.IsLatest).ToListAsync();
            IsFeaturedProducts=IsFeaturedProducts.OrderByDescending(p => p.CreatedAt).Take(8);
            BestSellerProducts = BestSellerProducts.OrderByDescending(p => p.CreatedAt).Take(8);
            LatestProducts=LatestProducts.OrderByDescending(p=>p.CreatedAt).Take(8);

            HomeVM vm = new HomeVM
            {
                Featured=IsFeaturedProducts,
                BestSeller=BestSellerProducts,
                Latest=LatestProducts,
                Sliders = await _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync(),      
                New = await _context.Products.Where(p => p.IsDeleted == false && p.IsNew).OrderByDescending(p=>p.Id).ToListAsync(),
            };
            return View(vm);
        }
    }
}

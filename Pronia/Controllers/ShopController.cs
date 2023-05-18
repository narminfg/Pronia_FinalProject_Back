using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels;
using Pronia.ViewModels.ShopViewModels;

namespace Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
         public ShopController(AppDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> Products= await _context.Products.Include(p => p.Reviews.Where(r => r.IsDeleted == false)).Include(p=>p.ProductImages.Where(pi=>pi.IsDeleted==false)).Where(p=>p.IsDeleted==false).ToListAsync();
            IEnumerable<Category> Categories = await _context.Categories.Include(c=>c.Products.Where(p=>p.IsDeleted==false)).Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.PageIndex = 1;
            ViewBag.PageCount = (int)Math.Ceiling((decimal)Products.Count() / 3);
            Products = Products.Take(3).ToList();

            ShopVM shopVM= new ShopVM 
            { 
            Products= Products,
            Categories= Categories
            
            };
            
            

            return View(shopVM);
        }

        public async Task<IActionResult> getShopFilter(int? categoryId, int pageIndex=1, string range="")
        {

            double minValue = 5;
            double maxValue = 100;


            
            if (!string.IsNullOrWhiteSpace(range))
            {
                string[] arr = range.Split("-");
                minValue = double.Parse(arr[0]);
                maxValue = double.Parse(arr[1]);
            }




            IEnumerable<Product> Products = await _context.Products
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Where(p => p.IsDeleted == false).ToListAsync();


            IEnumerable<Category> Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            ViewBag.CategoryId = categoryId;
            ViewBag.PageIndex = pageIndex;
            ViewBag.Range = range;
            

            if (categoryId != null && !Categories.Any(c => c.Id == categoryId))
            {
                return BadRequest();
            }

           if(categoryId != null)
            {
                Products = Products.Where(p => p.CategoryId == categoryId).ToList();

            }

            
            Products=Products.Where(p=> p.Price>minValue && p.Price<maxValue).ToList();
            ViewBag.PageCount = (int)Math.Ceiling((decimal)Products.Count() / 3);

            Products=Products.Skip((pageIndex-1) * 3).Take(3).ToList();

            

            ShopVM shopVM = new ShopVM
            {
                Products = Products,
                Categories = Categories

            };


            return PartialView("_ShopProductListPartial", shopVM);


        }


        //public async Task<IActionResult> RangeFilter(string range = "", int pageIndex = 1)
        //{

        //    double minValue = 0;
        //    double maxValue = 0;


        //    range = range?.Replace("$", "");
        //    if (!string.IsNullOrWhiteSpace(range))
        //    {
        //        string[] arr = range.Split(" - ");
        //        minValue = double.Parse(arr[0]);
        //        maxValue = double.Parse(arr[1]);
        //    }
        //    IEnumerable<Product> product = await _context.Products
        //        .Where(p => p.IsDeleted == false).ToListAsync();

        //    return PartialView("_ShopProductListPartial", product);
        //}





    }
}

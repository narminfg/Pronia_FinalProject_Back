using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels.ProductViewModel;
using System.Data;

namespace Pronia.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Category)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = new Review { ProductId = id },
            };
            return View(productReviewVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddReview(Review review)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .Include(p => p.Category)
                .Include(p => p.Reviews.Where(r => r.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == review.ProductId);

            ProductReviewVM productReviewVM = new ProductReviewVM { Product = product, Review = review };

            if (!ModelState.IsValid) return View("Detail", productReviewVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            review.IsDeleted=false;

            if (product.Reviews != null && product.Reviews.Count() > 0 && product.Reviews.Any(r => r.UserId == appUser.Id))
            {
                ModelState.AddModelError("Name", "You have already commented");
                return View("Detail", productReviewVM);
            }

            review.UserId = appUser.Id;
            review.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            review.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Review.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = product.Id });
        }

        public async Task<IActionResult> ProductModal(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return PartialView("_ModalPartial", product);
        }
        public async Task<IActionResult> Search(string search)
        {

            IEnumerable<Product> products = await _context.Products.Where(p => p.IsDeleted == false && p.Title.ToLower().Contains(search.ToLower().Trim())).ToListAsync();

            return PartialView("_SearchPartial", products);

        }

    }
}

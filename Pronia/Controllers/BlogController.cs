using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels;
using System.Reflection.Metadata;

namespace Pronia.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Blog> blogs = _context.Blogs.Where(b => b.IsDeleted == false);
            IEnumerable<Blog> recentBlogs = _context.Blogs.Where(b => b.IsDeleted == false).OrderByDescending(b => b.CreatedAt).Take(4);

            ViewBag.Blogs = recentBlogs;

            return View(PageNatedList<Blog>.Create(blogs, pageIndex, 6));

            
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (blog == null) return NotFound();

            IEnumerable<Blog> recentBlogs = _context.Blogs.Where(b => b.IsDeleted == false).OrderBy(b => b.CreatedAt).Take(4);
            ViewBag.Blogs = recentBlogs;

            return View(blog);
        }
    }
}

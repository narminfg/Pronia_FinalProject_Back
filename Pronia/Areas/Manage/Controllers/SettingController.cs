using BackEndProject.Extentions;
using BackEndProject.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccessLayer;
using Pronia.Models;
using System.Data;

namespace Pronia.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Setting> settings = await _context.Settings.ToListAsync();

            return View(settings);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Setting setting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);

            if (setting == null) return NotFound();


            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Setting setting)
        {
            if (!ModelState.IsValid) return View(setting);

            if (id == null) return BadRequest();

            if (id != setting.Id) return BadRequest();

            Setting dbSetting = await _context.Settings.FirstOrDefaultAsync(s => s.Id == id);

            if (dbSetting == null) return NotFound();

            if (id == 2)
            {
                if (setting.File != null)
                {
                    if (!setting.File.CheckFileContentType("image/png"))
                    {
                        ModelState.AddModelError("File", "MainFile Yalniz PNG Olmalidir");
                        return View(setting);
                    }

                    if (!setting.File.CheckFileLength(300))
                    {
                        ModelState.AddModelError("File", "File Yalniz 300 kb Olmalidir");
                        return View(setting);
                    }
                    FileHelper.DeleteFile(dbSetting.Value, _env, "assets", "img", "logo");
                    dbSetting.Value = await setting.File.CreateFileAsync(_env, "assets", "img", "logo");
                }
            }
            else
            {
                dbSetting.Value = setting.Value;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

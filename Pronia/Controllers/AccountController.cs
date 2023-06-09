﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using Pronia.DataAccessLayer;
using Pronia.Models;
using Pronia.ViewModels.AccountViewModels;
using Pronia.ViewModels.BasketViewModels;
using System.Net.Mail;
using System.Transactions;
using static NuGet.Packaging.PackagingConstants;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult>Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid) return View(registerVM);
            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                SurName = registerVM.SurName,
                Email = registerVM.Email,
                UserName= registerVM.UserName,
            };
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }
            await _userManager.AddToRoleAsync(appUser, "Member");
            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is wrong");
                return View(loginVM);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager
                .PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account is blocked");
                return View(loginVM);
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is wrong");
                return View(loginVM);
            }
            string basket = HttpContext.Request.Cookies["basket"];
            if (string.IsNullOrWhiteSpace(basket))
            {
                if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {
                    List<BasketVM> basketVMs = new List<BasketVM>();

                    foreach (Basket basket1 in appUser.Baskets)
                    {
                        Product product = new Product();

                        BasketVM basketVM = new BasketVM
                        {
                            Id = (int)basket1.ProductId,
                            Count = basket1.Count,
                            Image = basket1.Image,
                            Title = basket1.Title,
                            Price = (double)basket1.Price
                        };
                        basketVMs.Add(basketVM);
                    }
                    basket = JsonConvert.SerializeObject(basketVMs);

                    HttpContext.Response.Cookies.Append("basket", basket);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket", "");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Append("basket", "");

            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(ua => ua.IsDeleted == false))
                .Include(u=>u.Orders.Where(o=>o.IsDeleted==false))
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            ProfileVM profileVM = new ProfileVM
            {
                
                Addresses = appUser.Addresses,
                Orders = appUser.Orders,
                Name=appUser.Name,
                SurName=appUser.SurName,
                Email=appUser.Email,
                UserName=appUser.UserName,
            };
            return View(profileVM);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileVM profileVM)
        {
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.Name = profileVM.Name;
            appUser.SurName = profileVM.SurName;
            appUser.Addresses= profileVM.Addresses;

            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant())
            {
                appUser.UserName = profileVM.UserName;
            }
            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant())
            {
                appUser.Email = profileVM.Email;
            }
            IdentityResult identityResult = await _userManager.UpdateAsync(appUser);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(profileVM);
            }
            if (!string.IsNullOrWhiteSpace(profileVM.OldPassword))
            {
                if (!await _userManager.CheckPasswordAsync(appUser, profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Password is wrong !");
                    return View(profileVM);
                }
                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                identityResult = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);

                if (!identityResult.Succeeded)
                {
                    foreach (IdentityError identityError in identityResult.Errors)
                    {
                        ModelState.AddModelError("", identityError.Description);
                    }
                    return View(profileVM);
                }
            }


            await _signInManager.SignInAsync(appUser, true);
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users
               .Include(u => u.Addresses.Where(ua => ua.IsDeleted == false))
               .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());


            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses,
            };
            if (!ModelState.IsValid)
            {
                return View(nameof(Profile), profileVM);
            }
            if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(a => a.IsMain))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain).IsMain = false;
            }
            address.IsDeleted = false;
            address.UserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            address.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            TempData["Tab"]= "address";

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddress(int? id)
        {
            if (id == null) { return NotFound(); }
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                            .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            Address oldAddress = appUser.Addresses.FirstOrDefault(a => a.IsDeleted == false && a.Id == id);
            if (oldAddress == null) { return NotFound(); }
            Address address = new Address
            {
                City = oldAddress.City,
                State = oldAddress.State,
                ZipCode = oldAddress.ZipCode,
                Country = oldAddress.Country,
                IsMain = oldAddress.IsMain,
                Id = oldAddress.Id
            };
            TempData["Tab"] = "address";
            return RedirectToAction(nameof(Profile), address);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditAddress(Address address)
        {
            if (address == null || address.Id == null) { return NotFound(); }
            AppUser user = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                           .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            if (user == null)
            {
                return NotFound();
            }

            Address oldAddress = user.Addresses.FirstOrDefault(a => a.IsDeleted == false && a.Id == address.Id);
            if (oldAddress == null) { return NotFound(); }
            bool isMain = address.IsMain;
            if (isMain)
            {
                IEnumerable<Address> differentAddresses = user.Addresses.Where(a => a.Id != address.Id && a.IsDeleted ==false && a.IsMain);
                foreach (Address differentAddress in differentAddresses)
                {
                    differentAddress.IsMain = false;
                }
            }

            oldAddress.City = address.City;
            oldAddress.State = address.State;
            oldAddress.ZipCode = address.ZipCode;
            oldAddress.Country = address.Country;
            oldAddress.IsMain = address.IsMain;
            

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            TempData["Tab"] = "address";


            return RedirectToAction(nameof(Profile));


        }

        #region MyRegion
        //public async Task<IActionResult> CreateUser()
        //{
        //    AppUser appUser= new AppUser();
        //    appUser.Name = "Super";
        //    appUser.SurName = "Admin";
        //    appUser.Email = "SuperAdminMain@gmail.com";
        //    appUser.UserName = "SuperAdminMain";
        //    await _userManager.CreateAsync(appUser, "Superadmin133");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");

        //    return Ok();

        //}


        //[HttpGet]
        //public async Task<IActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Admin"));
        //    await _roleManager.CreateAsync(new IdentityRole("Member"));
        //    return Content("ugurlu oldu");
        //}

        #endregion


        #region MyRegion
        //[HttpGet]
        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Name = "Super",
        //        SurName = "Admin",
        //        UserName = "SuperAdmin",
        //        Email = "superadmin@gmail.com",

        //    };
        //    await _userManager.CreateAsync(appUser,"SuperAdmin123");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");
        //    return Content("Ugurlu oldu");
        //}


        //[HttpGet]
        //public async Task<IActionResult> CreateUser()
        //{
        //    AppUser appUser = new AppUser
        //    {
        //        Name = "Super",
        //        SurName = "Admin",
        //        UserName = "SuperAdmin",
        //        Email = "superadmin@gmail.com"
        //    };

        //    await _userManager.CreateAsync(appUser, "SuperAdmin133");
        //    await _userManager.AddToRoleAsync(appUser, "SuperAdmin");


        //    return Content("Ugurlu Oldu");
        //}

        #endregion


    }
}

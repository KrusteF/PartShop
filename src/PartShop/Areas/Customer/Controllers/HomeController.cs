namespace PartShop.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using PartShop.Data;
    using PartShop.Models;
    using PartShop.Models.ViewModels;
    using PartShop.Utility;

    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(p => p.Category).Include(p => p.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(p => p.IsActive == true).ToListAsync()
            };


            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _db.ShoppingCart.Where(p => p.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);
            }
            return View(indexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuByID = await _db.MenuItem.Include(p => p.Category).Include(p => p.SubCategory).Where(p => p.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new()
            {
                MenuItem = menuByID,
                MenuItemId = menuByID.Id
            };

            return View(cartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCartModel)
        {
            shoppingCartModel.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCartModel.ApplicationUserId = claim.Value;

                var cartFromDb = await _db.ShoppingCart.Where(p => p.ApplicationUserId == shoppingCartModel.ApplicationUserId
                                                        && p.MenuItemId == shoppingCartModel.MenuItemId).FirstOrDefaultAsync();
                if (cartFromDb == null)
                {
                    ShoppingCart newModel = new()
                    {
                        ApplicationUserId = shoppingCartModel.ApplicationUserId,
                        Count = shoppingCartModel.Count,
                        MenuItemId = shoppingCartModel.MenuItemId
                    };
                    await _db.ShoppingCart.AddAsync(newModel);
                }
                else
                {
                    cartFromDb.Count += shoppingCartModel.Count;
                }
                await _db.SaveChangesAsync();

                var count = _db.ShoppingCart.Where(p => p.ApplicationUserId == shoppingCartModel.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuObj = await _db.MenuItem.Include(p => p.Category).Include(p => p.SubCategory).Where(p => p.Id == shoppingCartModel.MenuItemId).FirstOrDefaultAsync();
                ShoppingCart cart = new()
                {
                    MenuItem = menuObj,
                    MenuItemId = menuObj.Id
                };
                return View(cart);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

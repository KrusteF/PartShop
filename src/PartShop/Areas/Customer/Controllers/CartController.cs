namespace PartShop.Areas.Customer.Controllers
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using PartShop.Data;
    using PartShop.Models;
    using PartShop.Models.ViewModels;
    using PartShop.Utility;

    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public OrderDetailsCartViewModel DetailCartVM { get; set; }

        public async Task<IActionResult> Index()
        {
            DetailCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };
            DetailCartVM.OrderHeader.OrderTotal = 0;

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = await _db.ShoppingCart.Where(p => p.ApplicationUserId == claim.Value).ToListAsync();

            if (cart != null)
            {
                DetailCartVM.ListCart = cart;
            }
            foreach (var list in DetailCartVM.ListCart)
            {
                list.MenuItem = _db.MenuItem.Where(p => p.Id == list.MenuItemId).FirstOrDefault();
                DetailCartVM.OrderHeader.OrderTotal += list.MenuItem.Price * list.Count;
                list.MenuItem.Description = SD.ConvertToRawHtml(list.MenuItem.Description);
                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }
            DetailCartVM.OrderHeader.OrderTotalOrignal = DetailCartVM.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(p => p.Name.ToLower() == DetailCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                DetailCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb, DetailCartVM.OrderHeader.OrderTotalOrignal);
            }

            return View(DetailCartVM);
        }
        public async Task<IActionResult> Summary()
        {
            DetailCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            DetailCartVM.OrderHeader.OrderTotal = 0;

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ApplicationUser applicationUser = await _db.ApplicationUser.Where(p => p.Id == claim.Value).FirstOrDefaultAsync();

            var cart = _db.ShoppingCart.Where(p => p.ApplicationUserId == claim.Value).ToList();

            if (cart != null)
            {
                DetailCartVM.ListCart = cart;
            }
            foreach (var list in DetailCartVM.ListCart)
            {
                list.MenuItem = _db.MenuItem.Where(p => p.Id == list.MenuItemId).FirstOrDefault();
                DetailCartVM.OrderHeader.OrderTotal += list.MenuItem.Price * list.Count;
            }
            DetailCartVM.OrderHeader.OrderTotalOrignal = DetailCartVM.OrderHeader.OrderTotal;

            DetailCartVM.OrderHeader.PickupName = applicationUser.Name;
            DetailCartVM.OrderHeader.PickupNumber = applicationUser.PhoneNumber;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _db.Coupon.Where(p => p.Name.ToLower() == DetailCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                DetailCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb, DetailCartVM.OrderHeader.OrderTotalOrignal);
            }

            return View(DetailCartVM);
        }

        public IActionResult AddCoupon()
        {
            if (DetailCartVM.OrderHeader.CouponCode == null)
            {
                DetailCartVM.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, DetailCartVM.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cartById = await _db.ShoppingCart.Where(p => p.Id == cartId).FirstOrDefaultAsync();
            cartById.Count += 1;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartById = await _db.ShoppingCart.Where(p => p.Id == cartId).FirstOrDefaultAsync();

            cartById.Count -= 1;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartById = await _db.ShoppingCart.Where(p => p.Id == cartId).FirstOrDefaultAsync();
            _db.ShoppingCart.Remove(cartById);
            await _db.SaveChangesAsync();

            var userCartCount = _db.ShoppingCart.Where(p => p.ApplicationUserId == cartById.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, userCartCount);

            if (userCartCount < 1)
            {
                return Redirect("~/");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailCartVM.ListCart = _db.ShoppingCart.Where(p => p.ApplicationUserId == claim.Value).ToList();

            DetailCartVM.OrderHeader.OrderDate = DateTime.Now;
            DetailCartVM.OrderHeader.UserId = claim.Value;

            _db.OrderHeader.Add(DetailCartVM.OrderHeader);
            await _db.SaveChangesAsync();

            DetailCartVM.OrderHeader.OrderTotalOrignal = 0;

            foreach (var item in DetailCartVM.ListCart)
            {
                item.MenuItem = _db.MenuItem.Where(p => p.Id == item.MenuItemId).FirstOrDefault();
                OrderDetails orderDetails = new()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = DetailCartVM.OrderHeader.Id,
                    Name = item.MenuItem.Name,
                    Description = item.MenuItem.Description,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                DetailCartVM.OrderHeader.OrderTotalOrignal += orderDetails.Price * orderDetails.Count;
                _db.OrderDetails.Add(orderDetails);
            }
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                DetailCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _db.Coupon.Where(p => p.Name.ToLower() == DetailCartVM.OrderHeader.CouponCode.ToLower()).FirstOrDefault();
                DetailCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDb, DetailCartVM.OrderHeader.OrderTotalOrignal);
            }
            else
            {
                DetailCartVM.OrderHeader.OrderTotal = DetailCartVM.OrderHeader.OrderTotalOrignal;
            }
            DetailCartVM.OrderHeader.CouponCodeDiscount = DetailCartVM.OrderHeader.OrderTotalOrignal - DetailCartVM.OrderHeader.OrderTotal;
            _db.ShoppingCart.RemoveRange(DetailCartVM.ListCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);

            await _db.SaveChangesAsync();

            return RedirectToAction("Confirm", "Order", new { id = DetailCartVM.OrderHeader.Id });
        }
    }
}
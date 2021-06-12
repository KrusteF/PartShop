using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PartShop.Data;
using PartShop.Models;
using PartShop.Utility;

namespace PartShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminUser)]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Coupon CouponModel { get; set; }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponById = await _db.Coupon.FindAsync(id);
            if (couponById == null)
            {
                return NotFound();
            }
            return View(couponById);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponById = await _db.Coupon.FindAsync(id);
            if (couponById == null)
            {
                return NotFound();
            }
            return View(couponById);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponById = await _db.Coupon.FindAsync(id);
            if (couponById == null)
            {
                return NotFound();
            }
            return View(couponById);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using var ms1 = new MemoryStream();
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    CouponModel.Picture = p1;
                }
                _db.Coupon.Add(CouponModel);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(CouponModel);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponById = await _db.Coupon.FindAsync(id);
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using var ms1 = new MemoryStream();
                        fs1.CopyTo(ms1);
                        p1 = ms1.ToArray();
                    }
                    couponById.Picture = p1;
                }
                couponById.Name = CouponModel.Name;
                couponById.CouponType = CouponModel.CouponType;
                couponById.Discount = CouponModel.Discount;
                couponById.MinimumAmount = CouponModel.MinimumAmount;
                couponById.IsActive = CouponModel.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(CouponModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var couponById = await _db.Coupon.FindAsync(id);
            if (couponById == null)
            {
                return NotFound();
            }
            _db.Coupon.Remove(couponById);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
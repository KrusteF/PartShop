using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PartShop.Data;
using PartShop.Utility;

namespace PartShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminUser)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.ApplicationUser.Where(p => p.Id != claim.Value).ToListAsync());
        }

        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userById = await _db.ApplicationUser.FirstOrDefaultAsync(p => p.Id == id);
            if (userById == null)
            {
                return NotFound();
            }
            userById.LockoutEnd = DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userById = await _db.ApplicationUser.FirstOrDefaultAsync(p => p.Id == id);
            if (userById == null)
            {
                return NotFound();
            }
            userById.LockoutEnd = DateTime.Now;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var userById = await _db.ApplicationUser.FirstOrDefaultAsync(p => p.Id == id);

            if (userById == null) return NotFound();
            _db.ApplicationUser.Remove(userById);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
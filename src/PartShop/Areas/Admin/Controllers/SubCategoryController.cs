namespace PartShop.Areas.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    using PartShop.Data;
    using PartShop.Models;
    using PartShop.Models.ViewModels;
    using PartShop.Utility;

    [Area("Admin")]
    [Authorize(Roles = SD.AdminUser)]
    public class SubCategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var subCategory = await _db.SubCategory.Include(m => m.Category).ToListAsync();
            return View(subCategory);
        }

        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(m => m.Name).Select(u => u.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExits = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExits.Any())
                {
                    StatusMessage = "Error : Sub Category exists under" + doesSubCategoryExits.First().Category.Name + "category.Please use another name";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVm = new()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVm);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new();

            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(p => p.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(m => m.Name).Select(u => u.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExits = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExits.Any())
                {
                    StatusMessage = "Error : Sub Category exists under" + doesSubCategoryExits.First().Category.Name + "category.Please use another name";
                }
                else
                {
                    var subCategory = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCategory.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVm = new()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };
            return View(modelVm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(p => p.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(m => m.Name).Select(u => u.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(p => p.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(m => m.Name).Select(u => u.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var selectedSubCategory = await _db.SubCategory.FindAsync(id);
            if (selectedSubCategory == null)
            {
                return NotFound();
            }
            _db.SubCategory.Remove(selectedSubCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
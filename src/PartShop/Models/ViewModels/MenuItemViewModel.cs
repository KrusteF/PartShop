using System.Collections.Generic;

namespace PartShop.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<SubCategory> SubCategoryList { get; set; }
    }
}

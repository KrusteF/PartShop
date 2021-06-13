namespace PartShop.Models.ViewModels
{
    using System.Collections.Generic;

    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<SubCategory> SubCategoryList { get; set; }
    }
}

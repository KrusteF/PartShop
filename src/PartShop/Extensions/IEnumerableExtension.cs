using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace PartShop.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedList)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedList.ToString())
                   };
        }
    }
}

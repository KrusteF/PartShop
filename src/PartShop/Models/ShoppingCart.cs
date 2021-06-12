using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartShop.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Your Count should be atleast greater or equal to {1}")]
        public int Count { get; set; }
    }
}

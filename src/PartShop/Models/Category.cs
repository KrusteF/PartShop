using System.ComponentModel.DataAnnotations;

namespace PartShop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; }
    }
}

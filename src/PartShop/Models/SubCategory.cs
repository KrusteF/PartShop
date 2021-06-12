using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartShop.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [Display(Name = "Категория")]
        public virtual Category Category { get; set; }
    }
}

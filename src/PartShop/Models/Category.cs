namespace PartShop.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; }
    }
}

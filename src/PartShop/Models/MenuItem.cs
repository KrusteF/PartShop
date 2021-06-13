namespace PartShop.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Име")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Снимка")]
        public string Image { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Подкатегория")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Цената трябва да е по-висока от ${1}")]

        [Display(Name = "Цена")]
        public double Price { get; set; }
    }
}

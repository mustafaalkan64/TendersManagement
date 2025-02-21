using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Birim Adý")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Kýsa Kod")]
        [StringLength(10)]
        public string ShortCode { get; set; }
    }
}
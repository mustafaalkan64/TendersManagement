using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProjectOwner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        [Display(Name = "Ad Soyad")]
        public string Name { get; set; }

        [Required(ErrorMessage = "TC Kimlik No zorunludur")]
        [Display(Name = "TC Kimlik No")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik No 11 karakter olmalıdır")]
        public string IdentityNo { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur")]
        [Display(Name = "Telefon")]
        public string Telephone { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Display(Name = "Traktor")]
        public string Traktor { get; set; }

        [Display(Name = "Hp")]
        public int Hp { get; set; }
    }
}
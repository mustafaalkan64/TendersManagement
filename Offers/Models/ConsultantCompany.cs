using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class ConsultantCompany
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Firma Adı zorunludur")]
        [StringLength(150, MinimumLength = 3)]
        [Display(Name = "Firma Adı")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Ticari Unvan")]
        [StringLength(200)]
        public string TicariUnvan { get; set; }

        [Required]
        [Display(Name = "Vergi No")]
        [StringLength(11)]
        public string VergiNo { get; set; }

        [Display(Name = "Vergi Dairesi")]
        public string VergiDairesiAdi { get; set; }

        [Display(Name = "Ticari Sicil No")]
        public string TicariSicilNo { get; set; }

        [Required]
        [Display(Name = "Adres")]
        [StringLength(maximumLength: 500)]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Telefon")]
        [StringLength(20)]
        [Phone]
        public string Telefon { get; set; }

        [Display(Name = "Faks")]
        public string Faks { get; set; }

        [Required]
        [Display(Name = "E-posta")]
        [EmailAddress]
        [StringLength(100)]
        public string Eposta { get; set; }

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
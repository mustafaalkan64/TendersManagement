using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Proje Adi")]
        public string OfferName { get; set; }

        [Required]
        public int ProjectOwnerId { get; set; }

        [Required]
        [Display(Name = "Hazirlanma Suresi")]
        public int HazirlanmaSuresi { get; set; }

        [Required]
        [Display(Name = "Personel Sayisi")]
        public int PersonelSayisi { get; set; }

        [Required]
        [Display(Name = "OTP Hazirlanma Suresi")]
        public int OtpHazirlanmaSuresi { get; set; }

        [Required]
        [Display(Name = "OTP Personel Sayisi")]
        public int OtpPersonelSayisi { get; set; }

        [Required]
        [Display(Name = "Is Plani Hazirligi Yuzde")]
        public int IsPlaniHazirligiYuzde { get; set; } = 3;

        [Required]
        [Display(Name = "OTPYuzde")]
        public int OTPYuzde { get; set; } = 1;

        public bool IsApproved { get; set; } = false;

        public ProjectOwner ProjectOwner { get; set; }

        [Display(Name = "Toplam Tutar")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Olusturma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        [Display(Name = "Adres")]
        public string ProjectAddress { get; set; }

        public DateTime? TeklifGonderimTarihi { get; set; }

        public DateTime? SonTeklifBildirme { get; set; }

        public DateTime? TeklifGecerlilikSuresi { get; set; }

        public DateTime? DanismanlikTeklifGonderim { get; set; }

        public DateTime? DanismanlikSonTeklifBitis { get; set; }

        public DateTime? DanismanlikSonTeklifSunum { get; set; }

        public DateTime? DanismanlikTeklifGecerlilikSuresi { get; set; }

        public DateTime? TeklifSunumTarihi {  get; set; }

        public List<OfferItem> OfferItems { get; set; } = new List<OfferItem>();
    }
}
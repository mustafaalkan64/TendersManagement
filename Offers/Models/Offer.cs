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

        public ProjectOwner ProjectOwner { get; set; }

        [Display(Name = "Toplam Tutar")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Oluþturma Tarihi")]
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
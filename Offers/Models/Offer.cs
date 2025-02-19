using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Teklif Adý")]
        public string OfferName { get; set; }

        [Display(Name = "Toplam Tutar")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Oluþturma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public List<OfferItem> OfferItems { get; set; } = new List<OfferItem>();
    }
}
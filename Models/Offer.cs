using System.ComponentModel.DataAnnotations;

public class Offer
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    [Display(Name = "Offer Name")]
    public string OfferName { get; set; }

    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public List<OfferItem> OfferItems { get; set; } = new List<OfferItem>();

    public string ProjectAddress { get; set; }
    public DateTime TeklifGonderimTarihi { get; set; }
    public DateTime SonTeklifBildirme { get; set; }
    public DateTime TeklifGecerlilikSuresi { get; set; }
    public DateTime DanismanlikTeklifGonderim { get; set; }
    public DateTime DanismanlikSonTeklifBitis { get; set; }
    public DateTime DanismanlikSonTeklifSunum { get; set; }
    public DateTime DanismanlikTeklifGecerlilikSuresi { get; set; }
} 
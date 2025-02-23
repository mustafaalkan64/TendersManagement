using System.ComponentModel.DataAnnotations;

public class Company
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Firma Adı")]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    [Display(Name = "Vergi No")]
    [StringLength(11)]
    public string VergiNo { get; set; }

    [Required]
    [Display(Name = "Vergi Dairesi")]
    [StringLength(100)]
    public string VergiDairesiAdi { get; set; }

    [Required]
    [Display(Name = "Ticari Sicil No")]
    [StringLength(50)]
    public string TicariSicilNo { get; set; }

    [Required]
    [Display(Name = "Adres")]
    [StringLength(500)]
    public string Address { get; set; }

    [Required]
    [Display(Name = "Telefon")]
    [StringLength(20)]
    [Phone]
    public string Telefon { get; set; }

    [Display(Name = "Faks")]
    [StringLength(20)]
    public string Faks { get; set; }

    [Required]
    [Display(Name = "E-posta")]
    [EmailAddress]
    [StringLength(100)]
    public string Eposta { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
} 
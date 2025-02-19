using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OfferItem
{
    public int Id { get; set; }

    [Required]
    public int OfferId { get; set; }
    
    [Required]
    [Display(Name = "Equipment Model")]
    public int EquipmentModelId { get; set; }

    [Required]
    [Display(Name = "Company")]
    public int CompanyId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    // Navigation properties
    public Offer Offer { get; set; }
    public EquipmentModel EquipmentModel { get; set; }
    public Company Company { get; set; }
} 
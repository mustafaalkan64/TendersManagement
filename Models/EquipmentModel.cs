using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EquipmentModel
{
    public int Id { get; set; }

    [Required]
    public int EquipmentId { get; set; }

    [Required]
    [StringLength(100)]
    public string Model { get; set; }

    [Required]
    [StringLength(100)]
    public string Brand { get; set; }

    // Navigation property
    public Equipment Equipment { get; set; }
} 
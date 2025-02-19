using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class Equipment
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    // Navigation property
    public ICollection<EquipmentModel> Models { get; set; }
} 
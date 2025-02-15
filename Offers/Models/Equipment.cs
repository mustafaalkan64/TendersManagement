using System.ComponentModel.DataAnnotations;

namespace Models;

public class Equipment
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Equipment Name is required")]
    [StringLength(100, MinimumLength = 2)]
    [Display(Name = "Equipment Name")]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100, MinimumLength = 2)]
    [Required(ErrorMessage = "Brand is required")]
    public string Brand { get; set; }
}



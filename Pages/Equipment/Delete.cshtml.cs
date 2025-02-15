using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EquipmentApp.Pages.Equipment
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        // ... existing code ...
    }
} 
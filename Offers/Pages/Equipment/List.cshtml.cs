using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

[Authorize(Roles = "Admin, User")]
public class EquipmentListModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EquipmentListModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Equipment> Equipment { get; set; } = new List<Equipment>();

    public async Task OnGetAsync()
    {
        Equipment = await _context.Equipment.ToListAsync();
    }
}

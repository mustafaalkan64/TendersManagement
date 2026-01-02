using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

[Authorize(Policy = "CanListEquipment")]
public class EquipmentListModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public EquipmentListModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string SearchString { get; set; }

    public IList<Equipment> Equipment { get; set; } = new List<Equipment>();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var query = _context.Equipment
            .Include(x => x.Features)
            .AsQueryable();

        if (!string.IsNullOrEmpty(SearchString))
        {
            query = query.Where(em =>
                em.Name.Contains(SearchString) ||
                em.Description.Contains(SearchString));
        }

        Equipment = await query.ToListAsync(cancellationToken);
    }
}

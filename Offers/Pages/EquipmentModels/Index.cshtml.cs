using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pages.EquipmentModelPage
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<EquipmentModel> EquipmentModels { get; set; }

        public async Task OnGetAsync()
        {
            EquipmentModels = await _context.EquipmentModels
                .Include(e => e.Equipment)
                .OrderBy(e => e.Equipment.Name)
                .ThenBy(e => e.Brand)
                .ThenBy(e => e.Model)
                .ToListAsync();
        }
    }
} 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Threading.Tasks;

namespace Pages.EquipmentModelPage
{
    [Authorize(Policy = "CanDeleteEquipmentModel")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EquipmentModel EquipmentModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EquipmentModel = await _context.EquipmentModels
                .Include(e => e.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (EquipmentModel == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EquipmentModel = await _context.EquipmentModels.FindAsync(id);

            if (EquipmentModel != null)
            {
                // Check if this model is used in any offers
                var isUsed = await _context.OfferItems.AnyAsync(oi => oi.EquipmentModelId == id);
                if (isUsed)
                {
                    ModelState.AddModelError("", "This equipment model cannot be deleted because it is used in one or more offers.");
                    EquipmentModel = await _context.EquipmentModels
                        .Include(e => e.Equipment)
                        .FirstOrDefaultAsync(m => m.Id == id);
                    return Page();
                }

                _context.EquipmentModels.Remove(EquipmentModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
} 
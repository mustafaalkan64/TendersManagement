using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentModels.Pages
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EquipmentModel EquipmentModel { get; set; }
        public SelectList EquipmentList { get; set; }

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

            await LoadEquipmentList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadEquipmentList();
                return Page();
            }

            _context.Attach(EquipmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentModelExists(EquipmentModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadEquipmentList()
        {
            EquipmentList = new SelectList(
                await _context.Equipment.OrderBy(e => e.Name).ToListAsync(),
                "Id",
                "Name"
            );
        }

        private bool EquipmentModelExists(int id)
        {
            return _context.EquipmentModels.Any(e => e.Id == id);
        }
    }
} 
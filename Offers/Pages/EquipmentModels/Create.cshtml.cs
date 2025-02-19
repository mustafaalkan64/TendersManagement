using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;

namespace Pages.EquipmentModelPage
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EquipmentModel EquipmentModel { get; set; }
        public SelectList EquipmentList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadEquipmentList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (EquipmentModel.EquipmentId == null || string.IsNullOrEmpty(EquipmentModel.Brand) || string.IsNullOrEmpty(EquipmentModel.Model))
            {
                await LoadEquipmentList();
                return Page();
            }

            var equipment = await _context.Equipment.FindAsync(EquipmentModel.EquipmentId);

            _context.EquipmentModels.Add(new EquipmentModel() {  
                Brand = EquipmentModel.Brand,
                Model = EquipmentModel.Model,
                EquipmentId = EquipmentModel.EquipmentId
            });
            await _context.SaveChangesAsync();

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
    }
}
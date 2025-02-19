using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Pages.Offers
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
            OfferItems = new List<OfferItem>();
        }

        [BindProperty]
        public Offer Offer { get; set; }

        [BindProperty]
        public OfferItem NewItem { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public List<OfferItem> OfferItems { get; set; }
        public SelectList EquipmentModelList { get; set; }
        public SelectList CompanyList { get; set; }

        private async Task LoadDropDownLists()
        {
            var equipmentModels = await _context.EquipmentModels
            .Include(em => em.Equipment)
            .OrderBy(em => em.Equipment.Name)
            .ThenBy(em => em.Brand)
            .ThenBy(em => em.Model)
            .ToListAsync();

            EquipmentModelList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Please Select Equipment Model" }
                }.Concat(equipmentModels.Select(em => new SelectListItem
                {
                    Value = em.Id.ToString(),
                    Text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
                })), "Value", "Text");

            CompanyList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Please Select Company" }
                }.Concat(await _context.Companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync()), "Value", "Text");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropDownLists();
            // Initialize session for offer items
            HttpContext.Session.Set("OfferItems", OfferItems);
            return Page();
        }

        public async Task<IActionResult> OnPostAddItemAsync()
        {
            if (NewItem == null || NewItem.EquipmentModelId == 0 || NewItem.CompanyId == 0 || NewItem.Price <= 0 || NewItem.Quantity <= 0)
            {
                await LoadDropDownLists();
                return Page();
            }

            // Load existing items from session
            OfferItems = HttpContext.Session.Get<List<OfferItem>>("OfferItems") ?? new List<OfferItem>();

            // Load related entities for display
            // Load related entities for display
            NewItem.EquipmentModel = await _context.EquipmentModels
                .Include(em => em.Equipment)
                .FirstOrDefaultAsync(em => em.Id == NewItem.EquipmentModelId);
            NewItem.Company = await _context.Companies.FindAsync(NewItem.CompanyId);

            if (OfferItems.Any(x => x.CompanyId == NewItem.CompanyId) && OfferItems.Any(x => x.EquipmentModelId == NewItem.EquipmentModelId))
            {
                await LoadDropDownLists();
                StatusMessage = "Zaten kurum bu ekipmana teklif vermiþ";
                return Page();
            }

            if (OfferItems.Any())
            {

                var filteredItems = OfferItems.Where(x => x.EquipmentModelId == NewItem.EquipmentModelId);
                var minOffer = filteredItems.Any() ? filteredItems.Min(x => x.Price) : (decimal?)null;

                if (minOffer != null)
                {
                    var twentyPercentMore = (double)minOffer * (1.2);

                    if (NewItem.Price < minOffer)
                    {
                        StatusMessage = "Teklif tutarý en düþük teklif tutarýndan düþük olamaz";
                        await LoadDropDownLists();
                        return Page();
                    }

                    if ((double)NewItem.Price > twentyPercentMore)
                    {
                        StatusMessage = "Teklif tutarý en düþük teklif tutarýnýn %20sinden fazla olamaz";
                        await LoadDropDownLists();

                        return Page();
                    }
                }
            }

            OfferItems.Add(NewItem);

            // Save back to session
            HttpContext.Session.Set("OfferItems", OfferItems);

            await LoadDropDownLists();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int index)
        {
            OfferItems = HttpContext.Session.Get<List<OfferItem>>("OfferItems") ?? new List<OfferItem>();

            if (index >= 0 && index < OfferItems.Count)
            {
                OfferItems.RemoveAt(index);
                HttpContext.Session.Set("OfferItems", OfferItems);
            }

            await LoadDropDownLists();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            OfferItems = HttpContext.Session.Get<List<OfferItem>>("OfferItems") ?? new List<OfferItem>();

            //!ModelState.IsValid ||
            if (!OfferItems.Any())
            {
                if (!OfferItems.Any())
                {
                    ModelState.AddModelError("", "At least one offer item is required.");
                }
                await LoadDropDownLists();
                return Page();
            }

            // Calculate total price
            Offer.TotalPrice = CalculateTotalPrice();
            Offer.CreatedDate = DateTime.UtcNow;

            // Add offer
            _context.Offers.Add(Offer);
            await _context.SaveChangesAsync();

            // Add offer items
            foreach (var item in OfferItems)
            {
                item.OfferId = Offer.Id;
                _context.OfferItems.Add(new OfferItem() { CompanyId = item.CompanyId, EquipmentModelId = item.EquipmentModelId, OfferId = item.OfferId, Price = item.Price, Quantity = item.Quantity });
            }
            await _context.SaveChangesAsync();

            // Clear session
            HttpContext.Session.Remove("OfferItems");

            return RedirectToPage("./Index");
        }

        public decimal CalculateTotalPrice()
        {
            return OfferItems.Sum(item => item.Price * item.Quantity);
        }
    }
}

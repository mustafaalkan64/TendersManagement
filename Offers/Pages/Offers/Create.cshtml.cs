using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Offers.Services.Currency;
using System.Threading;

namespace Pages.Offers
{
    [Authorize(Policy = "CanAddOffer")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;

        public CreateModel(ApplicationDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
            OfferItems = new List<OfferItem>();
        }

        [BindProperty]
        public Offer Offer { get; set; } = new Offer();

        public SelectList ProjectOwnerList { get; set; }
        public SelectList ConsultantList { get; set; }

        [BindProperty]
        public OfferItem NewItem { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public List<OfferItem> OfferItems { get; set; }
        public SelectList EquipmentModelList { get; set; }
        public SelectList CompanyList { get; set; }

        private async Task LoadDropDownLists(CancellationToken cancellationToken = default)
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
                new SelectListItem { Value = "0", Text = "Ekipman modeli seciniz" }
                }.Concat(equipmentModels.Select(em => new SelectListItem
                {
                    Value = em.Id.ToString(),
                    Text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
                })), "Value", "Text");

            CompanyList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Sirket seciniz" }
                }.Concat(await _context.Companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync()), "Value", "Text");

            ProjectOwnerList = new SelectList(
               await _context.ProjectOwners.OrderBy(p => p.Name).ToListAsync(cancellationToken),
               "Id",
               "Name"
           );
            ConsultantList = new SelectList(
               await _context.ConsultantCompanies.OrderBy(p => p.Name).ToListAsync(cancellationToken),
               "Id",
               "Name"
           );
        }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            await LoadDropDownLists(cancellationToken);

            // Fetch current Euro rate
            var euroRate = await _currencyService.GetEuroRateAsync();
            Offer.EuroRate = euroRate;
            Offer.TeklifEuro = Offer.TeklifEuro ?? 40000;
           
            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
        {
            Offer.TotalPrice = 0;
            Offer.IsApproved = false;
            Offer.CreatedDate = DateTime.Now;

            // Add offer
            _context.Offers.Add(Offer);
            await _context.SaveChangesAsync(cancellationToken);

            // Clear session
            HttpContext.Session.Remove("OfferItems");

            return RedirectToPage("./Index");
        }
    }
}

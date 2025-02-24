using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace Pages.Offers
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Offer Offer { get; set; }

        public SelectList ProjectOwnerList { get; set; }

        public List<CompanySummaryViewModel> CompanySummaries { get; set; }

        public Decimal MinOfferAmount { get; set; }
        public string MinOfferCompany { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public OfferItem NewItem { get; set; } = new OfferItem();

        public SelectList EquipmentModelList { get; set; }
        public List<OfferItem> OfferItems { get; set; }
        public SelectList EquipmentList { get; set; }
        public SelectList CompanyList { get; set; }

        private async Task LoadRelatedData(CancellationToken cancellationToken = default)
        {
            var equipmentModels = await _context.EquipmentModels
            .Include(em => em.Equipment)
            .OrderBy(em => em.Equipment.Name)
            .ThenBy(em => em.Brand)
            .ThenBy(em => em.Model)
            .ToListAsync(cancellationToken);  

            CompanySummaries = await _context.OfferItems
                .Where(oi => oi.OfferId == Offer.Id)
                .GroupBy(oi => oi.Company.Name)
                .Select(g => new CompanySummaryViewModel
                {
                    CompanyName = g.Key,
                    TotalPrice = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderBy(s => s.TotalPrice)
                .ToListAsync(cancellationToken);

            MinOfferAmount = CompanySummaries.Any() ? CompanySummaries.First().TotalPrice : 0;
            MinOfferCompany = CompanySummaries.Any() ? CompanySummaries.First().CompanyName : "";

            EquipmentModelList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Ekipman Modeli Seciniz" }
                }.Concat(equipmentModels.Select(em => new SelectListItem
                {
                    Value = em.Id.ToString(),
                    Text = $"{em.Equipment.Name} - {em.Brand} {em.Model}"
                })), "Value", "Text");

            CompanyList = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Value = "0", Text = "Sirket Seciniz" }
                }.Concat(await _context.Companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync(cancellationToken)), "Value", "Text");

            var offer = await GetOfferById(Offer.Id);
            OfferItems = offer.OfferItems.ToList();

            ProjectOwnerList = new SelectList(
               await _context.ProjectOwners.OrderBy(p => p.Name).ToListAsync(cancellationToken),
               "Id",
               "Name"
           );
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Offer =  await GetOfferById(id);

            if (Offer == null)
            {
                return NotFound();
            }

            await LoadRelatedData();
            NewItem.EquipmentModelId = 0;
            NewItem.CompanyId = 0;
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if(string.IsNullOrEmpty(Offer.OfferName))
            {
                await LoadRelatedData();
                return Page();
            }

            if (!(Offer.SonTeklifBildirme > Offer.TeklifGonderimTarihi))
            {
                StatusMessage = "Son teklif bildirme tarihi teklif gönderim tarihinden sonra olmalýdýr";
                await LoadRelatedData();
                return Page();
            }

            if (!(Offer.TeklifSunumTarihi > Offer.TeklifGonderimTarihi && Offer.TeklifSunumTarihi <= Offer.SonTeklifBildirme)) {

                StatusMessage = "Teklif sunum tarihi teklif gonderim tarihinden sonra ve son teklif bildirme tarihinden önce olmalýdýr";
                await LoadRelatedData();
                return Page();
            }

            _context.Attach(Offer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(Offer.Id))
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

        public async Task<IActionResult> OnPostAddItemAsync()
        {
            var offer = await GetOfferById(Offer.Id);
            
            OfferItems = offer.OfferItems.ToList();

            if (NewItem == null || NewItem.EquipmentModelId == 0 || NewItem.CompanyId == 0 || NewItem.Price <= 0 || NewItem.Quantity <= 0)
            {
                await LoadRelatedData();
                await UpdateOfferTotalPrice(Offer.Id);
                return Page();
            }

            NewItem.OfferId = Offer.Id;

            if(OfferItems.Any(x => x.CompanyId == NewItem.CompanyId) && OfferItems.Any(x => x.EquipmentModelId == NewItem.EquipmentModelId))
            {
                await LoadRelatedData();

                StatusMessage = "Zaten kurum bu ekipmana teklif vermiþ";
                await UpdateOfferTotalPrice(Offer.Id);
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
                        await LoadRelatedData();

                        StatusMessage = "Teklif tutarý en düþük teklif tutarýndan düþük olamaz";
                        await UpdateOfferTotalPrice(Offer.Id);
                        return Page();
                    }

                    if ((double)NewItem.Price > twentyPercentMore)
                    {
                        await LoadRelatedData();

                        StatusMessage = "Teklif tutarý en düþük teklif tutarýnýn %20sinden fazla olamaz";
                        await UpdateOfferTotalPrice(Offer.Id);
                        return Page();
                    }
                }
            }

            _context.OfferItems.Add(NewItem);
            await _context.SaveChangesAsync();

            // Update total price
            await UpdateOfferTotalPrice(Offer.Id);

            return RedirectToPage("./Edit", new { id = Offer.Id });
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(int itemId)
        {
            var offerItem = await _context.OfferItems.FindAsync(itemId);
            if (offerItem == null)
            {
                return NotFound();
            }

            var offerId = offerItem.OfferId;
            _context.OfferItems.Remove(offerItem);
            await _context.SaveChangesAsync();

            // Update total price
            await UpdateOfferTotalPrice(offerId);

            return RedirectToPage("./Edit", new { id = offerId });
        }

        private async Task UpdateOfferTotalPrice(int offerId)
        {
            var offer = await _context.Offers.AsNoTracking()
                .Include(o => o.OfferItems)
                .FirstOrDefaultAsync(o => o.Id == offerId).ConfigureAwait(false);

            if (offer != null)
            {
                _context.Entry(offer).State = EntityState.Modified;
                offer.TotalPrice = offer.OfferItems.Sum(item => item.Price * item.Quantity);
                Offer.TotalPrice = offer.TotalPrice;
            }
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.Id == id);
        }

        private async Task<Offer> GetOfferById(int? id)
        {

            return await _context.Offers
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.EquipmentModel)
                    .ThenInclude(em => em.Equipment)
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.Company)
            .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
} 
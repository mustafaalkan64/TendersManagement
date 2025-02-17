using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public OfferItem NewItem { get; set; } = new OfferItem();

        public List<OfferItem> OfferItems { get; set; }
        public SelectList EquipmentList { get; set; }
        public SelectList CompanyList { get; set; }

        private async Task LoadRelatedData()
        {
            var equipments = await _context.Equipment.ToListAsync();
            var companies = await _context.Companies.ToListAsync();

            EquipmentList = new SelectList(
                new List<SelectListItem>
                {
            new SelectListItem { Value = "0", Text = "Please Select Equipment" }
                }.Concat(equipments.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })), "Value", "Text");

            CompanyList = new SelectList(
                new List<SelectListItem>
                {
            new SelectListItem { Value = "0", Text = "Please Select Company" }
                }.Concat(companies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })), "Value", "Text");
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Offer =  await GetOfferById(id);
            Offer.TotalPrice = Offer.OfferItems.Sum(item => item.Price * item.Quantity);

            if (Offer == null)
            {
                return NotFound();
            }

            await LoadRelatedData();
            OfferItems = Offer.OfferItems.ToList();
            NewItem.EquipmentId = 0;
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

            var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == Offer.Id);

            offer.OfferName = Offer.OfferName;

            _context.Attach(offer).State = EntityState.Modified;

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

            if (NewItem == null || NewItem.EquipmentId == 0 || NewItem.CompanyId == 0 || NewItem.Price <= 0 || NewItem.Quantity <= 0)
            {
                await LoadRelatedData();
                await UpdateOfferTotalPrice(Offer.Id);
                return Page();
            }

            NewItem.OfferId = Offer.Id;

            if(OfferItems.Any(x => x.CompanyId == NewItem.CompanyId) && OfferItems.Any(x => x.EquipmentId == NewItem.EquipmentId))
            {
                await LoadRelatedData();

                StatusMessage = "Zaten kurum bu ekipmana teklif vermiþ";
                await UpdateOfferTotalPrice(Offer.Id);
                return Page();
            }


            if (OfferItems.Any())
            {

                var minOffer = OfferItems.Min(x => x.Price);

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
                   .ThenInclude(oi => oi.Equipment)
               .Include(o => o.OfferItems)
                   .ThenInclude(oi => oi.Company)
               .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
} 
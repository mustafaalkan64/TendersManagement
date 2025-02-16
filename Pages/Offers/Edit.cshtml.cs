using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
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

        [BindProperty]
        public OfferItem NewItem { get; set; }

        public List<OfferItem> OfferItems { get; set; }
        public SelectList EquipmentList { get; set; }
        public SelectList CompanyList { get; set; }

        private async Task LoadRelatedData()
        {
            EquipmentList = new SelectList(await _context.Equipment.ToListAsync(), "Id", "Name");
            CompanyList = new SelectList(await _context.Companies.ToListAsync(), "Id", "Name");
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadRelatedData();

            if (id == null)
            {
                Offer = new Offer();
                OfferItems = new List<OfferItem>();
                return Page();
            }

            Offer = await _context.Offers
                .Include(o => o.OfferItems)
                    .ThenInclude(oi => oi.Equipment)
                .Include(o => o.OfferItems)
                    .ThenInclude(oi => oi.Company)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (Offer == null)
            {
                return NotFound();
            }

            OfferItems = Offer.OfferItems.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadRelatedData();
                return Page();
            }

            if (Offer.Id == 0)
            {
                _context.Offers.Add(Offer);
            }
            else
            {
                _context.Attach(Offer).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Edit", new { id = Offer.Id });
        }

        public async Task<IActionResult> OnPostAddItemAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadRelatedData();
                return Page();
            }

            NewItem.OfferId = Offer.Id;
            _context.OfferItems.Add(NewItem);
            await _context.SaveChangesAsync();

            // Update total price
            await UpdateOfferTotalPrice(Offer.Id);

            return RedirectToPage("./Edit", new { id = Offer.Id });
        }

        public async Task<IActionResult> OnPostDeleteItemAsync(int itemId)
        {
            var offerItem = await _context.OfferItems.FindAsync(itemId);
            if (offerItem != null)
            {
                _context.OfferItems.Remove(offerItem);
                await _context.SaveChangesAsync();

                // Update total price
                await UpdateOfferTotalPrice(offerItem.OfferId);
            }

            return RedirectToPage("./Edit", new { id = Offer.Id });
        }

        private async Task UpdateOfferTotalPrice(int offerId)
        {
            var offer = await _context.Offers
                .Include(o => o.OfferItems)
                .FirstOrDefaultAsync(o => o.Id == offerId);

            if (offer != null)
            {
                offer.TotalPrice = offer.OfferItems.Sum(item => item.Price * item.Quantity);
                await _context.SaveChangesAsync();
            }
        }
    }
} 
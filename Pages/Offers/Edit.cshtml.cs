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
        public SelectList EquipmentModelList { get; set; }
        public SelectList CompanyList { get; set; }

        private async Task LoadRelatedData()
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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Offer = await _context.Offers
                .Include(o => o.OfferItems)
                    .ThenInclude(oi => oi.EquipmentModel)
                        .ThenInclude(em => em.Equipment)
                .Include(o => o.OfferItems)
                    .ThenInclude(oi => oi.Company)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (Offer == null)
            {
                return NotFound();
            }

            OfferItems = Offer.OfferItems.ToList();
            await LoadRelatedData();
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
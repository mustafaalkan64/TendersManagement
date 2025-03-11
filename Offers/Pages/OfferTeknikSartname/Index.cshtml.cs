using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Offers.Services.Offer;
using System.Text;

namespace Pages.OfferTeknikSartname
{
    [Authorize(Policy = "CanListTeknikSartname")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IOfferService _offerService;

        public IndexModel(ApplicationDbContext context, IMemoryCache cache, IOfferService offerService)
        {
            _context = context;
            _cache = cache;
            _offerService = offerService;
        }

        public IList<Models.OfferTeknikSartname> OfferTeknikSartnames { get; set; }
        public int OfferId { get; set; }

        public async Task OnGetAsync(int offerId, CancellationToken cancellationToken = default)
        {
            OfferId = offerId;

            var offer = await GetOfferByIdAsync(OfferId);

            var offerItems = offer.OfferItems.ToList();

            var offerTeknikSartnames = await _context.OfferTeknikSartnames
                .Where(o => o.OfferId == offerId)
                .ToListAsync(cancellationToken);

            if(!offerTeknikSartnames.Any())
            {
                var offerTeknikSartNameList = await _offerService.GetOfferTeknikSartnameByOfferId(offerId);

                OfferTeknikSartnames = offerTeknikSartNameList;
            }
            else
            {
                OfferTeknikSartnames = offerTeknikSartnames;
            }
        }

        private async Task<Offer?> GetOfferByIdAsync(int? id)
        {
            return await _offerService.GetOfferByIdAsync(id);
        }
    }
}

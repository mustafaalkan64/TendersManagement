using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Models;
using System.Text;

namespace Offers.Services.Offer
{
    public class OfferService: IOfferService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        public OfferService(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<Models.Offer?> GetOfferByIdAsync(int? id, CancellationToken cancellationToken = default)
        {
            string cacheKey = $"offer_{id}"; // Unique key for each offer

            // Try to get the cached data first
            if (!_cache.TryGetValue(cacheKey, out Models.Offer cachedOffer))
            {
                // Data not in cache, fetch from the database
                cachedOffer = await _context.Offers
                    .Include(o => o.ProjectOwner)
                    .Include(o => o.OfferItems)
                        .ThenInclude(oi => oi.EquipmentModel)
                            .ThenInclude(em => em.Equipment)
                                .ThenInclude(em => em.Features)
                    .Include(o => o.OfferItems)
                        .ThenInclude(oi => oi.Company)
                    .Include(o => o.OfferItems)
                        .ThenInclude(oi => oi.EquipmentModel)
                            .ThenInclude(em => em.Features)
                                .ThenInclude(o => o.Unit)
                    .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

                if (cachedOffer != null)
                {
                    // Set the data in the cache with an expiration time of 1 hour
                    _cache.Set(cacheKey, cachedOffer, TimeSpan.FromHours(1));
                }
            }

            return cachedOffer; // Return the cached or fetched offer
        }

        public async Task<List<OfferTeknikSartname>> GetOfferTeknikSartnameByOfferId(int? offerId, CancellationToken cancellationToken = default)
        {
            var offer = await GetOfferByIdAsync(offerId);
            var minMaxFeatures = await _context.EquipmentModelFeatures
                        .GroupBy(emf => new { emf.EquipmentModel.EquipmentId, emf.FeatureKey })
                        .Select(g => new
                        {
                            EquipmentId = g.Key.EquipmentId,
                            FeatureKey = g.Key.FeatureKey,
                            MinFeatureValue = g.Min(emf => emf.FeatureValue),
                            MaxFeatureValue = g.Max(emf => emf.FeatureValue)
                        })
                        .ToListAsync(cancellationToken);

            int no = 1;
            List<Models.OfferTeknikSartname> offerTeknikSartnameList = new();
            foreach (var offerItem in offer.OfferItems.ToList())
            {
                var equipment = offerItem.EquipmentModel.Equipment.Name;
                if (offerTeknikSartnameList.Any(x => x.EquipmentName == equipment))
                {
                    continue;
                }
                var result = new StringBuilder();
                foreach (var feature in offerItem.EquipmentModel?.Features?.ToList())
                {
                    var min = offerItem.EquipmentModel.Equipment.Features.FirstOrDefault(x => x.FeatureKey == feature.FeatureKey)?.Min;
                    var max = offerItem.EquipmentModel.Equipment.Features.FirstOrDefault(x => x.FeatureKey == feature.FeatureKey)?.Max;


                    if (min != null && max != null)
                    {
                        if (minMaxFeatures.Any(x => x.FeatureKey == feature.FeatureKey && x.EquipmentId == feature.EquipmentModel.EquipmentId))
                        {
                            var minMaxFeature = minMaxFeatures.FirstOrDefault(x => x.FeatureKey == feature.FeatureKey);
                            var minVal = int.Parse(minMaxFeature.MinFeatureValue) - min;
                            var maxVal = int.Parse(minMaxFeature.MaxFeatureValue) + max;
                            result.AppendLine($"{feature.FeatureKey} {minVal}-{maxVal} {feature.Unit?.Name?.ToString().Replace("-", "") ?? ""}");
                        }
                    }
                    else
                    {
                        result.AppendLine($"{feature.FeatureKey} {feature.FeatureValue} {feature.Unit?.Name?.ToString().Replace("-", "") ?? ""}");
                    }
                }
                var features = result.ToString();
                var sayi = offerItem.Quantity;

                offerTeknikSartnameList.Add(new Models.OfferTeknikSartname() { No = no, OfferId = offer.Id, Birim = "Adet", Features = features, EquipmentName = equipment, Miktar = sayi });
                no += 1;

            }

            _context.OfferTeknikSartnames.AddRange(offerTeknikSartnameList);
            await _context.SaveChangesAsync(cancellationToken);

            return offerTeknikSartnameList;
        }

        public async Task<List<OfferTeknikSartname>> ReCreateOfferTeknikSartnameByOfferId(int? offerId, CancellationToken cancellationToken = default)
        {
            var offerTeknikSartnameList = await _context.OfferTeknikSartnames.Where(x => x.OfferId == offerId).ToListAsync(cancellationToken);
            _context.OfferTeknikSartnames.RemoveRange(offerTeknikSartnameList);
            return await GetOfferTeknikSartnameByOfferId(offerId, cancellationToken);
        }
    }
}

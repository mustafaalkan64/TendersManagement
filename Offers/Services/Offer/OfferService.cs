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
                        var values = offer.OfferItems.Where(x => x.EquipmentModel.Features.Any(x => x.FeatureKey == feature.FeatureKey) && x.EquipmentModel.EquipmentId == feature.EquipmentModel.EquipmentId).Select(x => x.EquipmentModel).ToList();
                        if (values != null)
                        {
                            var filteredFeatures = values
                                .SelectMany(x => x.Features)
                                .Where(y => y.FeatureKey == feature.FeatureKey)
                                .Select(y => y.FeatureValue)
                                .ToList();

                            var minValue = filteredFeatures.Any() ? filteredFeatures.Min() : default;
                            var maxValue = filteredFeatures.Any() ? filteredFeatures.Max() : default;

                            var minVal = int.Parse(minValue) - min;
                            var maxVal = int.Parse(maxValue) + max;
                            if(feature.FeatureKey.Contains("Güç", StringComparison.Ordinal) && offer.ProjectOwner.Hp > maxVal)
                                maxVal = offer.ProjectOwner.Hp;

                            if (feature.FeatureKey.Contains("Ebat", StringComparison.Ordinal) ||
                                feature.FeatureKey.Contains("Lastik Ebadı", StringComparison.Ordinal) ||
                                feature.FeatureKey.Contains("Boyut", StringComparison.Ordinal))
                            {
                                result.AppendLine($"{feature.FeatureKey} Belirtiniz.");
                            }
                            else 
                                result.AppendLine($"{feature.FeatureKey} {minVal}-{maxVal} {feature.Unit?.Name?.ToString().Replace("-", "") ?? ""}");
                        }
                    }
                    else
                    {
                        if (feature.FeatureKey.Contains("Ebat", StringComparison.Ordinal) ||
                                feature.FeatureKey.Contains("Lastik Ebadı", StringComparison.Ordinal) ||
                                feature.FeatureKey.Contains("Boyut", StringComparison.Ordinal))
                        {
                            result.AppendLine($"{feature.FeatureKey} Belirtiniz.");
                        }
                        else
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

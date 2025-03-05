using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Text;

namespace Pages.OfferTeknikSartname
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.OfferTeknikSartname> OfferTeknikSartnames { get; set; }
        public int OfferId { get; set; }

        public async Task OnGetAsync(int offerId, CancellationToken cancellationToken = default)
        {
            OfferId = offerId;

            var offer = await GetOfferById(OfferId);

            var offerItems = offer.OfferItems.ToList();

            var offerTeknikSartnames = await _context.OfferTeknikSartnames
                .Where(o => o.OfferId == offerId)
                .ToListAsync(cancellationToken);

            if(!offerTeknikSartnames.Any())
            {
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
                foreach (var offerItem in offerItems.ToList())
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

                    offerTeknikSartnameList.Add(new Models.OfferTeknikSartname() { No = no, OfferId = offerId, Birim = "Adet", Features = features, EquipmentName = equipment, Miktar = sayi });
                    no += 1;

                }

                _context.OfferTeknikSartnames.AddRange(offerTeknikSartnameList);
                await _context.SaveChangesAsync(cancellationToken);

                OfferTeknikSartnames = offerTeknikSartnameList;
            }
            else
            {
                OfferTeknikSartnames = offerTeknikSartnames;
            }
        }

        private async Task<Offer> GetOfferById(int? id)
        {

            return await _context.Offers
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
            .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}

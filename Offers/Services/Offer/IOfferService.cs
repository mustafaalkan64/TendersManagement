using Models;

namespace Offers.Services.Offer
{
    public interface IOfferService
    {
        Task<Models.Offer?> GetOfferByIdAsync(int? id, CancellationToken cancellationToken = default);
        Task<List<OfferTeknikSartname>> GetOfferTeknikSartnameByOfferId(int? offerId, CancellationToken cancellationToken = default);
        Task<List<OfferTeknikSartname>> ReCreateOfferTeknikSartnameByOfferId(int? offerId, CancellationToken cancellationToken = default);
    }
}

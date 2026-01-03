namespace Offers.Services.Currency
{
    public interface ICurrencyService
    {
        Task<decimal?> GetEuroRateAsync();
        Task<decimal?> GetEuroRateAsync(DateTime date);
    }
}

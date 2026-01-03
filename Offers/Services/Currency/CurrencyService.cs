using System.Xml.Linq;

namespace Offers.Services.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(HttpClient httpClient, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal?> GetEuroRateAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("kurlar/today.xml");
                var xml = XDocument.Parse(response);

                var euroRate = xml.Descendants("Currency")
                    .FirstOrDefault(c => c.Attribute("CurrencyCode")?.Value == "EUR")
                    ?.Element("ForexBuying")
                    ?.Value;

                if (decimal.TryParse(euroRate, System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out var rate))
                {
                    return rate;
                }

                _logger.LogWarning("Could not parse Euro rate from TCMB");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Euro rate from TCMB");
                return null;
            }
        }

        public async Task<decimal?> GetEuroRateAsync(DateTime date)
        {
            var maxRetry = 5;

            for (int i = 0; i < maxRetry; i++)
            {
                var tryDate = date.AddDays(-i);

                var url = tryDate.Date == DateTime.Today
                    ? "kurlar/today.xml"
                    : $"kurlar/{tryDate:yyyyMM}/{tryDate:ddMMyyyy}.xml";

                try
                {
                    var response = await _httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        continue;

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var xml = XDocument.Parse(xmlContent);

                    var euroRate = xml.Descendants("Currency")
                        .FirstOrDefault(c => c.Attribute("CurrencyCode")?.Value == "EUR")
                        ?.Element("ForexBuying")
                        ?.Value;

                    if (decimal.TryParse(
                        euroRate,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var rate))
                    {
                        return rate;
                    }
                }
                catch (HttpRequestException)
                {
                    continue;
                }
            }

            _logger.LogWarning(
                "Euro rate not found for {Date} or previous {Retry} days",
                date.ToString("yyyy-MM-dd"),
                maxRetry);

            return null;
        }
    }
}

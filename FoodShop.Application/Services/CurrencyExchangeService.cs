using Newtonsoft.Json;


namespace FoodShop.Application.Services
{
    public class CurrencyExchangeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public CurrencyExchangeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = "e6850091da2ae019a4577f8875638dfd";
            _apiUrl = "https://api.exchangeratesapi.io/v1/latest";
        }

        public async Task<decimal> GetUsdToVndRateAsync()
        {
            var response = await _httpClient.GetStringAsync(
                $"{_apiUrl}?access_key={_apiKey}&base=USD");
            var result = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);
            return result.Rates["VND"];
        }
    }

    public class ExchangeRateResponse
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }

}

namespace Algobot.Worker.Configuration
{
    public class CoinMarketCapOptions
    {
        public const string CoinMarketCap = "CoinMarketCap";

        public string ApiKey { get; set; } = string.Empty;

        public string Url { get; set; } = "https://pro-api.coinmarketcap.com/v1/";

        public string KeyName { get; set; } = "X-CMC_PRO_API_KEY";
    }
}

using Algo.Bot.Application.Ports.Services;
using Algo.Bot.Infrastructure.Adapters.Externals;
using Algo.Bot.Infrastructure.Adapters.HttpClients;
using Algo.Bot.Infrastructure.Configuration;
using Algobot.Worker.Infrastructure;
using CryptoExchange.Net.Authentication;
using Telegram.Bot;

namespace Algobot.Worker.Configuration
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddHttpClients(services, configuration);
            AddExternalServices(services, configuration);
            AddNotification(services, configuration);
        }

        public static void AddHttpClients(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CoinMarketCapOptions>(configuration.GetSection(CoinMarketCapOptions.CoinMarketCap));

            services.AddHttpClient<CoinMarketCapHttpClient>();
        }

        public static void AddExternalServices(IServiceCollection services, IConfiguration configuration)
        {
            var binanceOptions = new BinanceApiOptions();
            configuration.GetSection(BinanceApiOptions.BinanceApi).Bind(binanceOptions);

            services.AddBinance((restClientOptions) => {
                restClientOptions.ApiCredentials = new ApiCredentials(binanceOptions.ApiKey, binanceOptions.SecretKey);
            });

            services.AddTransient<ICryptocurrencyExchangeService, BinanceExchangeService>();
            services.AddTransient<IPostiveSentimentDataProvider, CoinMarketCapService>();
        }

        public static void AddNotification(IServiceCollection services, IConfiguration configuration)
        {
            var telegramOptions = new TelegramOptions();
            configuration.GetSection(TelegramOptions.Telegram).Bind(telegramOptions);

            services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.Telegram));

            services.AddHttpClient<ITelegramBotClient>(httpClient => new TelegramBotClient(telegramOptions.ApiKey, httpClient));
            services.AddSingleton<INotificationService, TelegramService>();
        }
    }
}

using Algobot.Worker.Application.Consumers;
using Algobot.Worker.Domain.ValueObject;
using MassTransit;
using static MassTransit.ValidationResultExtensions;

namespace Algobot.Worker.Infrastructure
{
    public interface IPostiveSentimentDataProvider
    {
        Task<IList<string>> GetSymbols(Interval interval, decimal minimumPercentageChange);
        Task<IList<string>> GetSymbols(Interval interval, IList<string> filterSymbols);
    }

    public class CoinMarketCapService : IPostiveSentimentDataProvider
    {
        private readonly CoinMarketCapHttpClient _marketCapHttpClient;
        private readonly IBus _bus;
        public CoinMarketCapService(CoinMarketCapHttpClient marketCapHttpClient, IBus bus)
        {
            _marketCapHttpClient = marketCapHttpClient;
            _bus = bus;
        }

        public async Task<IList<string>> GetSymbols(Interval interval, decimal minimumPercentageChange)
        {
            var result = new List<(string, decimal)>();

            if (interval == Interval.OneMinute
             || interval == Interval.FiveMinutes
             || interval == Interval.FifteenMinutes
             || interval == Interval.OneHour)
            {
                result = await _marketCapHttpClient.Get1HSymbols();
            }
            if (interval == Interval.FourHour
             || interval == Interval.OneDay)
            {
                result = await _marketCapHttpClient.Get24HSymbols();
            }

            var message = new TrendingMessage
            {
                Interval = interval,
                Symbols = result.OrderByDescending(x => x.Item2).Where(x => x.Item2 >= minimumPercentageChange).Select(x => x.Item1.Trim()).ToList()
            };

            await _bus.Publish(message);

            return message.Symbols.ToList();
        }

        public async Task<IList<string>> GetSymbols(Interval interval, IList<string> filterSymbols)
        {
            var message = new TrendingMessage
            {
                Interval = interval,
                Symbols = filterSymbols
            };

            await _bus.Publish(message);

            return message.Symbols.ToList();
        }
    }
}

using Algo.Bot.Application.Ports.Services;
using Algo.Bot.Domain.Ports;
using Algo.Bot.Infrastructure.Adapters.HttpClients;
using Algobot.Worker.Domain.ValueObject;
using Algobot.Worker.Infrastructure;
using MassTransit;

namespace Algobot.Worker.Application.Consumers
{
    public class TrendingMessage
    {
        public Interval Interval { get; set; } = Interval.FourHour;

        public ICollection<string> Symbols { get; set; } = [];
    }

    public class TrendingConsumer : IConsumer<TrendingMessage>
    {
        private readonly IDateTimeProvider _timeProvider;
        private readonly ICryptocurrencyExchangeService _exchangeService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TrendingConsumer> _logger;

        private const int LastCandles = 3;

        public TrendingConsumer(ICryptocurrencyExchangeService exchangeService, IDateTimeProvider timeProvider, INotificationService notificationService, ILogger<TrendingConsumer> logger)
        {
            _notificationService = notificationService;
            _exchangeService = exchangeService;
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TrendingMessage> context)
        {
            var symbols = context.Message.Symbols;

            foreach (var symbol in symbols)
            {
                try
                {
                    var candles = await _exchangeService.GetCandles(symbol, context.Message.Interval, LastCandles + 1);

                    if (IsFairValueGap(candles.Take(LastCandles).ToList()))
                    {
                        await _notificationService.Notify($"Fair value gap: {symbol}. Interval: {context.Message.Interval}.");
                    }
                }
                catch (ArgumentException e)
                {
                    _logger.LogWarning(e, e.Message);
                }
            }
        }

        private bool IsFairValueGap(IList<Candle> candles)
        {
            var fairValueGap = candles.First().High < candles.Last().Low;

            foreach (var candle in candles) 
            {
                Console.WriteLine($"Time: {candle.DateTime}. Low: {candle.Low}. High: {candle.High}.");
            }
            Console.WriteLine($"FairValueGap: {fairValueGap}.");

            return fairValueGap;
        }
    }
}
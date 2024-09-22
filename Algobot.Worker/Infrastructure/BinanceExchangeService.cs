using Algo.Bot.Domain.Ports;
using Algo.Bot.Infrastructure.Converters;
using Algobot.Worker.Domain.ValueObject;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;

namespace Algo.Bot.Infrastructure.Adapters.HttpClients
{
    public interface ICryptocurrencyExchangeService
    {
        Task<Candle> GetCandle(string symbol, Interval interval, DateTime start, DateTime end);
        Task<Candle> GetLastCandle(string symbol, Interval interval);
        Task<IList<Candle>> GetCandles(string symbol, Interval interval, DateTime start, DateTime end);
        Task<(int pricePrecision, int quantityPrecision)> GetSymbolPrecisions(string symbol);
    }


    public class BinanceExchangeService : ICryptocurrencyExchangeService
    {
        private readonly IBinanceRestClient _binanceClient;
        private readonly IDateTimeProvider _dateTimeProvider;

        public BinanceExchangeService(IBinanceRestClient binanceClient, IDateTimeProvider dateTimeProvider)
        {
            _binanceClient = binanceClient;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(int pricePrecision, int quantityPrecision)> GetSymbolPrecisions(string symbol)
        {
            var info = await _binanceClient.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync();

            if (info != null 
             && info.Success)
            {
                var symbolFilter = info.Data.Symbols.First(s => s.Name == symbol);

                return (symbolFilter.PricePrecision, symbolFilter.QuantityPrecision);
            }

            throw new ArgumentException($"Precision for {symbol} not found.");
        }

        public async Task<Candle> GetCandle(string symbol, Interval interval, DateTime start, DateTime end)
        {
            var binanceInterval = ToBinanceInterval.Convert(interval);
            var binanceCandles = await _binanceClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, binanceInterval, start, end, 1);
            if (binanceCandles.Data != null
            && binanceCandles.Data.Any())
            {
                var firstCandle = binanceCandles.Data.First();
                return new Candle
                {
                    Symbol = symbol,
                    Interval = interval,
                    DateTime = firstCandle.OpenTime,
                    Open = firstCandle.OpenPrice,
                    Close = firstCandle.ClosePrice,
                    High = firstCandle.HighPrice,
                    Low = firstCandle.LowPrice,
                };
            }

            throw new ArgumentException($"Candle not found at binance exchange. Symbol {symbol}, Interval {interval}.");
        }

        public async Task<IList<Candle>> GetCandles(string symbol, Interval interval, DateTime start, DateTime end)
        {
            var binanceInterval = ToBinanceInterval.Convert(interval);
            var binanceCandles = await _binanceClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, binanceInterval, start, end);
            if (binanceCandles.Data != null
            && binanceCandles.Data.Any())
            {
                return binanceCandles.Data.Select(firstCandle => new Candle
                {
                    Symbol = symbol,
                    Interval = interval,
                    DateTime = firstCandle.OpenTime,
                    Open = firstCandle.OpenPrice,
                    Close = firstCandle.ClosePrice,
                    High = firstCandle.HighPrice,
                    Low = firstCandle.LowPrice,
                }).ToList();
            }

            throw new ArgumentException($"Candle not found at binance exchange. Symbol {symbol}, Interval {interval}.");
        }

        public async Task<Candle> GetLastCandle(string symbol, Interval interval)
        {
            var binanceInterval = ToBinanceInterval.Convert(interval);
            var binanceCandles = await _binanceClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, binanceInterval, null, null, 2);
            if (binanceCandles.Data != null
            && binanceCandles.Data.Any())
            {
                var lastCloseCandle = binanceCandles.Data.Last(x => x.CloseTime < _dateTimeProvider.Now());
                return new Candle
                {
                    Symbol = symbol,
                    Interval = interval,
                    DateTime = lastCloseCandle.OpenTime,
                    Open = lastCloseCandle.OpenPrice,
                    Close = lastCloseCandle.ClosePrice,
                    High = lastCloseCandle.HighPrice,
                    Low = lastCloseCandle.LowPrice,
                };
            }

            throw new ArgumentException($"Candle not found at binance exchange. Symbol {symbol}, Interval {interval}.");
        }
    }
}

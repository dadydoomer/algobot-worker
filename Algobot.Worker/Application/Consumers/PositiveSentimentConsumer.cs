using Algo.Bot.Domain.Ports;
using Algo.Bot.Infrastructure.Adapters.HttpClients;
using Algobot.Worker.Domain.ValueObject;
using MassTransit;

namespace Algobot.Worker.Application.Consumers
{
    public class PositiveSentimentMessage
    {
        public Interval Interval { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class PositiveSentimentConsumer : IConsumer<PositiveSentimentMessage>
    {
        private readonly IDateTimeProvider _timeProvider;
        private readonly ICryptocurrencyExchangeService _exchangeService;

        public PositiveSentimentConsumer(IDateTimeProvider timeProvider, ICryptocurrencyExchangeService exchangeService)
        {
            _timeProvider = timeProvider;
            _exchangeService = exchangeService;
        }

        public Task Consume(ConsumeContext<PositiveSentimentMessage> context)
        {
            throw new Exception();
        }
    }
}

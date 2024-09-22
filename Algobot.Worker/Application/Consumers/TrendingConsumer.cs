using Algobot.Worker.Domain.ValueObject;
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
        public Task Consume(ConsumeContext<TrendingMessage> context)
        {
            return Task.CompletedTask;
        }
    }
}

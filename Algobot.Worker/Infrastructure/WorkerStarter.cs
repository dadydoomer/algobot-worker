using Algobot.Worker.Domain.ValueObject;
using Hangfire;

namespace Algobot.Worker.Infrastructure
{
    public class WorkerStarter : BackgroundService
    {
        private const string FifteenMinutesExpression = "0 0/15 1/1 1/1 * ?";
        private const string OneHourExpression = "0 0 0/1 1/1 * ?";
        private const string FourHoursExpression = "0 0 0/4 1/1 * ?";
        private const string OneDayExpression = "0 0 0 1/1 * ?";

        private readonly IPostiveSentimentDataProvider _postiveSentimentData;

        public WorkerStarter(IPostiveSentimentDataProvider postiveSentimentData)
        {
            _postiveSentimentData = postiveSentimentData;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RecurringJob.AddOrUpdate("15m job", () => _postiveSentimentData.GetSymbols(Interval.FifteenMinutes, 0.4m), FifteenMinutesExpression);
            RecurringJob.AddOrUpdate("1h job", () => _postiveSentimentData.GetSymbols(Interval.OneHour, 1m), OneHourExpression);
            RecurringJob.AddOrUpdate("4h job", () => _postiveSentimentData.GetSymbols(Interval.FourHour, 2m), FourHoursExpression);
            RecurringJob.AddOrUpdate("1d job", () => _postiveSentimentData.GetSymbols(Interval.OneDay, 5m), OneDayExpression);
        }
    }
}

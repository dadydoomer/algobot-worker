﻿using Algobot.Worker.Domain.ValueObject;
using Hangfire;

namespace Algobot.Worker.Infrastructure
{
    public class WorkerStarter : BackgroundService
    {
        private const string FourHoursExpression = "0 0 0/4 1/1 * ?";
        private const string OneDayExpression = "0 0 0 1/1 * ?";

        private readonly IPostiveSentimentDataProvider _postiveSentimentData;

        public WorkerStarter(IPostiveSentimentDataProvider postiveSentimentData)
        {
            _postiveSentimentData = postiveSentimentData;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            RecurringJob.AddOrUpdate("4h job", () => _postiveSentimentData.GetSymbols(Interval.FourHour, 3m), FourHoursExpression);
            RecurringJob.AddOrUpdate("1d job", () => _postiveSentimentData.GetSymbols(Interval.OneDay, 5m), OneDayExpression);
        }
    }
}
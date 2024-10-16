using Algo.Bot.Application.Adapters.Services;
using Algo.Bot.Domain.Ports;
using Algobot.Worker.Application.Consumers;
using Algobot.Worker.Configuration;
using Algobot.Worker.Infrastructure;
using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMassTransit(x => 
{
    x.AddConsumer<TrendingConsumer>();

    x.UsingInMemory((context, configuration) =>
    {
        configuration.ConfigureEndpoints(context);
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IPostiveSentimentDataProvider, CoinMarketCapService>();
builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

builder.Services.AddHangfireServer();

builder.Services.AddHostedService<WorkerStarter>();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

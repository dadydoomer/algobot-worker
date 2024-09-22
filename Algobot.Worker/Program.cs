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

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString(RedisOptions.SectionName);
    options.InstanceName = RedisOptions.SectionName;
    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
    {
        AbortOnConnectFail = true,
        EndPoints = { options.Configuration }
    };
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IStorageService, RedisStorageService>();
builder.Services.AddSingleton<IPostiveSentimentDataProvider, CoinMarketCapService>();

builder.Services.AddHangfire(configuration => configuration
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

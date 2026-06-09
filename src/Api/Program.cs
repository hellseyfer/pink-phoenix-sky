using System.Text.Json.Serialization;
using FlightAggregator.Modules.Bookings;
using FlightAggregator.Modules.Flights;
using FlightAggregator.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

IModule[] modules =
[
    new FlightsModule(),
    new BookingsModule()
];

foreach (var module in modules)
{
    module.RegisterServices(builder.Services);
}

var app = builder.Build();

foreach (var module in modules)
{
    module.MapEndpoints(app);
}

await app.RunAsync();

using System.Text.Json.Serialization;
using FlightAggregator.Core;
using FlightAggregator.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<IFlightProvider, GlobalAirProvider>();
builder.Services.AddSingleton<IFlightProvider, BudgetWingsProvider>();
builder.Services.AddSingleton<IFlightSearchService, FlightSearchService>();
builder.Services.AddSingleton<IBookingService, BookingService>();

var app = builder.Build();

app.MapPost("/api/flights/search", async (
    FlightSearchRequest request,
    IFlightSearchService searchService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Origin) || request.Origin.Length != 3)
    {
        return Results.BadRequest("origin must be a 3-letter IATA code");
    }

    if (string.IsNullOrWhiteSpace(request.Destination) || request.Destination.Length != 3)
    {
        return Results.BadRequest("destination must be a 3-letter IATA code");
    }

    if (request.Passengers <= 0)
    {
        return Results.BadRequest("passengers must be >= 1");
    }

    var offers = await searchService.SearchAsync(request, cancellationToken);
    return Results.Ok(offers);
});

app.MapPost("/api/bookings", async (
    CreateBookingRequest request,
    IBookingService bookingService,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.FlightId))
    {
        return Results.BadRequest("flightId is required");
    }

    if (request.Passengers is null || request.Passengers.Count == 0)
    {
        return Results.BadRequest("passengers must be a non-empty array");
    }

    var response = await bookingService.CreateAsync(request, cancellationToken);
    return Results.Ok(response);
});

app.Run();

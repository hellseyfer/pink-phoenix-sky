using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Infrastructure;

public sealed class ArticAirProvider : IFlightProvider
{
    private const string CurrencyCode = "USD";
    private const decimal MinimumPrice = 49.99m;
    private const decimal LoyaltyDiscount = 10.00m;
    private const string FlightId = "articair-aa789";

    public IReadOnlyList<string> KnownFlightIds => [FlightId];

    public Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(FlightSearchRequest request, CancellationToken cancellationToken)
    {
        var departure = request.DepartureDate.ToDateTime(new TimeOnly(13, 15), DateTimeKind.Utc);
        var arrival = request.DepartureDate.ToDateTime(new TimeOnly(17, 5), DateTimeKind.Utc);

        var baseFare = 100.00m;
        var perPassenger = CalculatePerPassengerPrice(baseFare);
        var total = perPassenger * request.Passengers;

        var offer = new FlightOffer(
            Id: FlightId,
            Provider: "ArticAir",
            FlightNumber: "AA789",
            Origin: request.Origin,
            Destination: request.Destination,
            DepartureTime: new DateTimeOffset(departure),
            ArrivalTime: new DateTimeOffset(arrival),
            DurationMinutes: (int)(arrival - departure).TotalMinutes,
            CabinClass: request.CabinClass,
            PricePerPassenger: perPassenger,
            TotalPrice: total,
            Currency: CurrencyCode
        );

        return Task.FromResult<IReadOnlyList<FlightOffer>>([offer]);
    }

    public static decimal CalculatePerPassengerPrice(decimal baseFare)
    {
        var fareWithMarkup = baseFare * 1.20m;
        var discounted = fareWithMarkup - LoyaltyDiscount;
        var final = discounted < MinimumPrice ? MinimumPrice : discounted;
        return decimal.Round(final, 2, MidpointRounding.AwayFromZero);
    }
}
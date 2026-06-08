using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Infrastructure;

public sealed class BudgetWingsProvider : IFlightProvider
{
    private const string CurrencyCode = "USD";
    private const decimal MinimumPrice = 29.99m;
    private const string FlightId = "budgetwings-bw456";

    public IReadOnlyList<string> KnownFlightIds => [FlightId];

    public Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(FlightSearchRequest request, CancellationToken cancellationToken)
    {
        var departure = request.DepartureDate.ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc);
        var arrival = request.DepartureDate.ToDateTime(new TimeOnly(12, 45), DateTimeKind.Utc);

        var baseFare = 100.00m;
        var perPassenger = CalculatePerPassengerPrice(baseFare);
        var total = perPassenger * request.Passengers;

        var offer = new FlightOffer(
            Id: FlightId,
            Provider: "BudgetWings",
            FlightNumber: "BW456",
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
        var discounted = baseFare - (baseFare * 0.10m);
        var final = discounted < MinimumPrice ? MinimumPrice : discounted;
        return decimal.Round(final, 2, MidpointRounding.AwayFromZero);
    }
}

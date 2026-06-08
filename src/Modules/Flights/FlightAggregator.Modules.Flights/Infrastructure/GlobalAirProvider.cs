using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Infrastructure;

public sealed class GlobalAirProvider : IFlightProvider
{
    private const string CurrencyCode = "USD";
    private const string FlightId = "globalair-ga123";

    public IReadOnlyList<string> KnownFlightIds => [FlightId];

    public Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(FlightSearchRequest request, CancellationToken cancellationToken)
    {
        var departure = request.DepartureDate.ToDateTime(new TimeOnly(8, 0), DateTimeKind.Utc);
        var arrival = request.DepartureDate.ToDateTime(new TimeOnly(11, 30), DateTimeKind.Utc);

        var baseFare = 100.00m;
        var perPassenger = CalculatePerPassengerPrice(baseFare);
        var total = perPassenger * request.Passengers;

        var offer = new FlightOffer(
            Id: FlightId,
            Provider: "GlobalAir",
            FlightNumber: "GA123",
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
        var final = baseFare + (baseFare * 0.15m);
        return decimal.Round(final, 2, MidpointRounding.AwayFromZero);
    }
}

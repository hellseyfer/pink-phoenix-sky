namespace FlightAggregator.Modules.Flights.Domain;

public sealed record FlightOffer(
    string Id,
    string Provider,
    string FlightNumber,
    string Origin,
    string Destination,
    DateTimeOffset DepartureTime,
    DateTimeOffset ArrivalTime,
    int DurationMinutes,
    CabinClass CabinClass,
    decimal PricePerPassenger,
    decimal TotalPrice,
    string Currency
);

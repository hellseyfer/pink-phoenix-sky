using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Features.SearchFlights;
// Per use-case. Feature contracts are request/response shapes for one specific use case and transport boundary. UI concerns
public sealed record SearchFlightsRequest(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    int Passengers,
    CabinClass CabinClass
);

// sealed: cannot be inherited nor extended, record: immutable data
public sealed record FlightOfferResponse(
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

using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Features.SearchFlights;

public sealed record SearchFlightsRequest(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    int Passengers,
    CabinClass CabinClass
);

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

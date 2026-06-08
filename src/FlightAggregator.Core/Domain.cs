namespace FlightAggregator.Core;

public enum CabinClass
{
    Economy,
    Business,
    First
}

public sealed record FlightSearchRequest(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    int Passengers,
    CabinClass CabinClass
);

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

public sealed record Passenger(
    string FullName,
    string Email,
    string DocumentNumber
);

public sealed record CreateBookingRequest(
    string FlightId,
    IReadOnlyList<Passenger> Passengers
);

public sealed record CreateBookingResponse(string BookingReference);

namespace FlightAggregator.Modules.Flights.Domain;

public sealed record FlightSearchRequest(
    string Origin,
    string Destination,
    DateOnly DepartureDate,
    int Passengers,
    CabinClass CabinClass
);

namespace FlightAggregator.Modules.Bookings.Domain;

public sealed record BookingPassenger(
    string FullName,
    string Email,
    string DocumentNumber
);

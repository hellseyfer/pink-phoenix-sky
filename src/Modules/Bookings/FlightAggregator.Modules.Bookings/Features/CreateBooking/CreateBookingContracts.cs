using FlightAggregator.Modules.Bookings.Domain;

namespace FlightAggregator.Modules.Bookings.Features.CreateBooking;

public sealed record CreateBookingRequest(
    string FlightId,
    IReadOnlyList<BookingPassenger> Passengers
);

public sealed record CreateBookingResponse(string BookingReference);

using System.Security.Cryptography;
using FlightAggregator.Modules.Flights.PublicApi;

namespace FlightAggregator.Modules.Bookings.Features.CreateBooking;

public sealed class CreateBookingHandler(IFlightsModuleApi flightsModuleApi)
{
    public async Task<CreateBookingResult> HandleAsync(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var flightExists = await flightsModuleApi.FlightExistsAsync(request.FlightId, cancellationToken);
        if (!flightExists)
        {
            return CreateBookingResult.FlightNotFound(request.FlightId);
        }

        var bytes = RandomNumberGenerator.GetBytes(3);
        var code = Convert.ToHexString(bytes);
        return CreateBookingResult.Success(new CreateBookingResponse($"BK-{code}"));
    }
}

public sealed record CreateBookingResult(bool IsSuccess, CreateBookingResponse? Response, string? Error)
{
    public static CreateBookingResult Success(CreateBookingResponse response) => new(true, response, null);

    public static CreateBookingResult FlightNotFound(string flightId) =>
        new(false, null, $"Flight '{flightId}' was not found");
}

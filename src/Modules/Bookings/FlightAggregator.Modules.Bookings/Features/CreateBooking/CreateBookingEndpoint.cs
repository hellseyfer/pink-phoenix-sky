using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FlightAggregator.Modules.Bookings.Features.CreateBooking;

public static class CreateBookingEndpoint
{
    public static void MapCreateBooking(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/bookings", async (
            CreateBookingRequest request,
            CreateBookingHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.FlightId))
            {
                return Results.BadRequest("flightId is required");
            }

            if (request.Passengers is null || request.Passengers.Count == 0)
            {
                return Results.BadRequest("passengers must be a non-empty array");
            }

            var result = await handler.HandleAsync(request, cancellationToken);
            return result.IsSuccess
                ? Results.Ok(result.Response)
                : Results.BadRequest(result.Error);
        });
    }
}

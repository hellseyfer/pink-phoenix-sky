using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FlightAggregator.Modules.Flights.Features.SearchFlights;

public static class SearchFlightsEndpoint
{
    public static void MapSearchFlights(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/flights/search", async (
            SearchFlightsRequest request,
            SearchFlightsHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Origin) || request.Origin.Length != 3)
            {
                return Results.BadRequest("origin must be a 3-letter IATA code");
            }

            if (string.IsNullOrWhiteSpace(request.Destination) || request.Destination.Length != 3)
            {
                return Results.BadRequest("destination must be a 3-letter IATA code");
            }

            if (request.Passengers <= 0)
            {
                return Results.BadRequest("passengers must be >= 1");
            }

            var offers = await handler.HandleAsync(request, cancellationToken);
            return Results.Ok(offers);
        });
    }
}

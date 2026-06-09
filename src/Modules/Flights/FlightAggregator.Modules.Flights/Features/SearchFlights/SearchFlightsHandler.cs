using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Features.SearchFlights;

public sealed class SearchFlightsHandler(IEnumerable<IFlightProvider> providers)
{
    private readonly IReadOnlyList<IFlightProvider> _providers = providers.ToArray();

    public async Task<IReadOnlyList<FlightOfferResponse>> HandleAsync(SearchFlightsRequest request, CancellationToken cancellationToken)
    {
        // map the request into a common domain model
        var domainRequest = new FlightSearchRequest(
            request.Origin,
            request.Destination,
            request.DepartureDate,
            request.Passengers,
            request.CabinClass);

        // materialize the task array using ToArray()
        var tasks = _providers.Select(p => p.SearchFlightsAsync(domainRequest, cancellationToken)).ToArray();
        // run the tasks in parallel, the duration of this request is limited by the slowest provider
        var results = await Task.WhenAll(tasks);

        // flatten the results and map to the response, materialize the array with ToArray()
        return results
            .SelectMany(offers => offers)
            .Select(MapToResponse)
            .ToArray();
    }

    // map to DTO response
    private static FlightOfferResponse MapToResponse(FlightOffer offer) => new(
        offer.Id,
        offer.Provider,
        offer.FlightNumber,
        offer.Origin,
        offer.Destination,
        offer.DepartureTime,
        offer.ArrivalTime,
        offer.DurationMinutes,
        offer.CabinClass,
        offer.PricePerPassenger,
        offer.TotalPrice,
        offer.Currency);
}

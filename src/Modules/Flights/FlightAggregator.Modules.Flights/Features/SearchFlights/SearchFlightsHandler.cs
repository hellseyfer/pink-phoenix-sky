using FlightAggregator.Modules.Flights.Domain;

namespace FlightAggregator.Modules.Flights.Features.SearchFlights;

public sealed class SearchFlightsHandler(IEnumerable<IFlightProvider> providers)
{
    private readonly IReadOnlyList<IFlightProvider> _providers = providers.ToArray();

    public async Task<IReadOnlyList<FlightOfferResponse>> HandleAsync(SearchFlightsRequest request, CancellationToken cancellationToken)
    {
        var domainRequest = new FlightSearchRequest(
            request.Origin,
            request.Destination,
            request.DepartureDate,
            request.Passengers,
            request.CabinClass);

        var tasks = _providers.Select(p => p.SearchFlightsAsync(domainRequest, cancellationToken)).ToArray();
        var results = await Task.WhenAll(tasks);

        return results
            .SelectMany(offers => offers)
            .Select(MapToResponse)
            .ToArray();
    }

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

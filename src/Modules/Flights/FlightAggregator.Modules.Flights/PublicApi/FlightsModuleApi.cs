using FlightAggregator.Modules.Flights.Domain;
using FlightAggregator.Modules.Flights.PublicApi;

namespace FlightAggregator.Modules.Flights.Infrastructure;

internal sealed class FlightsModuleApi(IEnumerable<IFlightProvider> providers) : IFlightsModuleApi
{
    private readonly IReadOnlyList<IFlightProvider> _providers = providers.ToArray();

    public Task<bool> FlightExistsAsync(string flightId, CancellationToken cancellationToken = default)
    {
        var exists = _providers.Any(p => p.KnownFlightIds.Contains(flightId));
        return Task.FromResult(exists);
    }
}

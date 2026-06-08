namespace FlightAggregator.Modules.Flights.Domain;

public interface IFlightProvider
{
    IReadOnlyList<string> KnownFlightIds { get; }

    Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(FlightSearchRequest request, CancellationToken cancellationToken);
}

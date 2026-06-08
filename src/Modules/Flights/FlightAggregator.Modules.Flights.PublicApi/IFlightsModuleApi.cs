namespace FlightAggregator.Modules.Flights.PublicApi;

public interface IFlightsModuleApi
{
    Task<bool> FlightExistsAsync(string flightId, CancellationToken cancellationToken = default);
}

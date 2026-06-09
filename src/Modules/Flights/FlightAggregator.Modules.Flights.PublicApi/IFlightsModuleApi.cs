namespace FlightAggregator.Modules.Flights.PublicApi;

// Public contract exposed by the Flights module for other modules.
public interface IFlightsModuleApi
{
    Task<bool> FlightExistsAsync(string flightId, CancellationToken cancellationToken = default);
}

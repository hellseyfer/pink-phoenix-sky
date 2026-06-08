namespace FlightAggregator.Core;

public interface IFlightProvider
{
    Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(FlightSearchRequest request, CancellationToken cancellationToken);
}

public interface IFlightSearchService
{
    Task<IReadOnlyList<FlightOffer>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken);
}

public interface IBookingService
{
    Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken);
}

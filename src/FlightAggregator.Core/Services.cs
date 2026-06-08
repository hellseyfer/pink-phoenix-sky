using System.Security.Cryptography;

namespace FlightAggregator.Core;

public sealed class FlightSearchService(IEnumerable<IFlightProvider> providers) : IFlightSearchService
{
    private readonly IReadOnlyList<IFlightProvider> _providers = providers.ToArray();

    public async Task<IReadOnlyList<FlightOffer>> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken)
    {
        var tasks = _providers.Select(p => p.SearchFlightsAsync(request, cancellationToken)).ToArray();
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(x => x).ToArray();
    }
}

public sealed class BookingService : IBookingService
{
    public Task<CreateBookingResponse> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var bytes = RandomNumberGenerator.GetBytes(3);
        var code = Convert.ToHexString(bytes);
        return Task.FromResult(new CreateBookingResponse($"BK-{code}"));
    }
}

using FlightAggregator.Modules.Flights.Domain;
using FlightAggregator.Modules.Flights.Features.SearchFlights;
using Xunit;

namespace FlightAggregator.Tests;

public sealed class AggregationTests
{
    [Fact]
    public async Task Search_AggregatesProviders_AndDoesNotUseProviderSpecificLogic()
    {
        var providers = new IFlightProvider[]
        {
            new FakeProvider("P1", perPassenger: 50m),
            new FakeProvider("P2", perPassenger: 60m)
        };

        var handler = new SearchFlightsHandler(providers);

        var request = new SearchFlightsRequest(
            Origin: "JFK",
            Destination: "LAX",
            DepartureDate: new DateOnly(2026, 06, 15),
            Passengers: 2,
            CabinClass: CabinClass.Economy
        );

        var offers = await handler.HandleAsync(request, CancellationToken.None);

        Assert.Equal(2, offers.Count);
        Assert.All(offers, o => Assert.Equal(o.PricePerPassenger * request.Passengers, o.TotalPrice));
    }

    private sealed class FakeProvider(string name, decimal perPassenger) : IFlightProvider
    {
        public IReadOnlyList<string> KnownFlightIds => [$"{name}-1"];

        public Task<IReadOnlyList<FlightOffer>> SearchFlightsAsync(FlightSearchRequest request, CancellationToken cancellationToken)
        {
            var departure = new DateTimeOffset(2026, 6, 15, 8, 0, 0, TimeSpan.Zero);
            var arrival = new DateTimeOffset(2026, 6, 15, 9, 0, 0, TimeSpan.Zero);

            var offer = new FlightOffer(
                Id: $"{name}-1",
                Provider: name,
                FlightNumber: "X1",
                Origin: request.Origin,
                Destination: request.Destination,
                DepartureTime: departure,
                ArrivalTime: arrival,
                DurationMinutes: 60,
                CabinClass: request.CabinClass,
                PricePerPassenger: perPassenger,
                TotalPrice: perPassenger * request.Passengers,
                Currency: "USD"
            );

            return Task.FromResult<IReadOnlyList<FlightOffer>>([offer]);
        }
    }
}

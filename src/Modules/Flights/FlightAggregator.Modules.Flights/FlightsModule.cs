using FlightAggregator.Modules.Flights.Domain;
using FlightAggregator.Modules.Flights.Features.SearchFlights;
using FlightAggregator.Modules.Flights.Infrastructure;
using FlightAggregator.Modules.Flights.PublicApi;
using FlightAggregator.Shared;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FlightAggregator.Modules.Flights;

public sealed class FlightsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<IFlightProvider, GlobalAirProvider>();
        services.AddSingleton<IFlightProvider, BudgetWingsProvider>();
        services.AddSingleton<IFlightProvider, ArticAirProvider>();
        services.AddSingleton<SearchFlightsHandler>();
        services.AddSingleton<IFlightsModuleApi, FlightsModuleApi>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapSearchFlights();
    }
}

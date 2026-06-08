using FlightAggregator.Modules.Bookings.Features.CreateBooking;
using FlightAggregator.Shared;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FlightAggregator.Modules.Bookings;

public sealed class BookingsModule : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<CreateBookingHandler>();
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCreateBooking();
    }
}

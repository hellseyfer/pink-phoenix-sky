using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FlightAggregator.Shared;

public interface IModule
{
    void RegisterServices(IServiceCollection services);

    void MapEndpoints(IEndpointRouteBuilder endpoints);
}

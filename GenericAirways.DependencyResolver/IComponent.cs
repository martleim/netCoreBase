using Microsoft.Extensions.DependencyInjection;

namespace GenericAirways.DependencyResolver
{
    public interface IComponent
    {
        void SetUp(IServiceCollection services);
    }
}
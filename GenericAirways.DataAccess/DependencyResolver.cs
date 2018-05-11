using GenericAirways.Contracts;
using GenericAirways.DependencyResolver;
using GenericAirways.Model;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition;
using Microsoft.Extensions.DependencyInjection;


namespace GenericAirways.DataAccess
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IServiceCollection services)
        {
            services.AddTransient<IPassengerRecordRepository, PassengerRecordRepository>();
            services.AddTransient<IPNLFileRepository, PNLFileRepository>();
            services.AddTransient<IRecordLocatorRepository, RecordLocatorRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
        }
    }
}
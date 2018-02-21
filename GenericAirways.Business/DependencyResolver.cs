using GenericAirways.Contracts;
using GenericAirways.DependencyResolver;
using GenericAirways.Model;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition;
using Microsoft.Extensions.DependencyInjection;


namespace GenericAirways.Business
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IServiceCollection services)
        {
            //services.AddTransient<IPNLProcessor<PNLFile, PassengerRecord, string>, PNLFileProcessor>();
            services.AddTransient<IPNLFileDataLogic, PNLFileDataLogic>();
        }
    }
}
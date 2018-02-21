using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Loader;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using System.Composition.Hosting;
using System.Composition;


namespace GenericAirways.DependencyResolver
{
    public class ComponentLoader
    {
        public static void LoadContainer(IServiceCollection services, string path, string pattern)
        {

            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+path, pattern )
                            .Select(  AssemblyLoadContext.Default.LoadFromAssemblyPath );
            var configuration = new ContainerConfiguration().WithAssemblies( assemblies );
            var container = configuration.CreateContainer();
            try
            {
                IEnumerable<object> exports = container.GetExports(typeof( IComponent ));
                foreach (IComponent module in exports)
                {
                    module.SetUp(services);
                }
               

            }
            catch (ReflectionTypeLoadException typeLoadException)
            {
                var builder = new StringBuilder();
                foreach (Exception loaderException in typeLoadException.LoaderExceptions)
                {
                    builder.AppendFormat("{0}\n", loaderException.Message);
                }

                throw new TypeLoadException(builder.ToString(), typeLoadException);
            }
        }

    }

}
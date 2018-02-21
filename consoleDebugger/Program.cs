using System;
using GenericAirways.DependencyResolver;
using Microsoft.Extensions.DependencyInjection;

namespace consoleDebugger
{
    class Program
    {
        static void Main(string[] args)
        {
            string command = "";
            while (command !="exit")
            {
                command = Console.ReadLine();
                if(command=="test"){
                    ComponentLoader.LoadContainer(null, "", "*.dll");
                }
            }
        }
    }
}

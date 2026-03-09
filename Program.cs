using System;
using System.Threading.Tasks;
using UtilitiesManager;

namespace UtilitiesManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                await CliUtilMan.RunCliMode(args);
            }
            else
            {
                await CliUtilMan.RunInteractiveMode();
            }
        }
    }
}

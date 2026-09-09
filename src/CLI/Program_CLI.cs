using System;
using System.Linq;
using System.Threading.Tasks;

namespace UtilitiesManagerCLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                await UtilitiesManager.CliUtilMan.RunCliMode(args);
            }
            else
            {
                // Run CLI interactive mode
                await UtilitiesManager.CliUtilMan.RunInteractiveMode();
            }
        }
    }
}

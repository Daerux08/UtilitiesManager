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
                await UtilitiesManagerCLI.CliUtilMan.RunCliMode(args);
            }
            else
            {
                // Run CLI interactive mode
                await UtilitiesManagerCLI.CliUtilMan.RunInteractiveMode();
            }
        }
    }
}

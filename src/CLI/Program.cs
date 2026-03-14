using System;
using System.Linq;
using System.Threading.Tasks;
using UtilitiesManagerCLI;

namespace UtilitiesManagerCLI
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
                // Run CLI interactive mode
                await CliUtilMan.RunInteractiveMode();
            }
        }
    }
}

using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class ServicesService
    {

        public static async Task MenuService(CheckDependencyCommand checker)
        {
            Console.WriteLine("DEBUG: Starting Service Management Menu");
            var checker = new CheckDependencyCommand();
            await checker.CheckDependenciesAsync();
            Console.WriteLine($"DEBUG: IsSystemctlAvailable = {CheckDependencyCommand.IsSystemctlAvailable}");

            if (!CheckDependencyCommand.IsSystemctlAvailable)
            {
                MenuEngine.ErrorMessage("systemctl is not available on this system.");
                return;
            }

            var menuOptions = new List<(string, Func<Task>)>
            {
                ("📋 List all services", async () => { await ServicesService.HandleServicesCommand(new string[] { "services" }); MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🔍 Check specific service status", async () => { var serviceName = MenuEngine.TextInput("Enter service name"); if (!string.IsNullOrEmpty(serviceName)) { await ServicesService.HandleServicesCommand(new string[] { "services", "status", serviceName }); } MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("▶️ Start a service", async () => { var startService = MenuEngine.TextInput("Enter service name to start"); if (!string.IsNullOrEmpty(startService)) { await ServicesService.HandleServicesCommand(new string[] { "services", "start", startService }); } MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("⏹️ Stop a service", async () => { var stopService = MenuEngine.TextInput("Enter service name to stop"); if (!string.IsNullOrEmpty(stopService)) { await ServicesService.HandleServicesCommand(new string[] { "services", "stop", stopService }); } MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("🔄 Restart a service", async () => { var restartService = MenuEngine.TextInput("Enter service name to restart"); if (!string.IsNullOrEmpty(restartService)) { await ServicesService.HandleServicesCommand(new string[] { "services", "restart", restartService }); } MenuEngine.GeneralMessage("Press any key to continue..."); Console.ReadKey(true); }),
                ("ℹ️ What are services?", async () => { MenuEngine.GeneralMessage("Services are background programs that run on your system. Use systemctl to manage them."); await Task.Delay(1); }),
                ("⬅ Back to main menu", async () => { throw new GoBackException(); })
            };

            await MenuEngine.DisplayMenuAsync(menuOptions);
        }



        public static async Task HandleServicesCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            if (!CheckDependencyCommand.IsSystemctlAvailable)
            {
                return;
            }

            if (args.Length > 1)
            {
                var action = args[1].ToLower();
                var serviceName = args.Length > 2 ? args[2] : "";

                if (!string.IsNullOrEmpty(serviceName))
                {
                    switch (action)
                    {
                        case "start":
                            await TerminalCommands.RunCommandAsync($"sudo systemctl start {serviceName}");
                            Console.WriteLine($"Starting service: {serviceName}");
                            break;
                        case "stop":
                            await TerminalCommands.RunCommandAsync($"sudo systemctl stop {serviceName}");
                            Console.WriteLine($"Stopping service: {serviceName}");
                            break;
                        case "restart":
                            await TerminalCommands.RunCommandAsync($"sudo systemctl restart {serviceName}");
                            Console.WriteLine($"Restarting service: {serviceName}");
                            break;
                        case "status":
                            var status = await TerminalCommands.RunCommandAsync($"systemctl status {serviceName}");
                            Console.WriteLine($"Service status for {serviceName}:");
                            Console.WriteLine(status);
                            break;
                        default:
                            Console.WriteLine("Usage: UtilMan services start|stop|restart|status <service_name>");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Usage: UtilMan services start|stop|restart|status <service_name>");
                }
            }
            else
            {
                var services = await checker.GetServicesAsync();
                Console.WriteLine("=== SERVICES ===");
                Console.WriteLine("{0,-25} {1,-8} {2,-8} {3,-8}", "Service", "Load", "Active", "Sub");
                Console.WriteLine(new string('-', 60));

                foreach (var service in services.Take(15))
                {
                    Console.WriteLine("{0,-25} {1,-8} {2,-8} {3,-8}",
                        service.Name, service.Load, service.Active, service.Sub);
                }
            }
        }
    }
}


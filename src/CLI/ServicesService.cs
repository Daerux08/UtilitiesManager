using System;
using UtilitiesManager;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class ServicesService
    {

        public static async Task ServiceManagementMenu()
        {
            Console.WriteLine("DEBUG: Starting Service Management Menu");
            var checker = new CheckDependencyCommand();
            await checker.CheckDependenciesAsync();
            Console.WriteLine($"DEBUG: IsSystemctlAvailable = {checker.IsSystemctlAvailable}");

            if (!checker.IsSystemctlAvailable)
            {
                MenuHelper.ShowError("Service Management", "systemctl is not available on this system.");
                return;
            }

            while (true)
            {
                var menuOptions = new List<string>
                {
                    "📋 List all services",
                    "🔍 Check specific service status",
                    "▶️ Start a service",
                    "⏹️ Stop a service",
                    "🔄 Restart a service",
                    "ℹ️ What are services?",
                    "⬅ Back to main menu"
                };

                var choice = MenuHelper.ShowArrowMenu("SERVICE MANAGEMENT", menuOptions);

                switch (choice)
                {
                    case 0:
                        await ServicesService.HandleServicesCommand(new string[] { "services" });
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 1:
                        var serviceName = MenuHelper.GetUserInput("Enter service name");
                        if (!string.IsNullOrEmpty(serviceName))
                        {
                            await ServicesService.HandleServicesCommand(new string[] { "services", "status", serviceName });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 2:
                        var startService = MenuHelper.GetUserInput("Enter service name to start");
                        if (!string.IsNullOrEmpty(startService))
                        {
                            await ServicesService.HandleServicesCommand(new string[] { "services", "start", startService });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 3:
                        var stopService = MenuHelper.GetUserInput("Enter service name to stop");
                        if (!string.IsNullOrEmpty(stopService))
                        {
                            await ServicesService.HandleServicesCommand(new string[] { "services", "stop", stopService });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 4:
                        var restartService = MenuHelper.GetUserInput("Enter service name to restart");
                        if (!string.IsNullOrEmpty(restartService))
                        {
                            await ServicesService.HandleServicesCommand(new string[] { "services", "restart", restartService });
                        }
                        Console.WriteLine();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    case 5:
                        var helpText = @"=== WHAT ARE SYSTEMD SERVICES? ===

                            Services (systemd services) are background programs that run on your Linux system.
                            They manage core system functionality and applications.

                            COMMON SERVICES:
                            • sshd - Secure Shell server for remote access
                            • nginx - Web server
                            • docker - Container management
                            • ufw - Firewall management
                            • NetworkManager - Network connections
                            • cron - Scheduled tasks

                            SERVICE STATES:
                            • active (running) - Service is currently running
                            • inactive (dead) - Service is stopped
                            • enabled - Service starts automatically on boot
                            • disabled - Service must be started manually

                            WHY MANAGE SERVICES?
                            • Fix problems by restarting problematic services
                            • Improve security by stopping unused services  
                            • Save resources by disabling unnecessary services
                            • Debug system issues by checking service status

                            TIPS:
                            • Be careful when stopping system-critical services
                            • Use 'status' first before making changes
                            • Some services require sudo privileges to control";

                        MenuHelper.ShowMessage("About Services", helpText);
                        break;
                    case 6:
                        return;
                    case -1:
                        return;
                }
            }
        }



        public static async Task HandleServicesCommand(string[] args)
        {
            var checker = new CheckDependencyCommand();
            if (!checker.IsSystemctlAvailable)
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


using System;
using System.Threading.Tasks;
using UtilitiesManager;

namespace UtilitiesManager
{
    public static class DownloadScript
    {
        private static readonly (string Package, string Description)[] RequiredPackages = {
            // Core system utilities
            ("brightnessctl", "Screen brightness control for laptops and monitors"),
            ("pactl", "PulseAudio command-line sound server control"), 
            ("upower", "Power management and battery information"),
            ("nmcli", "NetworkManager command-line interface for WiFi/network control"),
            ("powerprofilesctl", "Power profile management for performance modes"),
            
            // System monitoring packages
            ("procps", "Process utilities (ps, free, top, who - usually pre-installed)"),
            ("lm-sensors", "Hardware sensor monitoring (temperature, fan speed)"),
            ("sysstat", "System performance monitoring (iostat, mpstat, sar)"),
            ("iotop", "I/O monitoring for disk usage by process"),
            ("nethogs", "Network monitoring showing bandwidth by process"),
            
            // Security and maintenance
            ("ufw", "Uncomplicated Firewall - easy iptables frontend"),
            ("fail2ban", "Intrusion prevention that bans suspicious IPs"),
            ("bleachbit", "System cleaner - removes temporary files and frees space"),
            ("ncdu", "NCurses Disk Usage - interactive disk space analyzer"),
            
            // Additional useful utilities
            ("htop", "Interactive process viewer (better than top)"),
            ("tree", "Directory tree viewer"),
            ("jq", "JSON command-line processor"),
            ("curl", "Data transfer utility with multiple protocols"),
            ("wget", "File download utility"),
            ("vim", "Improved vi text editor (or nano)"),
            ("git", "Version control system"),
            ("unzip", "ZIP archive extractor"),
            ("tar", "Archive utility for .tar files"),
            ("net-tools", "Network tools (ping, netstat, arp, traceroute)"),
            ("dnsutils", "DNS tools (nslookup, dig)"),
            ("man-db", "Manual pages database"),
            ("sudo", "Superuser do command - should be pre-installed")
        };

        private static readonly string[] PackageCommands = {
            "apt update",
            "apt install -y brightnessctl pactl upower nmcli powerprofilesctl procps lm-sensors sysstat iotop nethogs ufw fail2ban bleachbit ncdu htop tree jq curl wget vim git unzip tar net-tools dnsutils man-db sudo"
        };

        public static async Task<bool> RunPackageInstallationAsync()
        {
            Console.WriteLine("=== PACKAGE INSTALLATION SCRIPT ===");
            Console.WriteLine("This script will install all required packages for Utilities Manager.");
            Console.WriteLine();
            Console.WriteLine("Packages to be installed:");
            foreach (var (package, description) in RequiredPackages)
            {
                Console.WriteLine($"  - {package} - {description}");
            }
            Console.WriteLine();

            Console.Write("Do you want to continue? (y/N): ");
            var response = Console.ReadLine()?.ToLower().Trim();
            
            if (response != "y" && response != "yes")
            {
                Console.WriteLine("Installation cancelled.");
                return false;
            }

            Console.WriteLine();
            Console.WriteLine("Starting package installation...");
            Console.WriteLine();

            try
            {
                // Check if running as root
                if (!await IsRunningAsRoot())
                {
                    Console.WriteLine("This operation requires root privileges.");
                    Console.WriteLine("Attempting to use sudo...");
                    
                    // Try with sudo
                    return await ExecuteWithSudo();
                }
                else
                {
                    // Running as root, execute directly
                    return await ExecuteInstallationCommands();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during package installation: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> IsRunningAsRoot()
        {
            try
            {
                var result = await TerminalCommands.RunCommandWithResultAsync("id -u");
                return result.Output.Trim() == "0";
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> ExecuteWithSudo()
        {
            Console.WriteLine("Please enter your sudo password to continue...");
            
            foreach (var command in PackageCommands)
            {
                try
                {
                    Console.WriteLine($"Executing: {command}");
                    
                    // Use sudo for the command
                    var result = await TerminalCommands.RunCommandWithResultAsync($"sudo {command}");
                    
                    if (result.ExitCode != 0)
                    {
                        Console.WriteLine($"Command failed: {command}");
                        if (!string.IsNullOrEmpty(result.Error))
                        {
                            Console.WriteLine($"Error: {result.Error}");
                        }
                        
                        // Check if it's a password error
                        if (result.Error?.ToLower().Contains("password") == true || 
                            result.Error?.ToLower().Contains("authentication") == true)
                        {
                            Console.WriteLine("Authentication failed. Please check your password.");
                            return false;
                        }
                        
                        // Ask if user wants to continue
                        Console.Write("Continue with remaining commands? (y/N): ");
                        var continueResponse = Console.ReadLine()?.ToLower().Trim();
                        if (continueResponse != "y" && continueResponse != "yes")
                        {
                            Console.WriteLine("Installation cancelled by user.");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("✓ Command completed successfully");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception executing command '{command}': {ex.Message}");
                    
                    Console.Write("Continue with remaining commands? (y/N): ");
                    var continueResponse = Console.ReadLine()?.ToLower().Trim();
                    if (continueResponse != "y" && continueResponse != "yes")
                    {
                        Console.WriteLine("Installation cancelled by user.");
                        return false;
                    }
                }
                
                Console.WriteLine();
            }

            Console.WriteLine("Package installation completed!");
            return true;
        }

        private static async Task<bool> ExecuteInstallationCommands()
        {
            foreach (var command in PackageCommands)
            {
                try
                {
                    Console.WriteLine($"Executing: {command}");
                    
                    var result = await TerminalCommands.RunCommandWithResultAsync(command);
                    
                    if (result.ExitCode != 0)
                    {
                        Console.WriteLine($"Command failed: {command}");
                        if (!string.IsNullOrEmpty(result.Error))
                        {
                            Console.WriteLine($"Error: {result.Error}");
                        }
                        
                        Console.Write("Continue with remaining commands? (y/N): ");
                        var continueResponse = Console.ReadLine()?.ToLower().Trim();
                        if (continueResponse != "y" && continueResponse != "yes")
                        {
                            Console.WriteLine("Installation cancelled by user.");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("✓ Command completed successfully");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception executing command '{command}': {ex.Message}");
                    
                    Console.Write("Continue with remaining commands? (y/N): ");
                    var continueResponse = Console.ReadLine()?.ToLower().Trim();
                    if (continueResponse != "y" && continueResponse != "yes")
                    {
                        Console.WriteLine("Installation cancelled by user.");
                        return false;
                    }
                }
                
                Console.WriteLine();
            }

            Console.WriteLine("Package installation completed!");
            return true;
        }

        public static async Task<bool> CheckAndInstallPackageAsync(string packageName)
        {
            try
            {
                // Check if package is already installed
                var checkResult = await TerminalCommands.RunCommandWithResultAsync($"dpkg -l | grep -w {packageName}");
                
                if (checkResult.ExitCode == 0 && !string.IsNullOrEmpty(checkResult.Output))
                {
                    Console.WriteLine($"✓ {packageName} is already installed");
                    return true;
                }

                Console.WriteLine($"Installing {packageName}...");
                
                var installCommand = await IsRunningAsRoot() 
                    ? $"apt install -y {packageName}"
                    : $"sudo apt install -y {packageName}";
                
                var result = await TerminalCommands.RunCommandWithResultAsync(installCommand);
                
                if (result.ExitCode == 0)
                {
                    Console.WriteLine($"✓ {packageName} installed successfully");
                    return true;
                }
                else
                {
                    Console.WriteLine($"✗ Failed to install {packageName}");
                    if (!string.IsNullOrEmpty(result.Error))
                    {
                        Console.WriteLine($"Error: {result.Error}");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error installing {packageName}: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> InstallIndividualPackagesAsync()
        {
            Console.WriteLine("=== INDIVIDUAL PACKAGE INSTALLATION ===");
            Console.WriteLine("This allows you to install packages one by one.");
            Console.WriteLine();

            var successCount = 0;
            var totalCount = RequiredPackages.Length;

            foreach (var packageTuple in RequiredPackages)
            {
                var package = packageTuple.Package;
                var description = packageTuple.Description;
                Console.WriteLine($"Processing {package} - {description}...");
                if (await CheckAndInstallPackageAsync(package))
                {
                    successCount++;
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Installation summary: {successCount}/{totalCount} packages installed successfully");
            return successCount == totalCount;
        }

        public static async Task ShowPackageStatusAsync()
        {
            Console.WriteLine("=== PACKAGE STATUS ===");
            Console.WriteLine("{0,-20} {1,-10}", "Package", "Status");
            Console.WriteLine(new string('-', 30));

            foreach (var packageTuple in RequiredPackages)
            {
                var package = packageTuple.Package;
                var description = packageTuple.Description;
                try
                {
                    var result = await TerminalCommands.RunCommandWithResultAsync($"dpkg -l | grep -w {package}");
                    var status = (result.ExitCode == 0 && !string.IsNullOrEmpty(result.Output)) ? "Installed" : "Not found";
                    Console.WriteLine("{0,-20} {1,-10} {2,-50}", package, status, description);
                }
                catch
                {
                    Console.WriteLine("{0,-20} {1,-10} {2,-50}", package, "Error", description);
                }
            }
        }

        public static async Task<bool> SetupSensorsAsync()
        {
            Console.WriteLine("=== SENSOR SETUP ===");
            Console.WriteLine("Setting up hardware sensors...");
            
            try
            {
                if (!await IsRunningAsRoot())
                {
                    var command = "sudo sensors-detect --auto";
                    Console.WriteLine($"Running: {command}");
                    
                    var result = await TerminalCommands.RunCommandWithResultAsync(command);
                    
                    if (result.ExitCode == 0)
                    {
                        Console.WriteLine("✓ Sensor detection completed successfully");
                        
                        // Load the sensors module
                        Console.WriteLine("Loading kernel modules...");
                        await TerminalCommands.RunCommandAsync("sudo /etc/init.d/kmod start");
                        
                        Console.WriteLine("✓ Sensor setup completed");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("✗ Sensor detection failed");
                        if (!string.IsNullOrEmpty(result.Error))
                        {
                            Console.WriteLine($"Error: {result.Error}");
                        }
                        return false;
                    }
                }
                else
                {
                    // Running as root
                    var result = await TerminalCommands.RunCommandWithResultAsync("sensors-detect --auto");
                    
                    if (result.ExitCode == 0)
                    {
                        Console.WriteLine("✓ Sensor detection completed successfully");
                        await TerminalCommands.RunCommandAsync("/etc/init.d/kmod start");
                        Console.WriteLine("✓ Sensor setup completed");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("✗ Sensor detection failed");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during sensor setup: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> ConfigureFirewallAsync()
        {
            Console.WriteLine("=== FIREWALL CONFIGURATION ===");
            Console.WriteLine("Configuring UFW firewall...");
            
            try
            {
                var commands = new[]
                {
                    "ufw --force enable",
                    "ufw default deny incoming",
                    "ufw default allow outgoing",
                    "ufw allow ssh",
                    "ufw allow 22/tcp"
                };

                foreach (var command in commands)
                {
                    var fullCommand = await IsRunningAsRoot() ? command : $"sudo {command}";
                    
                    Console.WriteLine($"Executing: {fullCommand}");
                    var result = await TerminalCommands.RunCommandWithResultAsync(fullCommand);
                    
                    if (result.ExitCode != 0)
                    {
                        Console.WriteLine($"✗ Command failed: {command}");
                        if (!string.IsNullOrEmpty(result.Error))
                        {
                            Console.WriteLine($"Error: {result.Error}");
                        }
                        return false;
                    }
                    else
                    {
                        Console.WriteLine($"✓ Command completed: {command}");
                    }
                }

                Console.WriteLine("✓ Firewall configuration completed");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during firewall configuration: {ex.Message}");
                return false;
            }
        }
    }
}

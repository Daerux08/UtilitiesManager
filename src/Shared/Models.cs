using System;
using System.Collections.Generic;

namespace UtilitiesManager
{
    public class BatteryInfo
    {
        public string State { get; set; } = "Unknown";
        public int Percentage { get; set; } = -1;
        public string TimeToEmpty { get; set; } = "N/A";
        public string TimeToFull { get; set; } = "N/A";
        public double EnergyRate { get; set; } = -1;
        public bool IsPresent { get; set; } = false;
    }

    public class WiFiInfo
    {
        public string SSID { get; set; } = "";
        public string BSSID { get; set; } = "";
        public string Mode { get; set; } = "";
        public string Chan { get; set; } = "";
        public string Rate { get; set; } = "";
        public string Signal { get; set; } = "";
        public string Security { get; set; } = "";
        public bool IsActive { get; set; } = false;
    }

    public class BluetoothInfo
    {
        public string Address { get; set; } = "";
        public string Name { get; set; } = "";
        public string Alias { get; set; } = "";
        public string Type { get; set; } = "";
        public string RSSI { get; set; } = "";
        public bool Paired { get; set; } = false;
        public bool Connected { get; set; } = false;
        public bool Trusted { get; set; } = false;
        public bool Available { get; set; } = false;
    }

    public class SystemInfo
    {
        public string Uptime { get; set; } = "";
        public string[] LoadAverage { get; set; } = new string[0];
        public Dictionary<string, string> MemoryInfo { get; set; } = new();
        public Dictionary<string, string> Temperatures { get; set; } = new();
        public List<DiskInfo> DiskUsage { get; set; } = new();
        public List<NetworkInterface> NetworkInterfaces { get; set; } = new();
    }

    public class ServiceInfo
    {
        public string Name { get; set; } = "";
        public string Load { get; set; } = "";
        public string Active { get; set; } = "";
        public string Sub { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class UserInfo
    {
        public string Username { get; set; } = "";
        public string UID { get; set; } = "";
        public string GID { get; set; } = "";
        public string Home { get; set; } = "";
        public string Shell { get; set; } = "";
        public bool IsLoggedIn { get; set; } = false;
    }

    public class LogEntry
    {
        public string Timestamp { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public class FirewallStatus
    {
        public string UfwStatus { get; set; } = "";
        public List<string> IptablesRules { get; set; } = new();
        public string Fail2banStatus { get; set; } = "";
    }

    public class DiskInfo
    {
        public string Filesystem { get; set; } = "";
        public string Size { get; set; } = "";
        public string Used { get; set; } = "";
        public string Available { get; set; } = "";
        public string UsePercent { get; set; } = "";
        public string MountPoint { get; set; } = "";
    }

    public class NetworkInterface
    {
        public string Interface { get; set; } = "";
        public string IPAddress { get; set; } = "";
    }
}

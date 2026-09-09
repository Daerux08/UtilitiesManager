using System.Reflection;

namespace UtilitiesManager
{
    public static class AppVersion
    {
        public static string Current =>
            Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                .Split('+')[0]
            ?? "unknown";
    }
}

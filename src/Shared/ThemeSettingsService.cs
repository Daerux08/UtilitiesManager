using System;
using System.IO;
using System.Text.Json;
using UtilitiesManager.Models;

namespace UtilitiesManager.Services
{
    public class ThemeSettingsService
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "UtilitiesManager",
            "theme-settings.json"
        );

        private static ThemeSettings? _cachedSettings;

        public static ThemeSettings LoadSettings()
        {
            if (_cachedSettings != null)
                return _cachedSettings;

            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    _cachedSettings = JsonSerializer.Deserialize<ThemeSettings>(json);
                    return _cachedSettings ?? new ThemeSettings();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading theme settings: {ex.Message}");
            }

            _cachedSettings = new ThemeSettings();
            return _cachedSettings;
        }

        public static void SaveSettings(ThemeSettings settings)
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(SettingsFilePath, json);
                _cachedSettings = settings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving theme settings: {ex.Message}");
            }
        }

        public static void ResetSettings()
        {
            _cachedSettings = new ThemeSettings();
            SaveSettings(_cachedSettings);
        }
    }
}

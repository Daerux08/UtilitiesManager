using System.Text.Json.Serialization;

namespace UtilitiesManager.Models
{
    public class ThemeSettings
    {
        [JsonPropertyName("theme")]
        public string Theme { get; set; } = "Classic";

        [JsonPropertyName("variant")]
        public string Variant { get; set; } = "Light";
    }

    public enum ThemeType
    {
        Classic,
        Material,
        Aero,
        Bland
    }

    public enum ThemeVariant
    {
        Light,
        Dark
    }
}

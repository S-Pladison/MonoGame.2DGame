using System.Text.Json.Serialization;

namespace Pladi.Utilities.DataStructures
{
    public class ConfigData
    {
        [JsonPropertyName("Screen")]
        public ScreenConfig Screen { get; set; }

        // ...

        public ConfigData()
        {
            Screen = new ScreenConfig();
        }

        // ...

        public class ScreenConfig
        {
            [JsonPropertyName("Width")]
            public int Width { get; set; }

            [JsonPropertyName("Height")]
            public int Height { get; set; }

            [JsonPropertyName("Fullscreen")]
            public bool Fullscreen { get; set; }

            [JsonPropertyName("WindowMaximized")]
            public bool WindowMaximized { get; set; }
        }
    }
}
using System.Text.Json.Serialization;

namespace Pladi
{
    public class Config
    {
        [JsonPropertyName("Screen")]
        public ScreenConfig Screen { get; set; }

        // ...

        public Config()
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
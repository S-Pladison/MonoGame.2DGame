using System.Text.Json.Serialization;

namespace Pladi
{
    public class Settings
    {
        [JsonPropertyName("FullScreen")]
        public bool FullScreen { get; set; }
        [JsonPropertyName("ScreenWidth")]
        public int ScreenWidth { get; set; }

        [JsonPropertyName("ScreenHeight")]
        public int ScreenHeight { get; set; }
    }
}
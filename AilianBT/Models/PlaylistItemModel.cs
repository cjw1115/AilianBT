using System.Text.Json.Serialization;

namespace AilianBT.Models
{
    public class PlaylistItemModel
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}

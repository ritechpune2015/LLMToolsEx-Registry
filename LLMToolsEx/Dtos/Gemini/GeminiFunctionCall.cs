using System.Text.Json;
using System.Text.Json.Serialization;

namespace LLMToolsEx.Dtos.Gemini
{
    public class GeminiFunctionCall
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("args")]
        public JsonElement Args { get; set; }
    }
}

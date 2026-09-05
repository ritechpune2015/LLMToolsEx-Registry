using System.Text.Json.Serialization;

namespace LLMToolsEx.Dtos.Gemini
{
    public class GeminiFunctionResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("response")]
        public object Response { get; set; } = new();

    }
}

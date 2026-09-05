using System.Text.Json.Serialization;

namespace LLMToolsEx.Dtos.Gemini
{
    public class GeminiFunctionDeclaration
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("parameters")]
        public object Parameters { get; set; }
            = new();
    }
}

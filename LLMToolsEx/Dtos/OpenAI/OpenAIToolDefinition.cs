using System.Text.Json.Serialization;

namespace LLMToolsEx.Dtos.OpenAI
{
    public class OpenAIToolDefinition
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "function";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("parameters")]
        public object Parameters { get; set; } = new();
    }
}

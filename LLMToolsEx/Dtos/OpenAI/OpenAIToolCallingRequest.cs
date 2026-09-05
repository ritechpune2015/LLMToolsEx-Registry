using System.Text.Json.Serialization;

namespace LLMToolsEx.Dtos.OpenAI
{
    public class OpenAIToolCallingRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = "";
        [JsonPropertyName("input")]
        public string Input { get; set; } = "";
        [JsonPropertyName("tools")]
        //   public List<OpenAIToolDefinition> Tools { get; set; }= [];
        public List<object> Tools { get; set; } = [];

        [JsonPropertyName("tool_choice")]
        public string ToolChoice { get; set; } = "auto";

    }
}

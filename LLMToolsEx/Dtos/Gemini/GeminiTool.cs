using System.Text.Json.Serialization;

namespace LLMToolsEx.Dtos.Gemini
{
    public class GeminiTool
    {
        [JsonPropertyName("functionDeclarations")]
        public List<GeminiFunctionDeclaration>
    FunctionDeclarations { get; set; } = [];

    }
}

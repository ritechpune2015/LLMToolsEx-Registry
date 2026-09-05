namespace LLMToolsEx.Options
{
    public class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-5.4-mini";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1/";
        public double Temperature { get; set; } = 0.2;
        public int MaxOutputTokens { get; set; } = 1000;

    }
}

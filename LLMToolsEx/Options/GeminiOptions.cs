namespace LLMToolsEx.Options
{
    public class GeminiOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-3.6-flash";
        public string BaseUrl { get; set; }
            = "https://generativelanguage.googleapis.com/v1beta/";
        public double Temperature { get; set; } = 0.2;
        public int MaxOutputTokens { get; set; } = 4500;

    }
}

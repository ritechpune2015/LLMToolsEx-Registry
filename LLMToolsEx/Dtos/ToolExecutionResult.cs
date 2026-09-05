namespace LLMToolsEx.Dtos
{
    public class ToolExecutionResult
    {
        public ToolCall ToolCall { get; set; } = new();
        public object Result { get; set; }  = new();

    }
}

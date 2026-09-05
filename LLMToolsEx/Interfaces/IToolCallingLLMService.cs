namespace LLMToolsEx.Interfaces
{
    public interface IToolCallingLLMService
    {
        Task<string> GenerateWithToolsAsync(string prompt);

    }
}

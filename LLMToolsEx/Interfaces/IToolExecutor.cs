using System.Text.Json;

namespace LLMToolsEx.Interfaces
{
    public interface IToolExecutor
    {
        Task<object> ExecuteAsync(string toolName, JsonElement arguments);
    }
}

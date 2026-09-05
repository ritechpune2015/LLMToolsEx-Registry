using System.Text.Json;

namespace LLMToolsEx.Interfaces
{
    public interface ITool
    {
        string Name { get; }

        string Description { get; }

        object GetDefinition();

        Task<object> ExecuteAsync(JsonElement arguments);

    }
}

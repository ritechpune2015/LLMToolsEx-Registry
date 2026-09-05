using LLMToolsEx.Dtos;
using LLMToolsEx.Dtos.CustomerTools;
using LLMToolsEx.Interfaces;
using System.Text.Json;

namespace LLMToolsEx.Services
{
    public class ToolExecutor : IToolExecutor
    {

        private readonly IToolRegistry _registry;

        public ToolExecutor(IToolRegistry registry)
        {
            _registry = registry;
        }

        public async Task<object>  ExecuteAsync( string toolName,JsonElement arguments)
        {
            var tool =_registry.GetTool(toolName);

            if (tool == null)
            {
                throw new InvalidOperationException(
                    $"Tool '{toolName}' " +
                    $"is not registered.");
            }

            return await  tool.ExecuteAsync(arguments);
        }
    }

}
        



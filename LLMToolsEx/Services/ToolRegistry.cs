using LLMToolsEx.Interfaces;

namespace LLMToolsEx.Services
{
    public class ToolRegistry : IToolRegistry
    {
         private readonly Dictionary<string,ITool> _tools;

         public ToolRegistry(IEnumerable<ITool> tools)
          {
            _tools =
                tools.ToDictionary(
                    x => x.Name,
                    StringComparer.OrdinalIgnoreCase);
         }


        public ITool? GetTool(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            _tools.TryGetValue(name,out var tool);
            return tool;
        }


        public IReadOnlyCollection<ITool>     GetAllTools()
        {
            return _tools.Values
                .ToList()
                .AsReadOnly();
        }


        public List<object> GetToolDefinitions()
        {
            return _tools.Values
                .Select(
                    x => x.GetDefinition())
                .ToList();
        }
    }

}

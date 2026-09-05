namespace LLMToolsEx.Interfaces
{
    public interface IToolRegistry
    {
        ITool? GetTool(string name);
        IReadOnlyCollection<ITool>   GetAllTools();
        List<object>  GetToolDefinitions();

    }
}

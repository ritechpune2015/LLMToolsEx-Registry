using LLMToolsEx.Dtos.CustomerTools;
using LLMToolsEx.Interfaces;
using LLMToolsEx.Options;
using LLMToolsEx.Services;
using OllamaChatBoatWithoutRAG.Options;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
//builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
builder.Services.Configure<AIProviderOptions>(builder.Configuration.GetSection("AIProvider"));

builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));
builder.Services.AddScoped<IToolCallingLLMService, OpenAIService>();
//builder.Services.AddScoped<IToolCallingLLMService, GeminiService>();
builder.Services.AddScoped<IToolExecutor,ToolExecutor>();

builder.Services.AddScoped<ITool,CustomerTool>();
builder.Services.AddScoped<ITool, CustomerBalanceTool>();
builder.Services.AddScoped<ITool, CustomerOrdersTool>();
builder.Services.AddScoped<IToolRegistry,ToolRegistry>();

builder.Services.AddHttpClient<OpenAIService>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                "https://api.openai.com/v1/");
    });

builder.Services.AddHttpClient<GeminiService>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                "https://generativelanguage.googleapis.com/v1beta/");
    });

var app = builder.Build();
app.MapDefaultControllerRoute();
app.Run();

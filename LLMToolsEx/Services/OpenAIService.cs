using LLMToolsEx.Dtos;
using LLMToolsEx.Dtos.OpenAI;
using LLMToolsEx.Interfaces;
using LLMToolsEx.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LLMToolsEx.Services
{

    public class OpenAIService: IToolCallingLLMService
    {

        private readonly HttpClient _client;
        private readonly OpenAIOptions _options;
        private readonly IToolRegistry _toolRegistry;
        private readonly IToolExecutor _toolExecutor;

        public OpenAIService(HttpClient client,IOptions<OpenAIOptions> options,IToolRegistry toolRegistry,IToolExecutor toolExecutor)
        {
            _client = client;
            _options = options.Value;
            _toolRegistry=toolRegistry;
            _toolExecutor =toolExecutor;
            _client.BaseAddress =new Uri(_options.BaseUrl);
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _options.ApiKey);
        }

        //public async Task<string> GenerateWithToolsAsync(string prompt)
        //{
        //    // ==========================================
        //    // 1. Define available tools
        //    // ==========================================

        //    //var tools = new List<OpenAIToolDefinition>
        //    //  {
        //    //    BuildCustomerBalanceTool()
        //    //};

        //    var tools = BuildTools();

        //    // ==========================================
        //    // 2. Create request
        //    // ==========================================

        //    var request = new OpenAIToolCallingRequest
        //        {
        //            Model = _options.Model,
        //            Input = prompt,
        //            Tools = tools,
        //            ToolChoice = "auto"
        //        };


        //    // ==========================================
        //    // 3. Send first request
        //    // ==========================================

        //    var response = await _client.PostAsJsonAsync("responses",request);

        //    var responseJson = await response.Content.ReadAsStringAsync();


        //    // ==========================================
        //    // 4. Error handling
        //    // ==========================================

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        throw new HttpRequestException(
        //            $"OpenAI Tool Calling failed. " +
        //            $"StatusCode: {response.StatusCode}. " +
        //            $"Response: {responseJson}");
        //    }


        //    // ==========================================
        //    // 5. Extract function call
        //    // ==========================================

        //    //           var toolCall = ExtractToolCall(responseJson);

        //    var toolCalls = ExtractToolCalls(response);


        //    // ==========================================
        //    // 6. No tool required
        //    // ==========================================

        //    if (toolCall == null)
        //    {
        //        return ExtractOutputText(
        //            responseJson);
        //    }


        //    // ==========================================
        //    // 7. Parse arguments
        //    // ==========================================

        //    var arguments =JsonSerializer.Deserialize<JsonElement>(
        //            toolCall.Arguments);


        //    // ==========================================
        //    // 8. Execute tool
        //    // ==========================================

        //    var toolResult =await _toolExecutor.ExecuteAsync(
        //            toolCall.Name,
        //            arguments);

        //     // ==========================================
        //    // 9. Send tool result back to OpenAI
        //    // ==========================================

        //    return await ContinueAfterToolCallAsync(
        //            responseJson,
        //            toolCall,
        //            toolResult);
        //}


        public async Task<string> GenerateWithToolsAsync(string prompt)
        {
            // ==========================================
            // 1. Build all available tools
            // ==========================================

            var tools = _toolRegistry.GetToolDefinitions(); 


            // ==========================================
            // 2. Create request
            // ==========================================

            var request =
                new OpenAIToolCallingRequest
                {
                    Model =
                        _options.Model,

                    Input =
                        prompt,

                    Tools =
                        tools,

                    ToolChoice =
                        "auto"
                };


            // ==========================================
            // 3. First OpenAI call
            // ==========================================

            var response =
                await SendOpenAIRequestAsync(
                    request);


            // ==========================================
            // 4. Extract ALL tool calls
            // ==========================================

            var toolCalls =
                ExtractToolCalls(
                    response);


            // ==========================================
            // 5. No tools required
            // ==========================================

            if (toolCalls.Count == 0)
            {
                return ExtractOutputText(
                    response);
            }


            // ==========================================
            // 6. Execute ALL tools
            // ==========================================

            //var toolResults =
            //    await ExecuteToolCallsAsync(
            //        toolCalls);

            var toolResults = await ExecuteToolCallsInParallelAsync(
         toolCalls);



            // ==========================================
            // 7. Continue conversation
            // ==========================================

            return await
                ContinueAfterToolCallsAsync(
                    response,
                    toolResults,
                    tools);
        }


        private async Task<string> ContinueAfterToolCallsAsync(
        string originalResponseJson, List<ToolExecutionResult>
            toolResults,

        List<object>
            tools)
        {
            using var document =
                JsonDocument.Parse(
                    originalResponseJson);

            var root =
                document.RootElement;

            var responseId =
                root.GetProperty("id")
                    .GetString();


            // ==========================================
            // Create ALL function_call_output items
            // ==========================================

            var inputs =
                new List<object>();


            foreach (
                var execution
                in toolResults)
            {
                var resultJson =
                    JsonSerializer.Serialize(
                        execution.Result);


                inputs.Add(
                    new
                    {
                        type =
                            "function_call_output",

                        call_id =
                            execution
                                .ToolCall
                                .CallId,

                        output =
                            resultJson
                    });
            }


            // ==========================================
            // Second request
            // ==========================================

            var request = new
            {
                model =
                    _options.Model,

                previous_response_id =
                    responseId,

                input =
                    inputs,

                tools =
                    tools
            };


            var response =
                await _client.PostAsJsonAsync(
                    "responses",
                    request);


            var responseJson =
                await response.Content
                    .ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"OpenAI tool result failed. " +
                    $"StatusCode: {response.StatusCode}. " +
                    $"Response: {responseJson}");
            }


            return ExtractOutputText(
                responseJson);
        }

        private async Task<string>  ContinueAfterToolCallsAsync(string originalResponseJson, List<object> toolResults,  List<OpenAIToolDefinition> tools)
        {
            using var document =
                JsonDocument.Parse(
                    originalResponseJson);

            var root =
                document.RootElement;

            var responseId =
                root.GetProperty("id")
                    .GetString();


            // ==========================================
            // Build function_call_output items
            // ==========================================

            var inputs =
                new List<object>();


            foreach (dynamic item
                in toolResults)
            {
                ToolCall toolCall =
                    item.ToolCall;

                object result =
                    item.Result;


                var resultJson =
                    JsonSerializer.Serialize(
                        result);


                inputs.Add(
                    new
                    {
                        type =
                            "function_call_output",

                        call_id =
                            toolCall.CallId,

                        output =
                            resultJson
                    });
            }


            // ==========================================
            // Second OpenAI request
            // ==========================================

            var request = new
            {
                model =
                    _options.Model,

                previous_response_id =
                    responseId,

                input =
                    inputs,

                tools =
                    tools
            };


            var response =
                await _client.PostAsJsonAsync(
                    "responses",
                    request);


            var responseJson =
                await response.Content
                    .ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"OpenAI tool result failed. " +
                    $"StatusCode: {response.StatusCode}. " +
                    $"Response: {responseJson}");
            }


            return ExtractOutputText(
                responseJson);
        }


        private async Task<string> SendOpenAIRequestAsync(OpenAIToolCallingRequest request)
        {
            // ==========================================
            // 1. Send request to OpenAI Responses API
            // ==========================================

            var response =
                await _client.PostAsJsonAsync(
                    "responses",
                    request);


            // ==========================================
            // 2. Read response body
            // ==========================================

            var responseJson =
                await response.Content
                    .ReadAsStringAsync();


            // ==========================================
            // 3. Check HTTP status
            // ==========================================

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"OpenAI request failed. " +
                    $"StatusCode: {response.StatusCode}. " +
                    $"Response: {responseJson}");
            }


            // ==========================================
            // 4. Return raw JSON
            // ==========================================

            return responseJson;
        }




        private async Task<List<ToolExecutionResult>> ExecuteToolCallsInParallelAsync(
    List<ToolCall> toolCalls)
        {
            var tasks = toolCalls.Select(ExecuteSingleToolAsync);

            var results =
                await Task.WhenAll(tasks);

            return results.ToList();
        }

     private async Task<ToolExecutionResult>  ExecuteSingleToolAsync(
        ToolCall toolCall)
        {
            var arguments =
                JsonSerializer.Deserialize<JsonElement>(
                    toolCall.Arguments);


            var result =
                await _toolExecutor.ExecuteAsync(
                    toolCall.Name,
                    arguments);

            return new ToolExecutionResult
            {
                ToolCall = toolCall,
                Result = result
            };
        }

        private async Task<List<object>> ExecuteToolCallsAsync(List<ToolCall> toolCalls)
        {
            var results = new List<object>();

            foreach (var toolCall
                in toolCalls)
            {
                var arguments =
                    JsonSerializer.Deserialize<JsonElement>(
                        toolCall.Arguments);

                var result =
                    await _toolExecutor.ExecuteAsync(
                        toolCall.Name,
                        arguments);

                results.Add(
                    new
                    {
                        ToolCall = toolCall,
                        Result = result
                    });
            }

            return results;
        }



        private List<ToolCall>  ExtractToolCalls(
        string responseJson)
        {
            var result = new List<ToolCall>();

            using var document =JsonDocument.Parse(responseJson);

            var root =document.RootElement;

            if (!root.TryGetProperty(
                    "output",
                    out var output))
            {
                return result;
            }

            foreach (
                var item
                in output.EnumerateArray())
            {
                if (!item.TryGetProperty(
                        "type",
                        out var type))
                {
                    continue;
                }

                if (type.GetString() !=
                    "function_call")
                {
                    continue;
                }

                var toolCall =
                    new ToolCall
                    {
                        CallId =
                            item.GetProperty(
                                "call_id")
                                .GetString() ?? "",

                        Name =
                            item.GetProperty(
                                "name")
                                .GetString() ?? "",

                        Arguments =
                            item.GetProperty(
                                "arguments")
                                .GetString() ?? "{}"
                    };

                result.Add(toolCall);
            }

            return result;
        }


       

        private async Task<string> ContinueAfterToolCallAsync(string originalResponseJson, ToolCall toolCall, object toolResult)
        {
            using var document =
                JsonDocument.Parse(
                    originalResponseJson);

            var root =
                document.RootElement;

            var responseId =
                root.GetProperty("id")
                    .GetString();


            var toolOutput =
                JsonSerializer.Serialize(
                    toolResult);


            var request = new
            {
                model = _options.Model,
                previous_response_id = responseId,
                input = new object[]
                {
                    new
                    {
                        type = "function_call_output",
                        call_id = toolCall.CallId,
                        output = toolOutput
                    }
                },
                tools = this._toolRegistry.GetToolDefinitions()
            };


            var response =await _client.PostAsJsonAsync(
                    "responses",
                    request);


            var responseJson =
                await response.Content
                    .ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"OpenAI tool result failed. " +
                    $"StatusCode: {response.StatusCode}. " +
                    $"Response: {responseJson}");
            }

            return ExtractOutputText(
                responseJson);
        }

        private ToolCall? ExtractToolCall(string responseJson)
        {
            using var document =JsonDocument.Parse(responseJson);

            var root = document.RootElement;

            if (!root.TryGetProperty(
                    "output",
                    out var output))
            {
                return null;
            }

            foreach (var item
                in output.EnumerateArray())
            {
                if (!item.TryGetProperty(
                        "type",
                        out var type))
                {
                    continue;
                }

                if (type.GetString() !=
                    "function_call")
                {
                    continue;
                }

                return new ToolCall
                {
                    CallId =item.GetProperty("call_id")
                            .GetString() ?? "",

                    Name =item.GetProperty("name")
                            .GetString() ?? "",

                    Arguments =item.GetProperty("arguments")
                            .GetString() ?? "{}"
                };
            }

            return null;
        }


   private string ExtractOutputText(string responseJson)
        {
         using var document = JsonDocument.Parse(responseJson);

            var root = document.RootElement;

            // Look inside output[]
            if (!root.TryGetProperty(
                    "output",
                    out var output))
            {
                return "";
            }

            foreach (var item
                in output.EnumerateArray())
            {
                // We only want message items
                if (!item.TryGetProperty(
                        "type",
                        out var type))
                {
                    continue;
                }

                if (type.GetString() != "message")
                {
                    continue;
                }

                // Get message.content[]
                if (!item.TryGetProperty(
                        "content",
                        out var content))
                {
                    continue;
                }

                foreach (var contentItem
                    in content.EnumerateArray())
                {
                    if (!contentItem.TryGetProperty(
                            "type",
                            out var contentType))
                    {
                        continue;
                    }

                    if (contentType.GetString() !=
                        "output_text")
                    {
                        continue;
                    }

                    if (contentItem.TryGetProperty(
                            "text",
                            out var text))
                    {
                        return text.GetString() ?? "";
                    }
                }
            }

            return "";
        }

               


    }

}


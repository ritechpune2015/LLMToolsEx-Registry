using LLMToolsEx.Dtos.Gemini;
using LLMToolsEx.Interfaces;
using LLMToolsEx.Options;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Text.Json;

namespace LLMToolsEx.Services
{
    public class GeminiService: IToolCallingLLMService

    {
        private readonly HttpClient _client;
        private readonly GeminiOptions _options;
        private readonly IToolExecutor _toolExecutor;
        public GeminiService(HttpClient client, IOptions<GeminiOptions> options,IToolExecutor toolExecutor)
        {
            _client = client;
            _options = options.Value;
            _toolExecutor = toolExecutor;
          
        _client.BaseAddress =
                new Uri(_options.BaseUrl);

            _client.DefaultRequestHeaders
                .Remove("x-goog-api-key");

            _client.DefaultRequestHeaders
                .Add("x-goog-api-key", _options.ApiKey);
        }

        public async Task<string> GenerateWithToolsAsync(string prompt)
        {
            // ==========================================
            // 1. Create function declaration
            // ==========================================

            var functionDeclaration = BuildCustomerBalanceTool();


            // ==========================================
            // 2. Create Gemini tool
            // ==========================================

            var tool =
                new GeminiTool
                {
                    FunctionDeclarations =
                    [
                        functionDeclaration
                    ]
                };


            // ==========================================
            // 3. Create first content
            // ==========================================

            var userContent =
                new GeminiContent
                {
                    Role = "user",
                    Parts =
                    [
                        new GeminiPart
                      {
                          Text = prompt
                        }
                    ]
                };


            // ==========================================
            // 4. Create request
            // ==========================================

            var request =
                new GeminiToolRequest
                {
                    Contents =
                    [
                        userContent
                    ],

                    Tools =
                    [
                        tool
                    ]
                };


            // ==========================================
            // 5. Send first request
            // ==========================================

            var response =
                await SendGeminiRequestAsync(
                    request);


            // ==========================================
            // 6. Extract function call
            // ==========================================

            var functionCall =
                ExtractFunctionCall(
                    response);


            // ==========================================
            // 7. No function required
            // ==========================================

            if (functionCall == null)
            {
                return ExtractText(
                    response);
            }


            // ==========================================
            // 8. Execute function
            // ==========================================

            var toolResult =
                await _toolExecutor.ExecuteAsync(
                    functionCall.Name,
                    functionCall.Args);


            // ==========================================
            // 9. Add model response to conversation
            // ==========================================

            var modelContent =
                ExtractModelContent(
                    response);


            // ==========================================
            // 10. Create function response
            // ==========================================

            var functionResponsePart =
                new GeminiPart
                {
                    FunctionResponse =
                        new GeminiFunctionResponse
                        {
                            Id =
                                functionCall.Id,

                            Name =
                                functionCall.Name,

                            Response =
                                new
                                {
                                    result =
                                        toolResult
                                }
                        }
                };


            // ==========================================
            // 11. Create second request
            // ==========================================

            var secondRequest =
                new GeminiToolRequest
                {
                    Contents =
                    [
                        userContent,

                modelContent,

                new GeminiContent
                {
                    Role = "user",

                    Parts =
                    [
                        functionResponsePart
                    ]
                }
              ],

                    Tools =
                    [
                        tool
                    ]
                };


            // ==========================================
            // 12. Send result back to Gemini
            // ==========================================

            var finalResponse =
                await SendGeminiRequestAsync(
                    secondRequest);


            // ==========================================
            // 13. Extract final answer
            // ==========================================

            return ExtractText(
                finalResponse);
        }

   private async Task<string> SendGeminiRequestAsync(GeminiToolRequest request)
        {
         
            var url =$"models/{_options.Model}" + ":generateContent" + $"?key={_options.ApiKey}";


            var response = await _client.PostAsJsonAsync(url, request);

            var responseJson = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Gemini Tool Calling failed. " +
                    $"StatusCode: {response.StatusCode}. " +
                    $"Response: {responseJson}");
            }

            return responseJson;
        }


        private GeminiFunctionCall? ExtractFunctionCall(string responseJson)
        {
            using var document = JsonDocument.Parse(responseJson);

            var root = document.RootElement;

            if (!root.TryGetProperty("candidates",
                    out var candidates))
            {
                return null;
            }

            foreach (
                var candidate
                in candidates.EnumerateArray())
            {
                if (!candidate.TryGetProperty(
                        "content",
                        out var content))
                {
                    continue;
                }

                if (!content.TryGetProperty(
                        "parts",
                        out var parts))
                {
                    continue;
                }

                foreach (
                    var part
                    in parts.EnumerateArray())
                {
                    if (!part.TryGetProperty(
                            "functionCall",
                            out var functionCall))
                    {
                        continue;
                    }

                    return JsonSerializer.Deserialize<
                        GeminiFunctionCall>(
                            functionCall.GetRawText());
                }
            }

            return null;
        }

        private string ExtractText(string responseJson)
        {
            using var document =
                JsonDocument.Parse(
                    responseJson);

            var root =
                document.RootElement;


            if (!root.TryGetProperty(
                    "candidates",
                    out var candidates))
            {
                return "";
            }


            foreach (
                var candidate
                in candidates.EnumerateArray())
            {
                if (!candidate.TryGetProperty(
                        "content",
                        out var content))
                {
                    continue;
                }


                if (!content.TryGetProperty(
                        "parts",
                        out var parts))
                {
                    continue;
                }


                foreach (
                    var part
                    in parts.EnumerateArray())
                {
                    if (part.TryGetProperty(
                            "text",
                            out var text))
                    {
                        return text.GetString() ?? "";
                    }
                }
            }

            return "";
        }


        private GeminiContent ExtractModelContent(string responseJson)
        {
            using var document = JsonDocument.Parse(responseJson);

            var root =   document.RootElement;

            if (!root.TryGetProperty(
                    "candidates",
                    out var candidates))
            {
                throw new InvalidOperationException(
                    "Gemini response contains no candidates.");
            }

            var candidate =
                candidates
                    .EnumerateArray()
                    .FirstOrDefault();

            if (candidate.ValueKind ==
                JsonValueKind.Undefined)
            {
                throw new InvalidOperationException(
                    "Gemini returned no candidate.");
            }

            if (!candidate.TryGetProperty(
                    "content",
                    out var content))
            {
                throw new InvalidOperationException(
                    "Gemini response contains no content.");
            }

            return JsonSerializer.Deserialize<
                GeminiContent>(
                    content.GetRawText())!;
        }


        private GeminiFunctionDeclaration BuildCustomerBalanceTool()
        {
            return new GeminiFunctionDeclaration
            {
                Name ="get_customer_balance",
                Description =
                    "Gets the current account balance " +
                    "for a customer.",

                Parameters = new
                {
                    type = "OBJECT",

                    properties = new
                    {
                        customerId = new
                        {
                            type = "STRING",
                            description =
                                "Unique customer ID, " +
                                "for example CUST001."
                        }
                    },

                    required = new[]
                    {
                    "customerId"
                    }
                }
            };
        }


    }
}


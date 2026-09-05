using LLMToolsEx.Interfaces;
using System.Text.Json;

namespace LLMToolsEx.Dtos.CustomerTools
{
    public class CustomerTool : ITool
    {
        public string Name => "get_customer";

        public string Description => "Gets customer details using customer ID.";

        public object GetDefinition()
        {
            return new
            {
                type = "function",
                name = Name,
                description = Description,
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        customerId = new
                        {
                            type = "string",
                            description = "Unique customer ID."
                        }
                    },

                    required = new[]
                    {
                    "customerId"
                },

                    additionalProperties = false
                }
            };
        }


        public async Task<object> ExecuteAsync(
                JsonElement arguments)
        {
            var customerId =
                arguments
                    .GetProperty("customerId")
                    .GetString();

            if (string.IsNullOrWhiteSpace(
                customerId))
            {
                throw new ArgumentException(
                    "customerId is required.");
            }
            return GetCustomerAsync(customerId);
        }

        public Task<List<CustomerDetails>> GetCustomerAsync(string customerId)
        {
            var result = CustomerDetails.GetCustomreDetails().Where(p => p.CustomerId == customerId).ToList();
            return Task.FromResult(result);
        }
     }
  }

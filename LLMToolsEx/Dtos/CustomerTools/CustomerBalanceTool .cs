using LLMToolsEx.Interfaces;
using System.Text.Json;

namespace LLMToolsEx.Dtos.CustomerTools
{
    public class CustomerBalanceTool : ITool
    {
        public string Name => "get_customer_balance";

        public string Description => "Gets the current account balance " +
            "of a customer.";


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

                            description =
                                "Unique customer ID."
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

        public async Task<object> ExecuteAsync(JsonElement arguments)
        {
            var customerId = arguments
                    .GetProperty("customerId")
                    .GetString();

            if (string.IsNullOrWhiteSpace(
                customerId))
            {
                throw new ArgumentException(
                    "customerId is required.");
            }

            return GetCustomerBalanceAsync(customerId);
        }

        public Task<List<CustomerBalance>> GetCustomerBalanceAsync(string customerId)
        {
            var result = CustomerBalance.GetCustomerBalanceList().Where(p => p.CustomerId == customerId).ToList();

            return Task.FromResult(result);
        }

    }
}


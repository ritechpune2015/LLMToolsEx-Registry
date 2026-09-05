using LLMToolsEx.Interfaces;
using System.Text.Json;

namespace LLMToolsEx.Dtos.CustomerTools
{
    public class CustomerOrdersTool : ITool
    {
        public string Name => "get_customer_orders";
        public string Description => "Gets orders belonging to a customer.";

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
                            type = "string"
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

            return GetCustomerOrders(customerId);

        }
        public Task<List<CustomerOrder>> GetCustomerOrders(string customerId)
        {
            var result = CustomerOrder.GetCustomerOrders().Where(p => p.CustomerId == customerId).ToList();

            return Task.FromResult(result);
        }
    }
}

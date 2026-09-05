namespace LLMToolsEx.Dtos.CustomerTools
{
    public class CustomerOrder
    {
        public string OrderId { get; set; } = "";
        public string CustomerId { get; set; } = "";
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";

        public static List<CustomerOrder> GetCustomerOrders()
        {
            List<CustomerOrder> lst = new List<CustomerOrder>();
            lst.Add(new CustomerOrder() { OrderId = "1", CustomerId = "121", Amount = 15000, Status = "Dispatched" });

            lst.Add(new CustomerOrder() { OrderId = "2", CustomerId = "122", Amount = 16000, Status = "Pending" });
            lst.Add(new CustomerOrder() { OrderId = "3", CustomerId = "123", Amount = 55000, Status = "Delivered" });
            lst.Add(new CustomerOrder() { OrderId = "4", CustomerId = "124", Amount = 55000, Status = "Delivered" });



            return lst;
        }
    }
}

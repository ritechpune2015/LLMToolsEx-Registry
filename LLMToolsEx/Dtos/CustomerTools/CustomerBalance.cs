namespace LLMToolsEx.Dtos.CustomerTools
{
    public class CustomerBalance
    {
        public string CustomerId { get; set; } = "";
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "";


        public static List<CustomerBalance> GetCustomerBalanceList()
        {
            List<CustomerBalance> lst = new List<CustomerBalance>();

            lst.Add(new CustomerBalance() { CustomerId = "121", Balance = 10000, Currency = "INR" });

            lst.Add(new CustomerBalance() { CustomerId = "123", Balance = 10000, Currency = "INR" });

            lst.Add(new CustomerBalance() { CustomerId = "124", Balance = 18000, Currency = "INR" });

            lst.Add(new CustomerBalance() { CustomerId = "125", Balance = 21000, Currency = "INR" });
            return lst;
        }
    }
}

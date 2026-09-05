namespace LLMToolsEx.Dtos.CustomerTools
{
    public class CustomerDetails
    {
        public string CustomerId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public static List<CustomerDetails> GetCustomreDetails()
        { 
           List<CustomerDetails> lst =new List<CustomerDetails>();
           lst.Add(new CustomerDetails() {CustomerId="121",Name="Sunil",Email="sunil@gmail.com",Phone="123254353453" });

            lst.Add(new CustomerDetails() { CustomerId = "123", Name = "Amit", Email = "amit@gmail.com", Phone = "897788778787" });

            lst.Add(new CustomerDetails() { CustomerId = "124", Name = "Sujit", Email = "sujit@gmail.com", Phone = "78897887898" });

            lst.Add(new CustomerDetails() { CustomerId = "125", Name = "Suresh", Email = "suresh@gmail.com", Phone = "9878676777" });
            return lst;
        }
    }
}

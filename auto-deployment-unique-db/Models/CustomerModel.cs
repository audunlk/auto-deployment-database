using System.Collections.Generic;
namespace auto_deployment_unique_db.Models

{

    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<Order>? Orders { get; set; }
    }
}

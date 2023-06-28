using System;

namespace auto_deployment_unique_db.Models
{

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
    }

}

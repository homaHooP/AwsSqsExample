using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsSqsExample.models
{
    public class PizzaOrder
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PizzaType { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}

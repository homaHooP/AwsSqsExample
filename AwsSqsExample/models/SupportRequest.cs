using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsSqsExample.models
{
    public enum PriorityLevel
    {
        Low,
        Medium,
        High
    }
    public class SupportRequest
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PriorityLevel Priority { get; set; } = PriorityLevel.Low;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

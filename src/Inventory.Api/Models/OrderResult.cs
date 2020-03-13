using System.Collections.Generic;

namespace Inventory.Api.Models
{
    public class OrderResult
    {
        public IReadOnlyCollection<OrderRow> Products { get; set; }
        
        public IReadOnlyCollection<OrderRow> MissingProducts { get; set; }
    }
}
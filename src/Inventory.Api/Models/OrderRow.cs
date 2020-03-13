using System;

namespace Inventory.Api.Models
{
    public class OrderRow
    {
        public Guid ProductId { get; set; }
        
        public int Amount { get; set; }
    }
}
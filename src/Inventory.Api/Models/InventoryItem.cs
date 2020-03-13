using System;

namespace Inventory.Api.Models
{
    public class InventoryItem
    {
        public Guid ProductId { get; set; }
        
        public int Count { get; set; }
    }
}
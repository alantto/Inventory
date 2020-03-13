using System;

namespace Inventory.Api.Models
{
    public class Product
    {
        public Guid? Id { get; set; }
        
        public string Name { get; set; }
    }
}
using System;
using System.Collections.Generic;
using Inventory.Api.Models;

namespace Inventory.Api.Services
{
    public interface IProductRepository
    {
        Product Find(Guid productId);

        IEnumerable<Product> All();
        
        void Add(Product product);
    }
}
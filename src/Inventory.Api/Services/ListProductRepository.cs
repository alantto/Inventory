using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Api.Models;

namespace Inventory.Api.Services
{
    public class ListProductRepository : IProductRepository
    {
        private readonly IList<Product> _products;

        public ListProductRepository()
        {
            _products = new List<Product>();
        }

        public Product Find(Guid productId) => _products.FirstOrDefault(product => product.Id == productId);

        public IEnumerable<Product> All() => _products;
        
        public void Add(Product product)
        {
            _products.Add(product);
        }
    }
}
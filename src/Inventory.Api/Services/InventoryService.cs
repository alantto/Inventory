using System;
using System.Collections.Generic;
using Inventory.Api.Models;

namespace Inventory.Api.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
        }

        public OrderResult Order(IEnumerable<OrderRow> order)
        {
            var missingProducts = new List<OrderRow>();
            var products = new List<OrderRow>();
            foreach (var orderRow in order)
            {
                if (_inventoryRepository.TryTake(orderRow.ProductId, orderRow.Amount, out var takenAmount))
                {
                    products.Add(orderRow);
                }
                else if (takenAmount == 0)
                {
                    missingProducts.Add(orderRow);
                }
                else
                {
                    var taken = new OrderRow() {ProductId = orderRow.ProductId, Amount = takenAmount};
                    products.Add(taken);
                    
                    var missing = new OrderRow() {ProductId = orderRow.ProductId, Amount = orderRow.Amount - takenAmount};
                    missingProducts.Add(missing);
                }
            }
            
            return new OrderResult()
            {
                MissingProducts = missingProducts,
                Products = products
            };
        }

        public void FillStock(IEnumerable<OrderRow> stockRows)
        {
            foreach (var row in stockRows)
            {
                _inventoryRepository.Fill(row.ProductId, row.Amount);
            }
        }

        public IEnumerable<InventoryItem> GetAll()
        {
            return _inventoryRepository.GetAll();
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Inventory.Api.Models;

namespace Inventory.Api.Services
{
    public class DictionaryInventoryRepository : IInventoryRepository
    {
        private readonly ConcurrentDictionary<Guid, int> _productInventory;

        public DictionaryInventoryRepository()
        {
            _productInventory = new ConcurrentDictionary<Guid, int>();
        }
        
        public bool TryTake(Guid productId, int amount, out int takenAmount)
        {
            while (true)
            {
                if (!_productInventory.TryGetValue(productId, out var inventoryAmount))
                {
                    takenAmount = 0;
                    return false;
                }

                takenAmount = inventoryAmount >= amount ? amount : inventoryAmount;
                var newAmount = inventoryAmount - takenAmount;
                if (_productInventory.TryUpdate(productId, newAmount, inventoryAmount))
                {
                    return takenAmount == amount;
                }
            }
        }

        public void Fill(Guid productId, int amount)
        {
            _productInventory.AddOrUpdate(productId, amount, ((key, oldValue) => amount + oldValue));
        }

        public IEnumerable<InventoryItem> GetAll()
        {
            return _productInventory.Select(x => new InventoryItem(){ ProductId = x.Key, Count = x.Value });
        }
    }
}
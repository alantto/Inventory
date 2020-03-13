using System;
using System.Collections.Generic;
using Inventory.Api.Models;

namespace Inventory.Api.Services
{
    public interface IInventoryRepository
    {
        bool TryTake(Guid productId, int amount, out int takenAmount);

        void Fill(Guid productId, int amount);
        
        IEnumerable<InventoryItem> GetAll();
    }
}
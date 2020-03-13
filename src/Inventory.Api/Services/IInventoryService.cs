using System;
using System.Collections.Generic;
using Inventory.Api.Models;

namespace Inventory.Api.Services
{
    public interface IInventoryService
    {
        OrderResult Order(IEnumerable<OrderRow> order);
        
        void FillStock(IEnumerable<OrderRow> stockRows);

        IEnumerable<InventoryItem> GetAll();
    }
}
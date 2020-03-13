using System;
using System.Collections.Generic;
using Inventory.Api.Models;
using Inventory.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        [HttpGet]
        public IEnumerable<InventoryItem> Get()
        {
            return _inventoryService.GetAll();
        }

        [HttpPost]
        [Route("order")]
        public OrderResult Order(IEnumerable<OrderRow> order)
        {
            return _inventoryService.Order(order);
        }
        
        [HttpPost]
        [Route("fill")]
        public IActionResult FillStock(IEnumerable<OrderRow> stockRows)
        {
            try
            {
                _inventoryService.FillStock(stockRows);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

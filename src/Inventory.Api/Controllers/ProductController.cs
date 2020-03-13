using System;
using System.Collections.Generic;
using Inventory.Api.Models;
using Inventory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _productRepository.All();
        }

        [HttpGet("{id}")]
        public Product Get(Guid id)
        {
            return _productRepository.Find(id);
        }

        [HttpPost]
        public IActionResult Post(Product product)
        {
            product.Id = Guid.NewGuid();

            _productRepository.Add(product);

            var url = Url.Action("Get", "Product", new {product.Id});
            return Created(url, product);
        }
    }
}
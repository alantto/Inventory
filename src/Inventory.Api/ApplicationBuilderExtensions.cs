using System;
using Inventory.Api.Models;
using Inventory.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Api
{
    public static class ApplicationBuilderExtensions
    {
        private static readonly Product[] Products = {
            new Product() {Id = new Guid("7D17FCDC-A14C-48DE-9891-3EEE6914DD6A"), Name = "Wilfa Grinder"},
            new Product() {Id = new Guid("ACB30B46-D828-47F6-8E8E-C94EC2493BCC"), Name = "Aeropress"},
            new Product() {Id = new Guid("44BCF3D7-24E0-4B13-BF02-692B8E3B2F6D"), Name = "Filter papers"},
            new Product() {Id = new Guid("5A6C733A-7862-454B-9A4C-6D103F47F707"), Name = "Bialetti Moka Express"},
        };

        private static readonly int[] Amounts = {4, 22, 333, 54};
        
        public static void AddDefaultProducts(this IApplicationBuilder app)
        {
            var productRepository = app.ApplicationServices.GetRequiredService<IProductRepository>();
            
            foreach (var product in Products)
            {
                productRepository.Add(product);
            }
        }
        
        public static void AddDefaultInventory(this IApplicationBuilder app)
        {
            var inventoryRepository = app.ApplicationServices.GetRequiredService<IInventoryRepository>();

            for (var i = 0; i < Products.Length; i++)
            {
                var guid = Products[i].Id;
                if (guid.HasValue)
                {
                    inventoryRepository.Fill(guid.Value, Amounts[i]);
                }
            }
        }
    }
}
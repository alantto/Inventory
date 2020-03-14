using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json;
using Inventory.Api.Models;
using RestSharp;

namespace Inventory.DemoScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://localhost:5001/api/");
            
            FetchAndPrintProducts(client);
            FetchAndPrintInventory(client);

            OrderAeropresses(client);
            
            FetchAndPrintInventory(client);

            AddNewProductChemex(client);
            
            FetchAndPrintProducts(client);
            FetchAndPrintInventory(client);

            OrderMultipleProducts(client);
            
            FetchAndPrintInventory(client);
        }

        private static void OrderMultipleProducts(RestClient client)
        {
            var request = new RestRequest("inventory/order", DataFormat.Json);
            var order = new[]
            {
                new OrderRow() {ProductId = new Guid("7D17FCDC-A14C-48DE-9891-3EEE6914DD6A"), Amount = 100},
                new OrderRow() {ProductId = new Guid("ACB30B46-D828-47F6-8E8E-C94EC2493BCC"), Amount = 100},
                new OrderRow() {ProductId = new Guid("44BCF3D7-24E0-4B13-BF02-692B8E3B2F6D"), Amount = 100}
            };

            request.AddJsonBody(order);
            
            var result = client.Post(request);
            PrintOrderResult(result.Content);
        }

        private static void AddNewProductChemex(RestClient client)
        {
            var request = new RestRequest("product", DataFormat.Json);
            request.AddJsonBody(new Product() { Name = "Chemex", Id = Guid.Empty});
            var result = client.Post(request);
            if (result.StatusCode != HttpStatusCode.Created)
            {
                Console.WriteLine("Error while adding product!");
                Console.WriteLine("Http status was not \"created\" but " + result.StatusCode);
                throw new Exception();
            }

            Console.WriteLine("New product added");
            var product = Deserialize<Product>(result.Content);
            
            var addToInventoryRequest = new RestRequest("inventory/fill", DataFormat.Json);

            var inventoryUpdate = new[]
            {
                new OrderRow() {ProductId = product.Id.Value, Amount = 20}
            };
            addToInventoryRequest.AddJsonBody(inventoryUpdate);
            client.Post(addToInventoryRequest);
        }

        private static void OrderAeropresses(RestClient client)
        {
            var request = new RestRequest("inventory/order", DataFormat.Json);
            request.AddJsonBody(new[]
                {new OrderRow() {ProductId = new Guid("ACB30B46-D828-47F6-8E8E-C94EC2493BCC"), Amount = 12}});
            var result = client.Post(request);
            PrintOrderResult(result.Content);
        }

        static void FetchAndPrintInventory(IRestClient client)
        {
            var request = new RestRequest("inventory", Method.GET, DataFormat.Json);
            var result = client.Get(request);
            var inventoryItems = Deserialize<IEnumerable<InventoryItem>>(result.Content);
            Console.WriteLine("Inventory status".ToUpper());
            Console.WriteLine("ProductID\tCount");
            foreach (var inventoryItem in inventoryItems)
            {
                Console.WriteLine($"{inventoryItem.ProductId}\t{inventoryItem.Count}");
            }
            Console.WriteLine();
        }

        static void FetchAndPrintProducts(IRestClient client)
        {
            var request = new RestRequest("product", Method.GET, DataFormat.Json);
            var result = client.Get(request);
            var products = Deserialize<IEnumerable<Product>>(result.Content);
            Console.WriteLine("Product list".ToUpper());
            Console.WriteLine("ProductID\tName");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id}\t{product.Name}");
            }
            Console.WriteLine();
        }

        static void PrintOrderResult(string json)
        {
            var orderResult = Deserialize<OrderResult>(json);
            Console.WriteLine("Order results".ToUpper());
            
            if (orderResult.Products.Any())
            {
                Console.WriteLine("Products in order:");
                
                Console.WriteLine("ProductID\tAmount");
                foreach (var row in orderResult.Products)
                {
                    Console.Write($"{row.ProductId}\t{row.Amount}");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("No products for this order found");
            }
            
            
            if (orderResult.MissingProducts.Any())
            {
                Console.WriteLine("Missing products in order:");
                Console.WriteLine("ProductID\tAmount");
                foreach (var row in orderResult.MissingProducts)
                {
                    Console.Write($"{row.ProductId}\t{row.Amount}");
                }
                Console.WriteLine();
            }
            
            Console.WriteLine();
        }

        static T Deserialize<T>(string json)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (System.Exception)
            {
                Console.Error.WriteLine($"Error while deserializing. Value {json} to {typeof(T).FullName}");
                throw;
            }
        }
    }
}

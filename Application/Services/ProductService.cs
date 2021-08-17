using Application.Models.Product;
using Application.Repositories;
using Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<ProductWriteResponse> BulkWrite(IEnumerable<ProductWriteRequestModel> requestedProducts)
        {
            var productBarcodeList = requestedProducts.Select(x => x.Barcode).ToList();
            var products = requestedProducts.Adapt<List<Product>>();

            var exitsProducts = (await productRepository.ListAsync(x => productBarcodeList.Contains(x.Barcode)))
                .ToDictionary(x=>x.Barcode,x=>x);

            var updateProducts = new List<Product>();
            var newProducts = new List<Product>();

            foreach (var product in products)
            {
                product.ModifiedDateTime = DateTime.Now;
                if (exitsProducts.ContainsKey(product.Barcode))
                {
                    product.Id = exitsProducts[product.Barcode].Id;
                    product.CreatedDateTime = exitsProducts[product.Barcode].CreatedDateTime;
                    updateProducts.Add(product);
                }
                else
                {
                    product.CreatedDateTime = DateTime.Now;
                    newProducts.Add(product);
                }
            }

            var result = await productRepository.BulkWriteAsync(inserts: newProducts, updates: updateProducts);

            return new ProductWriteResponse
            {
                InsertedCount = result.Inserted,
                UpdatedCount = result.Updated,
                FailedCount = requestedProducts.Count() - result.Inserted - result.Updated
            };
        }

        public async Task<List<ProductListItemModel>> ListProducts()
        {
            var products = await productRepository.ListAsync(x => x.Status != Domain.Enums.EntityStatusType.Deleted);

            return products.Adapt<List<ProductListItemModel>>();
        }
    }
}

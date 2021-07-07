using BFNet_2021_StorageCorrection.Models.Entities;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BFNet_2021_StorageCorrection.Repositories
{
    public interface ProductRepository
    {
        Product Create(Product product);
        Task<Product> CreateAsync(Product product);
        Product ReadOneByRowKey(string rowkey);
        Task<Product> ReadOneByRowKeyAsync(string rowKey);

        List<ProductImage> ReadImageByProduct(Product product);
        ProductImage AddImageToProduct(Product product, string path);
        Task<ProductImage> AddImageToProductAsync(Product product, string path);
    }
    public class ProductRepositoryImpl : ProductRepository
    {
        private string _AccessKey = string.Empty;
        private string _TableName = "product";
        private string _UserPartitionKey = "user";
        private string _ImagePartitionKey = "image";

        public ProductRepositoryImpl(string accessKey)
        {
            _AccessKey = accessKey;
        }
        private async Task<CloudTable> GetTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_AccessKey);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(_TableName);

            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Table is created");
            }

            return table;
        }
        public ProductImage AddImageToProduct(Product product, string path)
        {

            Task<ProductImage> task = Task.Run(() => AddImageToProductAsync(product, path));
            task.Wait();
            return task.Result;
        }
        public async Task<ProductImage> AddImageToProductAsync(Product product, string path)
        {
            CloudTable table = await GetTable();

            ProductImage productImage = new ProductImage();
            productImage.PartitionKey = _ImagePartitionKey;
            productImage.RowKey = product.RowKey;
            productImage.Path = path;

            TableOperation insertOperation = TableOperation.InsertOrMerge(productImage);
            TableResult info = await table.ExecuteAsync(insertOperation);

            return info.Result as ProductImage;
        }
        public Product Create(Product product)
        {
            Task<Product> task = Task.Run(() => CreateAsync(product));
            task.Wait();
            return task.Result;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            CloudTable table = await GetTable();
            TableOperation insertOperation = TableOperation.InsertOrMerge(product);
            TableResult info = await table.ExecuteAsync(insertOperation);

            return info.Result as Product;
        }

        public List<ProductImage> ReadImageByProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Product ReadOneByRowKey(string rowkey)
        {
            Task<Product> task = Task.Run(() => ReadOneByRowKeyAsync(rowkey));
            task.Wait();
            return task.Result;
        }

        public async Task<Product> ReadOneByRowKeyAsync(string rowKey)
        {
            CloudTable table = await GetTable();
            TableOperation retrieveOperation = TableOperation.Retrieve<Product>(_UserPartitionKey, rowKey);
            TableResult product = await table.ExecuteAsync(retrieveOperation);
            return product.Result as Product;
        }
    }
}

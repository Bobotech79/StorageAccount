using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace STORAGEACCOUNT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Table storage sample");

            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=demoproductstorage01;AccountKey=v/NOSVf9MSgPmCIL4xx3qcApBh8h6Rtda4iY56luwIcntZQy7sxpNiqsuZgVFp1YanqEan9iDw1k+AStwBE2Wg==;EndpointSuffix=core.windows.net";
            var tableName = "Products";

            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(tableName);

            ProductEntity product = new ProductEntity("Cresent", "Regular"){
                BikeNumber = 20,
                Size = 28
            };

            MergeProduct(table, product).Wait();
            QueryProduct(table, "Cresent", "Regular").Wait();
        }

        public static async Task MergeProduct(CloudTable table, ProductEntity product){
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(product);

            // Execute the operation.
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            ProductEntity insertedProduct = result.Result as ProductEntity;

            Console.WriteLine("Added product.");
        }

        public static async Task QueryProduct(CloudTable table, string name, string Model){
            TableOperation retrieveOperation = TableOperation.Retrieve<ProductEntity>(name, Model);

            TableResult result = await table.ExecuteAsync(retrieveOperation);
            ProductEntity product = result.Result as ProductEntity;

            if(product != null){
                Console.WriteLine("Fetched \t{0}\t{1}\t{2}\t{3}",
                product.PartitionKey, product.RowKey, product.BikeNumber, product.Size);
            }
        }
    }

    public class ProductEntity : TableEntity
    {
        public ProductEntity(){}

        public ProductEntity(string Name, string Model)
        {
            PartitionKey = Name;
            RowKey = Model;
        }

        public int BikeNumber{get; set;}
        public int Size{get; set;}
    }
}



using MongoDB.Driver;
using ProductService.Application.Database;
using ProductService.Application.Models;

namespace ProductService.Application.Repositories;

public class ProductRepository
    (MongoDbConnectionFactory connection)
    : IProductRepository
{
    private readonly MongoDbConnectionFactory connection = connection;

    public void Create(Product product)
    {
        var database = connection.Database;
        var products = database
            .GetCollection<Product>(connection.ProductCollection);
        products.InsertOne(product);
    }

    public Product? GetById(Guid id)
    {
        var database = connection.Database;
        var products = database.
            GetCollection<Product>(connection.ProductCollection);

        var filter = Builders<Product>.Filter.Eq("_id", id);
        var product = products.Find(filter);

        return product.FirstOrDefault();
    }

    public bool Update(Product product)
    {
        var database = connection.Database;
        var products = database
            .GetCollection<Product>(connection.ProductCollection);

        var filter = Builders<Product>.Filter.Eq("_id", product.Id);

        var result = products.ReplaceOne(filter, product);
        return result.MatchedCount > 0;
    }
}
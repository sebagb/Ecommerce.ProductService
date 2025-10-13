using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductService.Application.Models;
using ProductService.Application.Repositories;

namespace ProductService.Application;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = new HostApplicationBuilder();

        var dbName = builder.Configuration["Database:Name"];
        var connectionString =
            builder.Configuration["Database:ConnectionString"];

        builder.Services.AddApplication(connectionString!, dbName!);

        using IHost host = builder.Build();

        var repo = host.Services.GetRequiredService<IProductRepository>();

        var p = new Product()
        {
            Id = new Guid("444db02a-e466-48bf-b1f7-2a87ad60eb41"),
            Name = "Kwis",
            Category = "",
        };
        repo.Update(p);
    }
}
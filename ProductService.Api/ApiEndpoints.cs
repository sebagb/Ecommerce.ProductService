using System.Text.Json;
using ProductService.Application.Repositories;
using ProductService.Contract.Requests;

namespace ProductService.Api;

public static class ApiEndpoints
{
    private const string Base = "products";
    private readonly static string Create = $"{Base}";
    private readonly static string GetById = $"{Base}/{{id}}";
    private readonly static string Update = $"{Base}/{{id}}";

    private static readonly string jsonBodyKey = "jsonBody";

    public static void RegisterProductEndpoints(
        this IEndpointRouteBuilder builder)
    {
        builder.MapPost(Create, CreateProduct)
            .AddEndpointFilter(BodyValidationFilter<CreateProductRequest>);
        builder.MapGet(GetById, GetProductById);
        builder.MapPut(Update, UpdateProduct)
            .AddEndpointFilter(BodyValidationFilter<UpdateProductRequest>);
    }

    private static async Task<IResult> CreateProduct(
        HttpContext context,
        IProductRepository repo)
    {
        var request = (CreateProductRequest)context.Items[jsonBodyKey]!;

        var product = request.MapToProduct();

        repo.Create(product);

        var productResponse = product.MapToResponse();
        return Results.Ok(productResponse);
    }

    private static IResult GetProductById(
        Guid id,
        IProductRepository repo)
    {
        var product = repo.GetById(id);
        if (product == null)
        {
            return Results.NotFound();
        }

        var response = product.MapToResponse();
        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateProduct(
        HttpContext context,
        IProductRepository repo)
    {
        var productRequest = (UpdateProductRequest)context.Items[jsonBodyKey]!;

        var routeId = context.Request.RouteValues["id"]!;
        var productGuid = new Guid((string)routeId);

        var product = productRequest.MapToProduct(productGuid);

        var isUpdated = repo.Update(product);

        if (!isUpdated)
        {
            return Results.NotFound();
        }

        var response = product.MapToResponse();
        return Results.Ok(response);
    }

    private static async ValueTask<object?> BodyValidationFilter<T>(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        try
        {
            var request =
                await context.HttpContext.Request.ReadFromJsonAsync<T>();

            context.HttpContext.Items.Add(jsonBodyKey, request);
        }
        catch (Exception ex)
        {
            if (ex is JsonException || ex is InvalidOperationException)
            {
                return Results.BadRequest(ex.Message);
            }
            throw;
        }

        return await next(context);
    }
}
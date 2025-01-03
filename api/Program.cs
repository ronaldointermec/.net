using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configuração do serviço
builder.Services.AddSqlServer<ApplicationDBContext>(builder.Configuration["Database:SqlServer"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/product", (ProductRequest productRequest, ApplicationDBContext context) =>
{

    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = (Category)category,
    };

    if (productRequest.Tags != null)
    {

        product.Tags = new List<Tag>();

        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = item });
        }
    }

    context.Products.Add(product);
    context.SaveChanges();
    return Results.Created("/product" + product.Id, product.Id);
});

app.MapGet("/product/{code}", ([FromRoute] string code) =>

{
    var product = ProductRepository.GetBy(code);

    return product is not null
        ? Results.Ok(product)
        : Results.NotFound();
}

);
app.MapPut("/product", (Product product) =>
{

    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
    return Results.Ok();

});

app.MapDelete("/product/{code}", ([FromRoute] string code) =>
{

    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
    return Results.Ok();
});


if (app.Environment.IsDevelopment())
    app.MapGet("/configuration/database", (IConfiguration configuraton) =>
    {
        return Results.Ok(
            new
            {
                Connection = configuraton["Database:SqlServer"],
            }
                );
    });

app.Run();

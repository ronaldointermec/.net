using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/user",  () => new { Name = "Ronaldo Silva", Age = 44});


app.MapGet("/addheader", (HttpResponse response) => {
    response.Headers["Teste"] = "Ronaldo Silva"; // This will replace the value if it already exists
    return new { Name = "Ronaldo Silva", Age = 44 };
});

app.MapPost("/product",(Product product) =>{

    ProductRepository.Add(product);
    return Results.Created("/product" + product.Code, product.Code);
});

// http://localhost:3000/getproduct?dateStart=2024-10-01&dateEnd=2024-11-22
app.MapGet("/product", ([FromQuery] string dateStart, [FromQuery] string dateEnd) =>{
    return dateStart + " - " + dateEnd;
});

// http://localhost:3000/getproduct/123456780?
app.MapGet("/product/{code}", ( [FromRoute] string code) => {
    return ProductRepository.GetBy(code);
});


// recebendo do header 
app.MapGet("/productheader", ( HttpRequest request) => {
    return request.Headers["product-code"].ToString();
});

app.MapPut("/product", (Product product ) =>{

    var productSaved = ProductRepository.GetBy(product.Code);
    productSaved.Name = product.Name;
  
});


app.MapDelete("/product/{code}", ( [FromRoute] string code)  => {

    var productSaved = ProductRepository.GetBy(code);
    ProductRepository.Remove(productSaved);
});


app.Run();


public static class ProductRepository{
    public static List<Product> Products { get; set; }

    public static void Add(Product product){
        Products ??= [];
        Products.Add(product);    

    }

    public static Product GetBy(string code){
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product){
        Products.Remove(product);
    }
}


public class Product{
    public string? Code { get; set; }
    public string? Name { get; set; }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Serving images endpoint
app.MapGet("/api/images/{imageName}", async (HttpContext context) =>
{
    var imageName = context.Request.RouteValues["imageName"] as string;
    var imagePath = $"api/images/{imageName}"; // Replace with your image path

    if (File.Exists(imagePath))
    {
        var imageBytes = await File.ReadAllBytesAsync(imagePath);
        await context.Response.Body.WriteAsync(imageBytes);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
})
.WithName("GetImage")
.WithOpenApi(); // Add this line if you want to document this endpoint in Swagger


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// .env.dev 파일 로드 (로컬 개발 환경용)
// 배포 환경에서는 docker-compose.prod.yml이 환경 변수를 직접 주입하므로 이 부분이 실행되지 않아도 됨
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
if (environment == "Development")
{
    var root = Directory.GetCurrentDirectory();
    var dotenv = Path.Combine(root, "..", ".env.dev");
    if (File.Exists(dotenv))
    {
        DotNetEnv.Env.Load(dotenv);
        Console.WriteLine($"✅ Loaded environment variables from: {dotenv}");
    }
    else
    {
        Console.WriteLine($"⚠️  .env.dev not found at: {dotenv}");
    }
}
else
{
    Console.WriteLine($"🚀 Running in {environment} mode - using environment variables from container");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

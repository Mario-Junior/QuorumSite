using QuorumSite.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure URL based on environment
// Local: http://localhost:5000
// Docker: http://+:8080 (configured via ASPNETCORE_URLS env var in Dockerfile)
if (!builder.Environment.IsProduction() && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://localhost:5000");
}

// Register services
builder.Services.AddSingleton<CsvParsingService>();
builder.Services.AddSingleton<DataRepository>();
builder.Services.AddSingleton<QueryService>();

var app = builder.Build();

// Enable static files
app.UseDefaultFiles();
app.UseStaticFiles();

// API Endpoints
app.MapGet("/api/summary/legislators", (QueryService queryService) =>
{
    return Results.Ok(queryService.GetLegislatorSummaries());
});

app.MapGet("/api/summary/bills", (QueryService queryService) =>
{
    return Results.Ok(queryService.GetBillSummaries());
});

app.Run();

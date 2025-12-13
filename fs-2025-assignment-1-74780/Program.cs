using fs_2025_assignment_1_74780.Endpoints;
using fs_2025_assignment_1_74780.Startup;
using fs_2025_assignment_1_74780.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IDublinBikeService, DublinBikeServiceV1>();

var cosmosConnection = builder.Configuration["Cosmos:ConnectionString"];

if (!string.IsNullOrWhiteSpace(cosmosConnection))
{
    builder.Services.Configure<CosmosOptions>(builder.Configuration.GetSection("Cosmos"));
    builder.Services.AddSingleton(_ => new CosmosClient(cosmosConnection));
    builder.Services.AddSingleton<IDublinBikeServiceV2, DublinBikeServiceV2>();
    builder.Services.AddHostedService<DublinBikeBackgroundUpdater>();
}

builder.AddDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddWeatherEndPoints();
app.AddRootEndPoints();
app.AddBookEndPoints();
app.AddCourseEndPoints();

app.MapDublinBikeEndpoints();

app.Run();

public partial class Program { }
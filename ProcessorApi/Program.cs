using ProcessorLib.Contracts;
using ProcessorLib.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Environment.EnvironmentName = "Development";
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBatchProcessorService, BatchProcessorService>();
builder.Services.AddSingleton<KeywordExtractor>();
builder.Services.AddSingleton<MatchScorer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConcurrentResumeProcessor v1"));
// app.UseHttpsRedirection();
app.MapControllers();

// TODO: DEBUG - Remove later
app.MapGet("/debug", () => new { message = "alive!", path = Directory.GetCurrentDirectory() });
app.MapGet("/parse/{filePath}", (string filePath) =>
{
    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
    var exists = File.Exists(fullPath);
    return new { fullPath, exists, size = exists ? new FileInfo(fullPath).Length : 0 };
});

app.Run();

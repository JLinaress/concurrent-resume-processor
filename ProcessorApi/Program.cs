using ProcessorLib.Contracts;
using ProcessorLib.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowBlazorWebUI", policy => 
        policy.WithOrigins("http://localhost:5002")  // ✅ Fixed typo: "http"
            .AllowAnyMethod()
            .AllowAnyHeader());  // ✅ Good to add this
});

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

// ✅ Now this matches the policy name above
app.UseCors("AllowBlazorWebUI");  

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConcurrentResumeProcessor v1"));
app.MapControllers();

app.MapGet("/debug", () => new { message = "alive!", path = Directory.GetCurrentDirectory() });
app.MapGet("/parse/{filePath}", (string filePath) =>
{
    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
    var exists = File.Exists(fullPath);
    return new { fullPath, exists, size = exists ? new FileInfo(fullPath).Length : 0 };
});

app.Run();
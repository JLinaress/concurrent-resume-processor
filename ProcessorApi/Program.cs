using ProcessorLib.Contracts;
using ProcessorLib.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWebUI", policy =>
        policy.WithOrigins("http://localhost:5002")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Environment.EnvironmentName = "Development";
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBatchProcessorService, BatchProcessorService>();
builder.Services.AddSingleton<IKeywordExtractor, KeywordExtractor>();
builder.Services.AddSingleton<IMatchScorer, MatchScorer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowBlazorWebUI");  

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConcurrentResumeProcessor v1"));
app.MapControllers();

app.Run();
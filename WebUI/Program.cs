using WebUI.Contracts;
// using WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddScoped<IBatchMatchServices, BatchMatchServices>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("ProcessorApi", client => 
    client.BaseAddress = new Uri(builder.Configuration["ProcessorApiBaseUrl"] ?? "http://localhost:5000/"));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapRazorPages();
app.MapControllers();

// app.MapFallbackToRoute("{**slug}", "Match");

app.Run();
using ProcessorWebUI.Components;
using ProcessorWebUI.Contracts;
using ProcessorWebUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add scopeed services for API communication
builder.Services.AddScoped<IBatchMatchService, BatchMatchService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("ProcessorApi", client => 
    client.BaseAddress = new Uri(builder.Configuration["ProcessorApiBaseUrl"] ?? "http://localhost:5000/"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
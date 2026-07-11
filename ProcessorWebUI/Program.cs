using ProcessorWebUI.Components;
using ProcessorWebUI.Contracts;
using ProcessorWebUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("ProcessorApi", client =>
    client.BaseAddress = new Uri(
        builder.Configuration["ProcessorApiBaseUrl"] ?? "http://localhost:5000/"));

// Add scoped services for API communication
builder.Services.AddScoped<IMatchService, MatchService>();

// Add services to container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
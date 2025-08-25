using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using Client.Services;
using Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Use backend API base URL (set in wwwroot/appsettings.json or environment)
var apiBase = builder.Configuration["Api:BaseUrl"] ?? "https://localhost:5727"; // <- use your Swagger URL
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<ApiAuthService>();
builder.Services.AddScoped<SupabaseAuthService>(); // add this
builder.Services.AddScoped<AuthHttpClient>(); // Register AuthHttpClient
builder.Services.AddScoped<WeatherService>(); // Register WeatherService

var host = builder.Build();

// initialize auth service on startup
var auth = host.Services.GetRequiredService<ApiAuthService>();
await auth.InitializeAsync();

await host.RunAsync();
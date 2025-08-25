using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Data;
using Server.Supabase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) CORS:
builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowClient", p =>
        p.WithOrigins("https://localhost:5121")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// 2) MVC + Swagger:
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3) Authentication - Updated to use Supabase JWKs endpoint
var supabaseConfig = builder.Configuration.GetSection("Supabase");
var supabaseUrl = supabaseConfig["Url"];

if (string.IsNullOrEmpty(supabaseUrl))
{
    throw new Exception("Supabase URL is not configured in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{supabaseUrl}/auth/v1";
        options.Audience = "authenticated";
        options.RequireHttpsMetadata = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"{supabaseUrl}/auth/v1",
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "sub",
            RoleClaimType = "role"
        };
        
        // Configure to use Supabase JWKs endpoint
        options.MetadataAddress = $"{supabaseUrl}/auth/v1/.well-known/jwks.json";
    });

// 4) Your services:
builder.Services.AddSingleton<OpenWeatherMapService>();
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>().GetSection("MongoSettings");
    return new WeatherDataService(
        cfg["ConnectionString"]!,
        cfg["DatabaseName"]!,
        cfg["CollectionName"]!);
});
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>().GetSection("Supabase");
    return new SupabaseUserService(
        cfg["Url"]!,
        cfg["ApiKey"]!);
});
builder.Services.AddHttpClient<ForecastService>();

// Add HttpContextAccessor and UserContext services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextAccessor, UserContextAccessor>();

var app = builder.Build();

// 5) Pipeline:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

using DormMS.Models;
//using DormMS.Repository;
using DormMS.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddDbContext<DormMsnContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyCnn")));
//builder.Services.AddScoped<IBookingbedRepository, BookingbedRepository>();
//builder.Services.AddScoped<IBookingbedService, BookingbedService>();
//builder.Services.AddScoped<IPayOSService, PayOSService>();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();





builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie()
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"];
    options.ClientSecret = builder.Configuration["Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
});




builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
        )
    };
});

var app = builder.Build();
//app.UseDefaultFiles();   // tìm file mặc định

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "Home.html" }
});
app.UseStaticFiles();    // bật phục vụ file tĩnh
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("ngrok-skip-browser-warning", "true");
    await next();
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
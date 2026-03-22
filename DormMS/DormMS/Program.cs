using System.Text;
using DormMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DormMsnContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyCnn")));
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers()
    .AddOData(opt => opt
        .AddRouteComponents("api", GetEdmModel())
        .Filter()
        .Select()
        .OrderBy()
        .SetMaxTop(100)
        .Count()
    );
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

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Payment>("payment");
    return builder.GetEdmModel();
}

using Backend.Models;
using Backend.Services;
using Microsoft.IdentityModel.Tokens;
using Minio;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton(provider =>
{
    var minioConfig = builder.Configuration.GetSection("MinIO");
    return new MinioClient()
        .WithEndpoint(minioConfig["Endpoint"])
        .WithCredentials(minioConfig["AccessKey"], minioConfig["SecretKey"])
        .WithSSL(bool.Parse(minioConfig["UseSSL"]!))
        .Build();
});

builder.Services.Configure<List<User>>(builder.Configuration.GetSection("MockUsers"));
builder.Services.Configure<string[]>(builder.Configuration.GetSection("AllowedBuckets"));
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<MinioService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularAndSelf",
        policy => policy
            .WithOrigins("http://localhost:4200","http://localhost:5231", "http://localhost:4000")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAngularAndSelf");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

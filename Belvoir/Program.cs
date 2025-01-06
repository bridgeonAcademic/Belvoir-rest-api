using Belvoir.Helpers;
using Belvoir.Services;
using MySql.Data.MySqlClient;
using System.Data;


var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddScoped<IAuthServices, AuthServices>();


builder.Services.AddScoped<IJwtHelper, JwtHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbPassword = Environment.GetEnvironmentVariable("dbpassword") ?? string.Empty;
defaultConnectionString = defaultConnectionString.Replace("{dbpassword}", dbPassword);

builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection(defaultConnectionString));

builder.Services.AddScoped<AdminServices>();
builder.Services.AddScoped<ITailorservice,Tailorservice>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using AutoMapper;
using Belvoir.Bll.Helpers;
using Belvoir.Bll.Mappings;
using Belvoir.Bll.Services;
using Belvoir.Bll.Services.Admin;
using Belvoir.Bll.Services.DeliverySer;
using Belvoir.Bll.Services.Rentals;
using Belvoir.DAL.Repositories;
using Belvoir.DAL.Repositories.Admin;
using Belvoir.DAL.Repositories.DeliveryRep;
using Belvoir.DAL.Repositories.Rental;
using Belvoir.DAL.Repositories.Tailors;
using Belvoir.Bll.Hubs;
using Belvoir.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using Belvoir.Bll.Services.Notification;
using Belvoir.Bll.Services.UserSer;
using Belvoir.DAL.Repositories.UserRep;
using Belvoir.Bll.Services.Payments;
using Belvoir.DAL.Repositories.Payments;


var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<ITailorservice,Tailorservice>();
builder.Services.AddScoped<IClothsServices, ClothsServices>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddTransient<IAdminServices,AdminServices>();
builder.Services.AddScoped<IJwtHelper, JwtHelper>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<IRazorpayService, RazorpayService>();

builder.Services.AddScoped<IRentalService, RentalSevice>();
builder.Services.AddScoped<IRentalCartService,RentalCartService>();
builder.Services.AddScoped<IFabricService, FabricService>();

builder.Services.AddScoped<IOrderServices,OrderServices>();

builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddScoped<IDesignService,DesignService>();

builder.Services.AddScoped<IDeliveryServices, DeliveryServices>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IuserService, UserService>();
builder.Services.AddScoped<IRatingService, RatingService>();

builder.Services.AddScoped<ICookieService, CookieService>();

//Add Repository
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITailorRepository, TailorRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IClothesRepository, ClothesRepository>();

builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IRentalCartRepository,RentalCartRepository>();
builder.Services.AddScoped<IFabricRepository, FabricRepository>();

builder.Services.AddScoped<IOrderRepository,OrderRepository>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();

builder.Services.AddScoped<IDesignRepository,DesignRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationServiceSignal,NotificationServiceSignal>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

builder.Services.AddTransient<GlobalExceptionHandler>();

builder.Services.AddSignalR();
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfiles>();
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
 
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {your JWT token}\"",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
});

//cors
var allowedOrigins = new[] { "http://localhost:3000", "https://yourdomain.com" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins(allowedOrigins) 
            .AllowCredentials() 
            .AllowAnyHeader()
            .AllowAnyMethod());
});


// JWT Authentication configuration
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});



var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var dbPassword = Environment.GetEnvironmentVariable("dbpassword") ?? string.Empty;
defaultConnectionString = defaultConnectionString.Replace("{dbpassword}", dbPassword);

builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection(defaultConnectionString));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();    

app.UseAuthorization();

app.MapHub<NotificationHub>("/notification");

app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

app.Run();

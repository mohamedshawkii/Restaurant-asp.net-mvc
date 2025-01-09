using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Auth.Implementation;
using Restaurant.Auth.Interface;
using Restaurant.Auth.Interface.Validations;
using Restaurant.Auth.Validations.Authentication;
using Restaurant.Data;
using System.Text;
using Restaurant.Auth.Interface.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Restaurant.Models.Identity;
using Restaurant.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Restaurant.DependencyInjection;
public static class ServiceContainer
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
    {
        string connectionString = "DefaultConnection";
        services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(config.GetConnectionString(connectionString),
            sqlOptions => 
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure();
            }),
        ServiceLifetime.Scoped);

        services.AddAuthentication(
            options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(options => {
            options.SaveToken = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JWT:Issuer"],
                ValidAudience = config["JWT:Audience"],
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!))
            };
        }).AddCookie();

        services.AddIdentity<BaseUserModel, IdentityRole>(options =>
        {
            //options.SignIn.RequireConfirmedEmail = true;
            //options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredUniqueChars = 1;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddMvc(action => action.EnableEndpointRouting = false);

        services.AddScoped<IRepository<Product>, Repository<Product>>();
        services.AddScoped<IRepository<ProductIngredient>, Repository<ProductIngredient>>();
        services.AddScoped<IRepository<Category>, Repository<Category>>();
        services.AddScoped<IRepository<Ingredient>, Repository<Ingredient>>();
        services.AddScoped<IRepository<Order>, Repository<Order>>();
        services.AddScoped<IRepository<OrderItem>, Repository<OrderItem>>();
        services.AddScoped<IRepository<OrderItemViewModel>, Repository<OrderItemViewModel>>();
        services.AddScoped<IRepository<OrderViewModel>, Repository<OrderViewModel>>();

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<IUserManagement, UserManagment>();
        services.AddScoped<IRoleManagement, RoleManagement>();
        services.AddScoped<ITokenManagement, TokenManagement>();
        return services;
    }
}

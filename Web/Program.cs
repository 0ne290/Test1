using Core.Application;
using Core.Domain;
using Dal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Web;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<DrinksVendingMachineInteractor>(
            serviceProvider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<VendingContext>();
                
                var options = optionsBuilder
                    .UseSqlite(serviceProvider.GetService<IConfiguration>()!.GetConnectionString("Sqlite"))
                    .Options;

                return new DrinksVendingMachineInteractor(
                    new DrinksVendingMachine(new CoinsDao(new VendingContext(options))),
                    new DrinksDao(new VendingContext(options)));
            });
        builder.Services.AddCoreAdmin("Administrator");
        
        // Настраиваем авторизацию с помощью JWT
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // указывает, будет ли валидироваться издатель при валидации токена
                    ValidateIssuer = true,
                    // строка, представляющая издателя
                    ValidIssuer = "TestServer",
                    // будет ли валидироваться потребитель токена
                    ValidateAudience = true,
                    // установка потребителя токена
                    ValidAudience = "TestServerClient",
                    // установка ключа безопасности
                    IssuerSigningKey = new SymmetricSecurityKey("mysupersecret_secretsecretsecretkey!123"u8.ToArray()),
                    // валидация ключа безопасности
                    ValidateIssuerSigningKey = true,
                    //
                    ValidateLifetime = false,
                    LifetimeValidator = (_, _, _, _) => true
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.MapDefaultControllerRoute();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
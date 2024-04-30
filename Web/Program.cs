using Core.Application;
using Core.Domain;
using Dal;
using Microsoft.EntityFrameworkCore;

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

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        await app.RunAsync();
    }
}
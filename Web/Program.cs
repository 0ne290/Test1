using Domain;
using Dal;
using Microsoft.EntityFrameworkCore;
using DrinksVendingMachine = Domain.DrinksVendingMachine;

namespace Web;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<DrinksVendingMachine>(// Т. к. работа с автоматом это постоянное общение между сервером и клиентом посредством AJAX, то состояние автомата должно сохраняться на протяжении всей работы с ним. А это значит, что если экземпляр автомата будет создаваться только на время одного запроса, то состояние автомата будет постоянно сбрасываться и работа с ним будет невозможна. Подводя черту: если сервис автомата Scope или Transient, то после каждого AJAX-запроса состояние автомата будет сброшено - так не пойдет. Чтобы решить проблему, делаем сервис автомата синглтоном. Это НЕ идеальное решание - по хорошему время жизни должно быть Scope, при этом область видимости должна быть кастомной и это должно быть нечто более сложное, чем просто HTTP-запрос. Навскидку можно попробовать решить с помощью куки. Хотя, возможно, решение лежит в сессиях, но я мало что об этом знаю. В любом случае: это тестовое задание, а не коммерческий проект - итак сойдет)
            serviceProvider =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<VendingContext>();
                
                var options = optionsBuilder
                    .UseSqlite(serviceProvider.GetService<IConfiguration>()!.GetConnectionString("Sqlite"))
                    .Options;

                return new DrinksVendingMachine(new Dao<Drink>(new VendingContext(options)), new Dao<Coin>(new VendingContext(options)));
            });
        builder.Services.AddDbContext<VendingContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));// Вообще-то все зависимости компонентов слоя бизнес логики (Domain) разрешаются в момент построения этих компонентов (т. е. зависимости компонентов НЕ зарегистрированы в качестве сервисов - зарегистрированы только сами компоненты), но для библиотеки CoreAdmin НЕОБХОДИМО регистрировать контекст БД в качестве сервиса - следовательно, этот сервис будет использоваться только библиотекой CoreAdmin
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCoreAdmin();
        
        // Настраиваем авторизацию с помощью JWT. Это первый способ ограничить доступ к админке
        //builder.Services.AddAuthorization();
        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            // указывает, будет ли валидироваться издатель при валидации токена
        //            ValidateIssuer = true,
        //            // строка, представляющая издателя
        //            ValidIssuer = "TestServer",
        //            // будет ли валидироваться потребитель токена
        //            ValidateAudience = true,
        //            // установка потребителя токена
        //            ValidAudience = "TestServerClient",
        //            // установка ключа безопасности
        //            IssuerSigningKey = new SymmetricSecurityKey("mysupersecret_secretsecretsecretkey!123"u8.ToArray()),
        //            // валидация ключа безопасности
        //            ValidateIssuerSigningKey = true,
        //            //
        //            ValidateLifetime = false,
        //            LifetimeValidator = (_, _, _, _) => true
        //        };
        //    });

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
        
        // Это второй способ ограничить доступ к админке
        //app.UseCoreAdminCustomAuth(sp =>
        //{
        //    var httpRequest = sp.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request;// Раньше я всегда смотрел документацию по System.Web.HttpContext, а сегодня я узнал, что оказывается в ASP.NET Core используется Microsoft.AspNetCore.Http.HttpContext и все это время я читал не ту документацию :)
        //    try
        //    {
        //        return Task.FromResult(httpRequest.Query["accessKey"].ToString().GetHashString() == "E7126B537C1AC1C3B94F399A623D28A625D8A57769C698B8EFF044F25A47E469");
        //    }
        //    catch
        //    {
        //        return Task.FromResult(false);
        //    }
        //});
        
        // Это третий способ ограничить доступ к админке
        app.UseCoreAdminCustomAuth(_ => Task.FromResult(true));
        app.UseCoreAdminCustomUrl("faa024b13a45492a845041423516d37c");

        await app.RunAsync();
    }
}
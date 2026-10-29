using Microsoft.EntityFrameworkCore;
using SmartCurrencyAssistantWeb.Data;
using SmartCurrencyAssistantWeb.Services;
using SmartCurrencyAssistantWeb.Services.Interfaces;

namespace SmartCurrencyAssistantWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddMemoryCache();

            builder.Services.AddExceptionHandler<ApiExceptionHandler>();

            builder.Services.AddProblemDetails();

            builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();

            builder.Services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

            builder.Services.AddScoped<ICurrencyTrendService, CurrencyTrendService>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
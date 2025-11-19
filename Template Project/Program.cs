using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using Template_Project.Configuration;
using Template_Project.Utilities;
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04

namespace Template_Project
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            //AppConfiguration.Config(builder.Services, connectionString);
            builder.Services.Config(connectionString);
<<<<<<< HEAD
            builder.Services.RegisterMapsterConfig();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitializer>();
                dbInitializer.Initializer();
            }

=======

            var app = builder.Build();

>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
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
                pattern: "{area=Identity}/{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}

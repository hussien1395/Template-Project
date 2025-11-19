using ECommerce.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using Template_Project.Utilities;
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04

namespace Template_Project
{
    public static class AppConfiguration 
    {
        public static void Config(this IServiceCollection service,string connectionString)
        {
          
            service.AddDbContext<ApplicationDbContext>(options =>
            {
<<<<<<< HEAD
                //options.UseSqlServer(builder.Configuration.GetSection("ConnectionString")["DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]);
                //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
                options.UseSqlServer(connectionString);
            });

            service.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

<<<<<<< HEAD
            service.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            service.AddTransient<IEmailSender, EmailSender>();
            service.AddScoped<IRepository<Category>, Repository<Category>>();
            //service.AddSingleton<IRepository<Category>, Repository<Category>>();
            //service.AddTransient<IRepository<Category>, Repository<Category>>();
=======
            service.AddTransient<IEmailSender, EmailSender>();

            service.AddScoped<IRepository<Category>, Repository<Category>>();
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
            service.AddScoped<IRepository<Actor>, Repository<Actor>>();
            service.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            service.AddScoped<IRepository<Movie>, Repository<Movie>>();
            service.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
            service.AddScoped<IRepository<MovieSubImage>, Repository<MovieSubImage>>();
            service.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
<<<<<<< HEAD
            service.AddScoped<IDBInitializer, DBInitializer>();
=======
>>>>>>> 47837ba82ed9ef513408b815d13bf8256bfb6f04
        }
    }
}

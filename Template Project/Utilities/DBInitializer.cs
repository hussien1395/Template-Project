using Microsoft.EntityFrameworkCore;

namespace Template_Project.Utilities
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DBInitializer> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(
            ApplicationDbContext context,
            ILogger<DBInitializer> logger, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }

         public void Initializer()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new(SData.SUPPER_ADMIN_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SData.ADMIN_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SData.EMPLOYEE_ROLE)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new(SData.CUSTOMER_ROLE)).GetAwaiter().GetResult();

                    // Create a default supper admin user
                    _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "SupperAdmin",
                        FirstName = "Supper",
                        LastName = "Admin",
                        Email = "SupperAdmin@Gmail.com",
                        EmailConfirmed = true,

                    }, "SupperAdmin@123").GetAwaiter().GetResult();

                    var user = _userManager.FindByNameAsync("SupperAdmin").GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(user, SData.SUPPER_ADMIN_ROLE).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}

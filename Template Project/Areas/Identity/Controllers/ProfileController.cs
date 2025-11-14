using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Template_Project.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            //var userVM = new ApplicationUserVM()
            //{
            //    FullName= $"{user.FirstName} {user.LastName}",
            //    Email = user.Email,
            //    PhoneNumber= user.PhoneNumber,
            //    Address= user.Address,
            //};

            //TypeAdapterConfig<ApplicationUser, ApplicationUserVM>
            //    .NewConfig()
            //    .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

            var userVM = user.Adapt<ApplicationUserVM>();
            return View(userVM);
        }

        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return NotFound();
            }

            //var user = applicationUserVM.Adapt<ApplicationUserVM>();
            user.FirstName = applicationUserVM.FullName?.Split(' ')[0] ?? "";
            user.LastName = applicationUserVM.FullName?.Split(' ').Length > 1 ? applicationUserVM.FullName?.Split(' ')[1] ?? "" : "";
            //user.Email = applicationUserVM.Email;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            user.Address = applicationUserVM.Address;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = String.Join(", ", result.Errors.Select(e => e.Description));
                return View("Index", applicationUserVM);
            }
            return View();
        }

        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            if (String.IsNullOrEmpty(applicationUserVM.CurrentPassword)|| String.IsNullOrEmpty(applicationUserVM.NewPassword))
            {
                TempData["Error"] = "Current password and new password are required.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password updated successfully.";
            }
            else
            {
                TempData["Error"] = String.Join(", ", result.Errors.Select(e => e.Description));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;
using Template_Project.Models;

namespace Template_Project.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller 
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOTP> _applicationUserOTPRepository;

        public AccountController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IEmailSender emailSender, 
            IRepository<ApplicationUserOTP> applicationUserOTPRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOTPRepository = applicationUserOTPRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            };

            var result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account",
            new { area = "Identity", token, userId = user.Id },
            protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
      registerVM.Email,
      "Confirm your email",
      $"Please confirm your account by <a href='{link}'>clicking here</a>.");

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                TempData["Error"] = "Invalid User";
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Email Confirmation Failed";
            }
            else
            {
                TempData["Success"] = "Email Confirmed Successfully";
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(loginVM);
            }

            //await _userManager.CheckPasswordAsync(user,loginVM.UserNameOrEmail);
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "User account locked out.");
                    return View(loginVM);
                }
                else if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email is not confirmed.");
                    return View(loginVM);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginVM);
                }
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });

        }

        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail);
            
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(resendEmailConfirmationVM);
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email is already confirmed.");
                return View(resendEmailConfirmationVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account",new { area = "Identity", token, userId = user.Id },protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email,"Confirm your email",$"Please confirm your account by <a href='{link}'>clicking here</a>.");
            return RedirectToAction("Login");

        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM fotgetPasswordVM)
        {
            var user = await _userManager.FindByNameAsync(fotgetPasswordVM.UserNameOrEmail) ?? await _userManager.FindByEmailAsync(fotgetPasswordVM.UserNameOrEmail);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(fotgetPasswordVM);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email first.");
                return View(fotgetPasswordVM);
            }

            var otps= await _applicationUserOTPRepository.GetAsync(otp => otp.ApplicationUserId == user.Id);
            var last12HoursOTPs = otps.Count(otp => otp.CreatedAt > DateTime.UtcNow.AddHours(-24));
            if (last12HoursOTPs >= 15)
            {
                ModelState.AddModelError(string.Empty, "You have exceeded the maximum number of OTP requests. Please try again later.");
                return View(fotgetPasswordVM);
            } 

            foreach(var otp in otps)
            {
                otp.IsValid = false;
                _applicationUserOTPRepository.Update(otp);
                await _applicationUserOTPRepository.CommitAsync();
            }
            var OTP = new Random().Next(1000,9999).ToString();

            ApplicationUserOTP applicationUserOTP = new ApplicationUserOTP(OTP,user.Id);
            await _applicationUserOTPRepository.AddAsync(applicationUserOTP);
            await _applicationUserOTPRepository.CommitAsync();

            await _emailSender.SendEmailAsync(user.Email, "Reset Password", $"<h1> use this OTP <span style='color:red;'>'{OTP}'</span> to reset your password </h1>");
            return RedirectToAction("ValidateOTP",new { ApplicationUserId=user.Id});
        }

        [HttpGet]
        public IActionResult ValidateOTP(string applicationUserId)
        {
            var vm = new ValidateOTPVM
            {
                ApplicationUserId = applicationUserId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            var user = await _userManager.FindByIdAsync(validateOTPVM.ApplicationUserId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user.");
                return View(validateOTPVM);
            }

            var applicationUserOTP = await _applicationUserOTPRepository.GetOneAsync(
                otp=>otp.ApplicationUserId==validateOTPVM.ApplicationUserId &&
                otp.OTP==validateOTPVM.OTP &&
                otp.IsValid);

            if (applicationUserOTP == null) 
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
                return View(validateOTPVM);
            }

            if (DateTime.UtcNow > applicationUserOTP.ExpireAt)
            {
                applicationUserOTP.IsValid = false;
                _applicationUserOTPRepository.Update(applicationUserOTP);
                await _applicationUserOTPRepository.CommitAsync();
                ModelState.AddModelError(string.Empty, "OTP has expired.");
                return View(validateOTPVM);
            }

            applicationUserOTP.IsValid = false;
            _applicationUserOTPRepository.Update(applicationUserOTP);
            await _applicationUserOTPRepository.CommitAsync();
            TempData["OTPValidateUserId"] = user.Id;
            return RedirectToAction("ResetPassword", new { applicationuserId = user.Id });

        }

        [HttpGet]
        public IActionResult ResetPassword(string applicationUserId)
        {
            var OTPValidateUserId = TempData["OTPValidateUserId"]?.ToString();
            if (String.IsNullOrEmpty(OTPValidateUserId) || OTPValidateUserId != applicationUserId)
            {
                ModelState.AddModelError(string.Empty, "Unauthorized access to reset password.");
                return RedirectToAction("ForgetPassword");
            }
            TempData.Keep("OTPValidateUserId");

            var vm = new ResetPasswordVM
            {
                ApplicationUserId = applicationUserId
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            var OTPValidateUserId = TempData["OTPValidateUserId"]?.ToString();
            if (String.IsNullOrEmpty(OTPValidateUserId) || OTPValidateUserId != resetPasswordVM.ApplicationUserId)
            {
                ModelState.AddModelError(string.Empty, "Unauthorized access to reset password.");
                return RedirectToAction("ForgetPassword");
            }

            var user = await _userManager.FindByIdAsync(resetPasswordVM.ApplicationUserId);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid user.");
                return View(resetPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordVM.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(resetPasswordVM);
            }

            TempData.Remove("OTPValidateUserId");
            return RedirectToAction("Login");

        }

        public async Task<IActionResult> Logout(LoginVM loginVM)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}

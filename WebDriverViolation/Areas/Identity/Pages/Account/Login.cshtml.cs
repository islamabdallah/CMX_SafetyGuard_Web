using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Take5.Models.Models;
using Take5.Services.Contracts;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApplication22.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEmployeeService _employeeService;

        public LoginModel(SignInManager<AspNetUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<AspNetUser> userManager,
            IEmployeeService employeeService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _employeeService = employeeService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            public long EmployeeNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Home/Index");
            if (ModelState.IsValid)
            {
                var Employee = _employeeService.GetEmployee(Input.EmployeeNumber);
                if (Employee != null)
                {
                    AspNetUser aspNetUser = _userManager.FindByIdAsync(Employee.UserId).Result;
                    if(Employee.Company == "Security" && aspNetUser.Company == "Security")
                    {
                        var result = await _signInManager.PasswordSignInAsync(aspNetUser.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");
                            returnUrl = Url.Content("~/Home/Index");
                            return LocalRedirect(returnUrl);
                        }
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "محاولة دخول خاطئة.");
                            return Page();
                        }
                    }
                    else if (Employee.Company == "Cement" && aspNetUser.Company == "Cement")
                    {
                        var result = await _signInManager.PasswordSignInAsync(aspNetUser.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("User logged in.");
                            returnUrl = Url.Content("~/TruckEvents/Index");
                            return LocalRedirect(returnUrl);
                        }
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                        if (result.IsLockedOut)
                        {
                            _logger.LogWarning("User account locked out.");
                            return RedirectToPage("./Lockout");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "محاولة دخول خاطئة.");
                            return Page();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "محاولة دخول خاطئة.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

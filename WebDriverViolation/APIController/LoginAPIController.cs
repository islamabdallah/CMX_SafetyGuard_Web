using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Services.Contracts;
using WebDriverViolation.Services.Models.APIModels;
using WebDriverViolation.Services.Models.MasterModels;
using WebDriverViolation.Services.Models.Messages;

namespace WebDriverViolation.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAPIController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly IEmployeeService _employeeService;

        public LoginAPIController(SignInManager<AspNetUser> signInManager,
            UserManager<AspNetUser> userManager,
            IEmployeeService employeeService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _employeeService = employeeService;
        }
        [HttpPost("userLogin")]
        public async Task<ActionResult> UserLogin(LoginModel loginModel)
        {
            
            EmployeeModel employee = _employeeService.GetEmployee(loginModel.UserNumber);
            if (employee != null)
            {

                AspNetUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(aspNetUser.Email, loginModel.Password, true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return Ok(new { Message = UserMessage.LoginDone[loginModel.LanguageId], Data = employee.EmployeeNumber });
                    }
                    else
                    {
                        return BadRequest(new { Message = UserMessage.LoginFailed[loginModel.LanguageId], Data = 0 }); // FailedAccount
                    }
                }
                else
                {
                    return BadRequest(new { Message = UserMessage.EmailNotFound[loginModel.LanguageId], Data = 0 });
                }
              
            }
            else
            {
                return BadRequest(new { Message = UserMessage.LoginInvalidNumber[loginModel.LanguageId], Data = 0 });
            }

        }
    }
}

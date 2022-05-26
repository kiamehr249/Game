using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NikGame.Dart.Service;
using NikGame.Service;
using NikGame.Web.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NikGame.Web.Apis
{
    [Route("/api/[controller]/[action]")]
    public class Account : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IDartService _iDartServ;

        public Account(
            IConfiguration config,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IDartService iDartServ
            )
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _iDartServ = iDartServ;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                return StatusCode(400, new { message = "این کاربری یافت نشد", data = new { } });
            }

            var isTrust = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isTrust)
            {
                return StatusCode(401, new { message = "رمز عبور یا نام کاربری اشتباه است", data = new { } });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
            var key = _config["TokenOptions:Key"];

            var token = SysTools.GenerateToken(user.Id, user.UserName, userRoles, key, lifeDaies);

            return Ok(new
            {
                message = "دریافت موفق",
                data = new
                {
                    create = DateTime.Now.ToString(),
                    expire = DateTime.Now.AddDays(lifeDaies).ToString(),
                    token = token,
                    type = "bearer"
                }
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public IActionResult TestRole()
        {
            return Ok("Accessed to this API");
        }

        [HttpPost]
        public async Task<IActionResult> UserRegister([FromBody] RegisterRequest request)
        {
            var messages = new List<string>();

            if (string.IsNullOrEmpty(request.UserName))
            {
                messages.Add("موبایل باید مقدار داشته باشد");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                messages.Add("رمز عبور باید مقدار داشته باشد.");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                messages.Add("تکرار رمز عبور باید مقدار داشته باشد.");
            }

            if (request.Password != request.ConfirmPassword)
            {
                messages.Add("مقدار رمز عبور و تکرار آن یکسان نیست.");
            }

            if (messages.Count > 0)
            {
                return BadRequest(new
                {
                    status = 400,
                    messages = messages,
                    data = ""
                });
            }

            var oldUser = await _userManager.FindByNameAsync(request.UserName);
            if (oldUser != null)
            {
                messages.Add("این شماره موبایل در سامانه قبلا ثبت نام شده است");
                return BadRequest(new
                {
                    status = 400,
                    messages = messages,
                    data = ""
                });
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                //Email = request.Email,
                PhoneNumber = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                if (result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        messages.Add("message" + error.Description);
                    }
                }

                return BadRequest(new
                {
                    status = 400,
                    messages = messages,
                    data = ""
                });

            }

            await _userManager.AddToRoleAsync(user, "User");

            await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            var userRoles = await _userManager.GetRolesAsync(user);
            int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
            var key = _config["TokenOptions:Key"];

            var token = SysTools.GenerateToken(user.Id, user.UserName, userRoles, key, lifeDaies);

            messages.Add("ثبت نام با موفقیت انجام شد");
            return Ok(new
            {
                status = 200,
                messages = messages,
                data = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> SignInUser([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return Ok(new
                {
                    status = 500,
                    message = "خطا در مقادیر ورودی"
                });
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return Ok(new
                {
                    status = 402,
                    message = "شماره موبایل وارد شده دارای حساب کاربری نمی باشد. لطفا ابتدا حساب کاربری خود را ایجاد نمایید."
                });
            }

            if (user != null && !user.EmailConfirmed)
            {
                return Ok(new
                {
                    status = 402,
                    message = "این نام کابری هنوز تایید نشده است"
                });

            }

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(request.UserName);
            }

            var okPass = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!okPass)
            {
                return Ok(new
                {
                    status = 402,
                    message = "رمز عبور وارد شده اشتباه است",
                    data = ""
                });
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
            var key = _config["TokenOptions:Key"];

            var token = SysTools.GenerateToken(user.Id, user.UserName, userRoles, key, lifeDaies);

            var result = await _signInManager.PasswordSignInAsync(request.UserName, request.Password, request.RememberMe, true);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    status = 200,
                    message = "ورود موفق",
                    data = token
                });
            }
            else if (result.IsLockedOut)
            {
                return Ok(new
                {
                    status = 403,
                    message = "عدم دسترسی به سامانه",
                    data = ""
                });
            }
            else
            {
                return Ok(new
                {
                    status = 403,
                    message = "عدم دسترسی به سامانه",
                    data = ""
                });
            }
        }



    }
}

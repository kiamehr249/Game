using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NikGame.Dart.Service;
using NikGame.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NikGame.Web.Controllers
{
    public class UserPrivacy : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IDartService _iDartServ;
        public List<string> Messages;

        public UserPrivacy(
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
            Messages = new List<string>();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            LoginRequest model = new LoginRequest();
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("NikAdmin") || User.IsInRole("Admin"))
                {
                    return Redirect("/Panel");
                }
                else
                {
                    return Redirect("/UserPrivacy");
                }
            }

            LoginRequest model = new LoginRequest();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/UserPrivacy");
            }

            if (string.IsNullOrEmpty(model.UserName))
            {
                Messages.Add("Mobile can not be empty");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                Messages.Add("Password can not be empty.");
            }

            if (Messages.Count > 0)
            {
                ViewBag.Messages = Messages;
                return View(model);
            }

            model.UserName = model.UserName.PersianToEnglish();

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                Messages.Add("User Name or Password is not valid.");
                ViewBag.Messages = Messages;
                return View(model);

            }

            if (await _userManager.CheckPasswordAsync(user, model.Password) == false)
            {
                Messages.Add("User Name or Password is not valid.");
                ViewBag.Messages = Messages;
                return View(model);

            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return Redirect("/UserPrivacy");
            }
            else if (result.IsLockedOut)
            {
                return View("AccountLocked");
            }
            else
            {
                Messages.Add("Something wrong in sign in progress");
                ViewBag.Messages = Messages;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/UserPrivacy");
            }

            ViewBag.Messages = Messages;
            RegisterRequest model = new RegisterRequest();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/UserPrivacy");

            }

            if (string.IsNullOrEmpty(request.FirstName))
            {
                Messages.Add("First Name can not be empty.");
            }

            if (string.IsNullOrEmpty(request.LastName))
            {
                Messages.Add("Last Name can not be empty.");
            }

            if (string.IsNullOrEmpty(request.UserName))
            {
                Messages.Add("Phone Number can not be empty");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                Messages.Add("password can not be empty");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                Messages.Add("Password Confirm can not be empty");
            }

            if (request.Password != request.ConfirmPassword)
            {
                Messages.Add("Password and Confirm are not the same.");
            }

            ViewBag.Messages = Messages;

            if (Messages.Count() > 0)
            {
                return View(request);
            }

            request.UserName = request.UserName.PersianToEnglish();

            var userCheck = await _userManager.FindByNameAsync(request.UserName);
            if (userCheck == null)
            {
                var user = new User
                {
                    UserName = request.UserName,
                    //Email = request.Email,
                    PhoneNumber = request.UserName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);
                    return Redirect("/UserPrivacy");
                }
                else
                {
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            Messages.Add("message" + error.Description);
                        }
                    }
                    ViewBag.Messages = Messages;
                    return View(request);
                }
            }
            else
            {
                Messages.Add("This phone number has already account");
                ViewBag.Messages = Messages;
                return View(request);
            }

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/UserPrivacy/Login");
        }

    }
}

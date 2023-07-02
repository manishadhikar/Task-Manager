﻿using Application.Interface.Data;
using Application.Interface.Service;
using Application.ViewModels;
using Application.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace IdentityPractice.Controllers
{
    public class AccountController : Controller
    {

        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
    private readonly IRoleRepository _roleRepository;

        public AccountController(IUserService userService,
                                    IRoleService roleService,
                             IRoleRepository roleRepository)
        {
            _userService = userService;
            _roleService = roleService;
        _roleRepository = roleRepository;
        }
      
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            LoginViewModel loginViewModel = new LoginViewModel();
            loginViewModel.ReturnUrl = returnUrl ?? Url.Content("~/");
            return View(loginViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Model state is not valid");
                return View(loginViewModel);
            }
            var userExists = _userService.GetUserIdByName(loginViewModel.UserName);
            if (userExists.Equals(Guid.Empty))
            {
                ModelState.AddModelError(string.Empty, "User DoesNot Exist, Please Register First ");
                return View(loginViewModel);
            }
           var loginResult= await _userService.Login(loginViewModel);
            if (!loginResult.success)
            {
                TempData["SuccessMessage"] = "Couldnt login try again!";

            }
            if (loginViewModel.UserName == null)
            {
                ModelState.AddModelError(string.Empty, "Username is null");
                return View(loginViewModel);
            }
            var logedInUserId = _userService.GetUserIdByName(loginViewModel.UserName);
            var role = _roleRepository.IsAdmin(logedInUserId);

            if (role == true)
            {
                TempData["SuccessMessage"] = $"Your are loged in  successfully  as {loginViewModel.UserName}!";
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["SuccessMessage"] = $"Your are loged in  successfully  as {loginViewModel.UserName}!";
                return RedirectToAction("Index", "Employee");
            }
      
        }
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            //var roles = await _db.Role.ToListAsync();
            var roles = await _roleService.AllRoles();
            var listItems = roles.Select(role => new SelectListItem
            {
                Value = role.Id.ToString(),
                Text = role.Name
            }).ToList();
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.RoleList = listItems;
            registerViewModel.ReturnUrl = returnUrl;
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel, String? returnUrl = null)
        {
            registerViewModel.ReturnUrl = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                //<check if role exists or not
                var role = await _roleService.CheckIfExists(registerViewModel);
                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, "Role Doesnot exists");
                }
               var result = await _userService.Register(registerViewModel);
                if (result.sucess)
                {
                    TempData["SuccessMessage"] = "Registration  completed successfully!";
                    return RedirectToAction("Login", "Account");
                }
                ModelState.AddModelError(string.Empty, "Couldn't Register the user");
                TempData["SuccessMessage"] = "Couldn't Register  successfully!";

                return RedirectToAction("Register", "Account" );
              

            }

            return View(registerViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
           await _userService.Logout();
            TempData["SuccessMessage"] = "Loged Out Sucessfully!";
            return RedirectToAction("Login", "Account");
        }
    }

}

﻿using LyricsFinder.NET.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LyricsFinder.NET.Controllers
{
    [Authorize(Roles = "Moderator, Admin")]
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CustomAppUserData> _userManager;

        public RoleManagerController(
            UserManager<CustomAppUserData> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var admin = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            if (user == null || admin == null) return NotFound();

            await _userManager.DeleteAsync(user);

            return RedirectToAction("DisplayUsers");
        }

        /// <summary>
        /// Displays all users and associated info in table
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DisplayUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesView>();
            foreach (CustomAppUserData user in users)
            {
                var thisViewModel = new UserRolesView
                {
                    Id = user.Id,
                    Name = user.Name,
                    DOB = user.DOB,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    Roles = await GetUserRolesAsync(user)
                };
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Change/modify/update user assigned website roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageAsync(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesView>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesView
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name!))
                    userRolesViewModel.Selected = true;
                else
                    userRolesViewModel.Selected = false;

                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        /// <summary>
        /// Change/modify/update user assigned website roles
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAsync(List<ManageUserRolesView> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var admin = await _userManager.FindByEmailAsync(User!.Identity!.Name!);

            if (user == null || admin == null) return View();

            var roles = await _userManager.GetRolesAsync(user);

            // Prevent admin from removing their own admin status
            if (user == admin && roles.Contains("Admin"))
            {
                roles.Remove("Admin"); // remove Admin from current roles list so it is not deleted during RemoveFromRolesAsync method below

                // If only 1 role selected, remove all rolls except for admin, then return to view. This prevents admin from removing their own admin status
                if (model.Where(x => x.Selected).Count() == 1)
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    return RedirectToAction("DisplayUsers");
                }

                // if Admin still has admin role checked in selection page, remove it from model so it doesn't cause error in AddToRolesAsync method below
                if (model.Where(x => x.Selected).Select(y => y.RoleName).Contains("Admin"))
                {
                    model.RemoveAll(x => x.RoleName!.Equals("Admin"));
                }
            }

            // Only add/remove user roles if atleast one was selected by admin
            if (model.Where(x => x.Selected).Count() >= 1)
            {
                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove user existing roles.");
                    return View(model);
                }

                result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName)!);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected roles to user.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "User must be assigned atleast one role.");
                return View(model);
            }

            return RedirectToAction("DisplayUsers");
        }

        public async Task<IActionResult> RoleIdDescriptionAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        private async Task<List<string>> GetUserRolesAsync(CustomAppUserData user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}
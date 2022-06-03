// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectX.Models;

namespace ProjectX.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            EmploymentStatuses = new List<SelectListItem>
            {
                new SelectListItem {Value = "Student", Text = "Student"},
                new SelectListItem {Value = "Job-hunting", Text = "Job-hunting"},
                new SelectListItem {Value = "Working part-time", Text = "Working part-time"},
                new SelectListItem {Value = "Working full-time", Text = "Working full-time"},
                new SelectListItem {Value="Unemployed / Sabbatical", Text = "Unemployed / Sabbatical"},
                new SelectListItem {Value="Self-employed", Text = "Self-employed"},
                new SelectListItem {Value="Retired", Text = "Retired"}
            };
            SightCategories = new List<SelectListItem>
            {
                new SelectListItem {Value = "Partially-sighted", Text = "Partially-sighted"},
                new SelectListItem {Value = "Light Perception", Text = "Light Perception"},
                new SelectListItem {Value = "Totally Blind", Text = "Totally Blind"},
            };
            OffspringStatuses = new string[] {"Yes", "No"};
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        public List<SelectListItem> SightCategories { get; }
        public List<SelectListItem> EmploymentStatuses { get; }
        public string[] OffspringStatuses { get; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Sight Category")]
            public string SightCategory { get; set; }
            
            [Display(Name = "Employment Status")]
            public string EmploymentStatus { get; set; }
            
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            [Display(Name = "Company/Organization")]
            public string Company { get; set; }

            [Display(Name = "Industry")]
            public string Industry { get; set; }

            [StringLength(150, ErrorMessage = "Keep it between {2} and {1} characters please.", MinimumLength = 20)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Work Roles")]
            public string WorkRoles { get; set; }
            
            //[Required(ErrorMessage = "This field is required")]
            [StringLength(300, ErrorMessage = "Keep it between {2} and {1} characters please.", MinimumLength = 20)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Nature of work or tasks you enjoy working on, especially if you feel that your disability gives you an edge")]
            public string Strengths { get; set; }
            
            [Display(Name = "Are you a parent?")]
            public string IsParent { get; set; }

            [StringLength(1000)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "More about you")]
            public string Biog { get; set; }
            // [Phone]
            // [Display(Name = "Phone number")]
            // public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var sightCategory = user.SightCategory;
            var employmentStatus = user.EmploymentStatus;
            var jobTitle = user.JobTitle;
            var company = user.Company;
            var industry = user.Industry;
            var workRoles = user.WorkRoles;
            var strengths = user.Strengths;
            var isParent = user.IsParent;
            var biog = user.Biog;
            Username = userName;

            Input = new InputModel
            {
                // PhoneNumber = phoneNumber
                SightCategory = sightCategory,
                EmploymentStatus = employmentStatus,
                JobTitle = jobTitle,
                Company = company,
                Industry = industry,
                WorkRoles = workRoles,
                Strengths = strengths,
                IsParent = isParent,
                Biog = biog
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            var sightCategory = user.SightCategory;
            var employmentStatus = user.EmploymentStatus;
            var jobTitle = user.JobTitle;
            var company = user.Company;
            var industry = user.Industry;
            var workRoles = user.WorkRoles;
            var strengths = user.Strengths;
            var isParent = user.IsParent;
            var biog = user.Biog;
            // var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.SightCategory != sightCategory)
            {
                user.SightCategory = Input.SightCategory;
                await _userManager.UpdateAsync(user);
            }
            if (Input.EmploymentStatus != employmentStatus)
            {
                user.EmploymentStatus = Input.EmploymentStatus;
                await _userManager.UpdateAsync(user);
            }
            if (Input.JobTitle != jobTitle)
            {
                user.JobTitle = Input.JobTitle;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Company != company)
            {
                user.Company = Input.Company;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Industry != industry)
            {
                user.Industry = Input.Industry;
                await _userManager.UpdateAsync(user);
            }
            if (Input.WorkRoles != workRoles)
            {
                user.WorkRoles = Input.WorkRoles;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Strengths != strengths)
            {
                user.Strengths = Input.Strengths;
                await _userManager.UpdateAsync(user);
            }
            if (Input.IsParent != isParent)
            {
                user.IsParent = Input.IsParent;
                await _userManager.UpdateAsync(user);
            }
            if (Input.Biog != biog)
            {
                user.Biog = Input.Biog;
                await _userManager.UpdateAsync(user);
            }
            // if (Input.PhoneNumber != phoneNumber)
            // {
            //     var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            //     if (!setPhoneResult.Succeeded)
            //     {
            //         StatusMessage = "Unexpected error when trying to set phone number.";
            //         return RedirectToPage();
            //     }
            // }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProjectX.Models;

namespace ProjectX.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public List<SelectListItem> SightCategories { get; }

        public List<SelectListItem> EmploymentStatuses { get; }

        public string[] OffspringStatuses { get; }
        
        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "Name must be at least 7 and at max 100 characters long.", MinimumLength = 7)]
            [Display(Name = "Name")]
            public string Name { get; set; }
            
            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date Of Birth")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime? DateOfBirth { get; set; }
            
            [Required]
            [Display(Name = "Sight Category")]
            public string SightCategory { get; set; }

            [Required]
            [Display(Name = "Employment Status")]
            public string EmploymentStatus { get; set; }
            
            [Required]
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            [Display(Name = "Company/Organization")]
            public string Company { get; set; }

            [Display(Name = "Industry")]
            public string Industry { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Working Since")]
            public DateTime WorkingSince { get; set; }

            [Required(ErrorMessage = "This field is required")]
            [Range(0, 1000,
                ErrorMessage = "This value must be between {1} and {2}.")]
            [Display(Name = "Excluding Maternity / Paternity Leave, how many months have you taken as a break or breaks from your career, either for mental health or disability reasons? If none, enter 0.")]
            public int BreakTime { get; set; }
            
            //[Required(ErrorMessage = "This field is required")]
            [StringLength(150, ErrorMessage = "Keep it between {2} and {1} characters please.", MinimumLength = 20)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Tell us a little about your role and responsibilities")]
            public string WorkRoles { get; set; }
            //[Required(ErrorMessage = "This field is required")]
            [StringLength(300, ErrorMessage = "Keep it between {2} and {1} characters please.", MinimumLength = 20)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Tell us about the types of work or tasks you enjoy working on, especially if you feel that your disability gives you an edge")]
            public string Strengths { get; set; }
            
            [Required]
            [Display(Name = "Are you a parent?")]
            public string IsParent { get; set; }

            [StringLength(1000)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Anything else you’d like to add for your public profile")]
            public string Biog { get; set; }
            
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "I agree to abide by the Blink site rules")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "To successfully register you must accept the Blink site rules.")]
            public bool HasAccepted { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Name,
                    DateOfBirth = Input.DateOfBirth,
                    SightCategory = Input.SightCategory,
                    EmploymentStatus = Input.EmploymentStatus,
                    JobTitle = Input.JobTitle,
                    Company = Input.Company,
                    Industry = Input.Industry,
                    WorkingSince = Input.WorkingSince,
                    BreakTime = Input.BreakTime,
                    WorkRoles = Input.WorkRoles,
                    Strengths = Input.Strengths,
                    IsParent = Input.IsParent,
                    Biog = Input.Biog,
                    Email = Input.Email,
                    Joined = DateTime.UtcNow
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
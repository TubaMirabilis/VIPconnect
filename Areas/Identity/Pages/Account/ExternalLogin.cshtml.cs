using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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
            OffspringStatuses = new string[] { "Yes", "No" };
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        public List<SelectListItem> SightCategories { get; }

        public List<SelectListItem> EmploymentStatuses { get; }

        public string[] OffspringStatuses { get; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "Username must be at least 7 and at max 100 characters long.", MinimumLength = 7)]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date Of Birth")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime DateOfBirth { get; set; }

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

            [StringLength(150, ErrorMessage = "Keep it between {2} and {1} characters please.", MinimumLength = 20)]
            [DataType(DataType.MultilineText)]
            [Display(Name = "Tell us a little about your role and responsibilities")]
            public string WorkRoles { get; set; }
            
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
            [Display(Name = "I agree to abide by the Blink site rules")]
            [Range(typeof(bool), "true", "true", ErrorMessage = "To successfully register you must accept the Blink site rules.")]
            public bool HasAccepted { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new {ReturnUrl = returnUrl });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor : true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ReturnUrl = returnUrl;
                ProviderDisplayName = info.ProviderDisplayName;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    Input = new InputModel
                    {
                        UserName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    };
                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }
            var identifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
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
                    Joined = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    if(!string.IsNullOrEmpty(identifier) && info.LoginProvider == "Twitter")
                    {
                        await _userManager.AddClaimAsync(user, new Claim("TwitterId", identifier));
                    }
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email), "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("./RegisterConfirmation", new { Email = info.Principal.FindFirstValue(ClaimTypes.Email) });
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ProviderDisplayName = info.ProviderDisplayName;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Shop.Common.Models;
using System.Text.RegularExpressions;
using IdentityServer.LdapExtension;
using IdentityServer.LdapExtension.UserModel;
using IdentityServer4;
using IdentityServer.LdapExtension.UserStore;
using Shop.Identity.Settings;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Shop.Common;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Play.Common.Services;
using System.Text.Encodings.Web;
using Shop.Common.Extensions;

namespace Shop.Identity.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        // private readonly IConfiguration _configuration;
        // private readonly ILogger<LdapService<OpenLdapAppUser>> _ldapLogger;
        // private readonly ILdapUserStore _ldapUserStore;
        // private readonly IdentitySettings _identitySettings;
        // private readonly UserManager<ApplicationUser> _userManager;
        // private readonly IUserStore<ApplicationUser> _userStore;

        public LoginModel(SignInManager<ApplicationUser> signInManager, 
                          ILogger<LoginModel> logger
                          /*IConfiguration configuration, 
                          ILogger<LdapService<OpenLdapAppUser>> ldapLogger, 
                          ILdapUserStore ldapUserStore,
                          IOptions<IdentitySettings> identitySettings,
                          UserManager<ApplicationUser> userManager,
                          IUserStore<ApplicationUser> userStore*/)
        {
            _signInManager = signInManager;
            _logger = logger;
            // _configuration = configuration;
            // _ldapLogger = ldapLogger;
            // _ldapUserStore = ldapUserStore;
            // _identitySettings = identitySettings.Value;
            // _userManager = userManager;
            // _userStore = userStore;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

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
            [Required]
            // [EmailAddress]
            public string UserOrEmail { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var isEmail = true;//Regex.IsMatch(Input.UserOrEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                Microsoft.AspNetCore.Identity.SignInResult result = new Microsoft.AspNetCore.Identity.SignInResult();
                if (!isEmail)
                {
                    // LDAP Authentication using IdentityServer.LdapExtension
                    // var ldapConfig = _configuration.GetSection(nameof(IdentityServerLdap)).Get<IdentityServerLdap>();
                    // ExtensionConfig conf = ldapConfig.MapToExtensionConfig();
                    // var ldapService = new LdapService<OpenLdapAppUser>(conf, _ldapLogger); // or CustomLdapUser if you have a custom user class

                    // var user = _ldapUserStore.ValidateCredentials(Input.UserOrEmail, Input.Password);

                    // if (user != null)
                    // {
                    //     var identityUser = await _userStore.FindByNameAsync(user.Username, CancellationToken.None);
                        
                    //     if (identityUser?.Id == default(Guid))
                    //     {
                    //         var adUser = CreateUser();
                    //         adUser.Budget = _identitySettings.StartingBudget;
                    //         adUser.Email = user.Claims.FirstOrDefault(m => m.Type.Equals("email"))?.Value;
                    //         adUser.NormalizedEmail = adUser.Email?.ToUpper();

                    //         await _userStore.SetUserNameAsync(adUser, Input.UserOrEmail, CancellationToken.None);
                            
                    //         adUser.EmailConfirmed = true;

                    //         var signInResult = await _userManager.CreateAsync(adUser, Input.Password);

                    //         if (signInResult.Succeeded)
                    //         {
                    //             _logger.LogInformation("User created a new account with password.");

                    //             await _userManager.AddToRoleAsync(adUser, Roles.Consumer);
                    //             await _userManager.AddClaimsAsync(adUser, new Claim[]
                    //             { 
                    //                 new Claim("appscopes", UserClaims.Cart_ReadAccess), 
                    //                 new Claim("appscopes", UserClaims.Catalog_ReadAccess),
                    //                 new Claim("appscopes", UserClaims.Cart_WriteAccess), 
                    //                 new Claim("appscopes", UserClaims.Catalog_WriteAccess)
                    //             });
                                
                    //             await _signInManager.SignInAsync(adUser, isPersistent: false);
                    //         }
                    //         else
                    //         {
                    //             ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    //             return Page();
                    //         }
                    //     }
                    //     else
                    //     {
                    //         _logger.LogInformation("User logged in.");
                    //         await _signInManager.SignInAsync(identityUser, isPersistent: false);
                    //         return LocalRedirect(returnUrl);
                    //     }
                    // }
                    // else
                    // {
                    //     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    //     return Page();
                    // }
                }
                else
                {
                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    result = await _signInManager.PasswordSignInAsync(Input.UserOrEmail, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
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
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}

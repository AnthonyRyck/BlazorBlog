using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BlazorBlog.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public RegisterModel(
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class InputModel
		{
			[Required]
			[StringLength(20, ErrorMessage = "Le login ne doit pas faire plus de 20 caractères")]
			[Display(Name = "Login")]
			public string UserLogin { get; set; }

			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Mot de passe")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirmer le mot de passe")]
			[Compare("Password", ErrorMessage = "Le mot de passe et la confirmation ne sont pas identique.")]
			public string ConfirmPassword { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var user = new IdentityUser { UserName = Input.UserLogin, Email = Input.Email, EmailConfirmed = true };
				var result = await _userManager.CreateAsync(user, Input.Password);
				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					//await _signInManager.SignInAsync(user, isPersistent: false);
					await _userManager.AddToRoleAsync(user, "Member");
					return LocalRedirect(returnUrl);

					// *********** Pour confirmer le compte avec un mail ***********
					//var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					//code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					//var callbackUrl = Url.Page(
					//	"/Account/ConfirmEmail",
					//	pageHandler: null,
					//	values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
					//	protocol: Request.Scheme);

					//await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
					//	$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					// Ajout du rôle.
					//IdentityResult resultRole = await _userManager.AddToRoleAsync(user, Input.AccountType.ToString());

					//if (_userManager.Options.SignIn.RequireConfirmedAccount)
					//{
					//	return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
					//}
					//else
					//{
					//	await _signInManager.SignInAsync(user, isPersistent: false);
					//	return LocalRedirect(returnUrl);
					//}
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

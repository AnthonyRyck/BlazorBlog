using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorBlog.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ForgotPasswordConfirmation : PageModel
	{
		public void OnGet()
		{
		}
	}
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBlog.Data
{
	public class DataInitializer
	{
		private const string ROOT_USER = "root";

		public static async Task InitData(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
		{
			var roles = Enum.GetNames(typeof(Role));

			foreach (var role in roles)
			{
				// User est juste pour l'affichage.
				if (role == Role.SansRole.ToString())
					continue;

				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}

			// Création de l'utilisateur Root.
			var user = await userManager.FindByNameAsync(ROOT_USER);

			if (user == null)
			{
				var poweruser = new IdentityUser
				{
					UserName = ROOT_USER,
					Email = "root@email.com",
					EmailConfirmed = true
				};
				string userPwd = "Azerty123!";
				var createPowerUser = await userManager.CreateAsync(poweruser, userPwd);
				if (createPowerUser.Succeeded)
				{
					await userManager.AddToRoleAsync(poweruser, Role.Admin.ToString());
				}

				string pathUser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, ROOT_USER);
				if (!Directory.Exists(pathUser))
				{
					Directory.CreateDirectory(pathUser);
				}
			}
		}

	}
}

using BlazorBlog.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace BlazorBlog
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var host = CreateHostBuilder(args).Build();

				var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
				using (var scope = scopeFactory.CreateScope())
				{
					var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
					var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

					// Vrai si la base de données est créée, false si elle existait déjà.
					if (db.Database.EnsureCreated())
					{
						DataInitializer.InitData(roleManager, userManager).Wait();
					}
				}

				// Pour les logs.
				// ATTENTION : il faut que la table Logs (créé par Serilog) soit faites APRES
				// la création des tables ASP, sinon "db.Database.EnsureCreated" considère que la
				// base est déjà créée.
				var configuration = new ConfigurationBuilder()
							.SetBasePath(Directory.GetCurrentDirectory())
							.AddJsonFile("appsettings.json")
							.Build();

				string connectionDb = configuration.GetConnectionString("MySqlConnection");

				string databaseAddress = Environment.GetEnvironmentVariable("DB_HOST");
				string login = Environment.GetEnvironmentVariable("LOGIN_DB");
				string mdp = Environment.GetEnvironmentVariable("PASSWORD_DB");
				string dbName = Environment.GetEnvironmentVariable("DB_NAME");

				connectionDb = connectionDb.Replace("USERNAME", login)
										.Replace("YOURPASSWORD", mdp)
										.Replace("YOURDB", dbName)
										.Replace("YOURDATABASE", databaseAddress);

				Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
					.MinimumLevel.Override("System", LogEventLevel.Warning)
					.WriteTo.MySQL(connectionDb, "Logs")
					.CreateLogger();

				host.Run();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Erreur dans Main");
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}

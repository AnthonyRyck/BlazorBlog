using BlazorBlog.Areas.Identity;
using BlazorBlog.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

string connectionDb = builder.Configuration.GetConnectionString("MySqlConnection");

// *** Dans le cas ou une utilisation avec DOCKER
// *** voir post sur : https://www.ctrl-alt-suppr.dev/2021/02/01/connectionstring-et-image-docker/
//string databaseAddress = Environment.GetEnvironmentVariable("DB_HOST");
//string login = Environment.GetEnvironmentVariable("LOGIN_DB");
//string mdp = Environment.GetEnvironmentVariable("PASSWORD_DB");
//string dbName = Environment.GetEnvironmentVariable("DB_NAME");

string databaseAddress = "127.0.0.1";
string login = "Login";
string mdp = "le MDP";
string dbName = "Mettre le nom du schéma";

connectionDb = connectionDb.Replace("USERNAME", login)
						.Replace("YOURPASSWORD", mdp)
						.Replace("YOURDB", dbName)
						.Replace("YOURDATABASE", databaseAddress);
						
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionDb, ServerVersion.AutoDetect(connectionDb)));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

// Pour forcer l'application en Français.
var cultureInfo = new CultureInfo("fr-Fr");
cultureInfo.NumberFormat.CurrencySymbol = "€";

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


// Ajout dans la base de l'utilisateur "root"
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
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
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
	.MinimumLevel.Override("System", LogEventLevel.Warning)
	.WriteTo.MySQL(connectionDb, "Logs")
	.CreateLogger();

app.Run();
using BlazorBlog.Areas.Identity;
using BlazorBlog.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

#if (RELEASEDOCKER)
    string connectionDb = builder.Configuration.GetConnectionString("MySqlConnection");

    // *** Dans le cas ou une utilisation avec DOCKER
    // *** voir post sur : https://www.ctrl-alt-suppr.dev/2021/02/01/connectionstring-et-image-docker/
    string databaseAddress = Environment.GetEnvironmentVariable("DB_HOST");
    string login = Environment.GetEnvironmentVariable("LOGIN_DB");
    string mdp = Environment.GetEnvironmentVariable("PASSWORD_DB");
    string dbName = Environment.GetEnvironmentVariable("DB_NAME");

    connectionDb = connectionDb.Replace("USERNAME", login)
                            .Replace("YOURPASSWORD", mdp)
                            .Replace("YOURDB", dbName)
                            .Replace("YOURDATABASE", databaseAddress);
#elif DEBUG
string connectionDb = "server=127.0.0.1;user id=root;password=PassBlogDb;database=blogblazordb";
#else
string connectionDb = builder.Configuration.GetConnectionString("MySqlConnection");
#endif
						
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseMySql(connectionDb, ServerVersion.AutoDetect(connectionDb)));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

// Pour les tables "blogs"
//builder.Services.AddDbContextPool<blogContext>(opt => opt.UseMySql(connectionDb, ServerVersion.AutoDetect(connectionDb)));

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
	//var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	//var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Vrai si la base de donnees est a creer, false si elle existait deja.
    if (db.Database.EnsureCreated())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

		// Création des tables pour le blog
		//var dbContextBlog = scope.ServiceProvider.GetRequiredService<blogContext>();
		//var databaseCreator = dbContextBlog.GetService<IRelationalDatabaseCreator>();
		//databaseCreator.CreateTables();

		// Ajout dans la base de l'utilisateur "root"
		await DataInitializer.InitData(roleManager, userManager);
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
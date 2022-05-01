using BlazorBlog.Areas.Identity;
using BlazorBlog.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MudBlazor.Services;
using BlazorBlog.ViewModels;
using MudBlazor;
using Microsoft.Extensions.FileProviders;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using BlazorDownloadFile;

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

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

// Service de l'application
builder.Services.AddSingleton(new BlogContext(connectionDb));
builder.Services.AddSingleton<SettingsSvc>();

// MudBlazor Services
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});
builder.Services.AddMudMarkdownServices();

builder.Services.AddHotKeys();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISettingsViewModel, SettingsViewModel>();
builder.Services.AddScoped<IServiceImage, ImageService>();
builder.Services.AddTransient<INewPostViewModel, NewPostViewModel>();
builder.Services.AddScoped<IDisplayPostViewModel, DisplayPostViewModel>();
builder.Services.AddScoped<IGalerieViewModel, GalerieViewModel>();
builder.Services.AddScoped<IIndexViewModel, IndexViewModel>();
builder.Services.AddScoped<IArticlesViewModel, ArticlesViewModel>();
builder.Services.AddScoped<IEditPostViewModel, EditPostViewModel>();
builder.Services.AddScoped<ICategoriesViewModel, CategoriesViewModel>();
builder.Services.AddScoped<IGalerieSettingViewModel, GalerieSettingViewModel>();
builder.Services.AddBlazorDownloadFile(ServiceLifetime.Scoped);


// Augmentation de la taille des messages pour des posts très long.
builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 102400000;
});

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

    // Vrai si la base de donnees est a creer, false si elle existait deja.
    if (db.Database.EnsureCreated())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

		// Ajout dans la base de l'utilisateur "root"
		await DataInitializer.InitData(roleManager, userManager);
    }
	
    var blogCtx = scope.ServiceProvider.GetService<BlogContext>();
	string pathSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script", "BlogDb.sql");
	await blogCtx.CreateTablesAsync(pathSql);

    string updateSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script", "BlogDbUpdate.sql");
    await blogCtx.UpdateDatabaseAsync(updateSql);
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

// Chemin pour stocker les images
string pathImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES);
if (!Directory.Exists(pathImages))
    Directory.CreateDirectory(pathImages);

	
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(pathImages),
    RequestPath = ConstantesApp.USERIMG
});

app.Run();
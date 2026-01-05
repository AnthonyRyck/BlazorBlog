using BlazorBlog.Composants;

namespace BlazorBlog.ViewModels
{
	public class ProfilViewModel : IProfilViewModel
	{
		private readonly ISnackbar Snack;
		private readonly string UserName;
		private readonly IDialogService DialogService;
		private readonly DialogOptions FullScreenOption;
		private readonly BlogContext Context;

		public ProfilViewModel(IHttpContextAccessor httpContextAccessor, ISnackbar snackbar, IDialogService dialogService, BlogContext blogContext)
		{
			FullScreenOption = new DialogOptions() { FullScreen = true, CloseButton = true };
			UserName = httpContextAccessor.HttpContext.User.Identity.Name;
			Snack = snackbar;
			DialogService = dialogService;
			Context = blogContext;
		}


		#region IProfilViewModel

		/// <summary>
		/// URL de l'image du profil
		/// </summary>
		public Profil UserProfil { get; private set; } = new Profil();


		public async Task LoadProfil()
		{
			try
			{
				UserProfil = await Context.GetProfil(UserName);
			}
			catch (Exception ex)
			{
				Snack.Add("Erreur sur le chargement du profil", Severity.Error);
				Log.Error(ex, "ProfilViewModel - LoadProfil");
			}
		}

		/// <summary>
		/// Ouvre la galerie d'image pour le profil.
		/// </summary>
		/// <returns></returns>
		public async Task OpenGalerieProfil()
		{
			string extensionImg = ".jpg, .jpeg, .png";
			var parameters = new DialogParameters();
			parameters.Add("AcceptExtensions", extensionImg);

			var dialog = DialogService.Show<GalerieComponent>("Galerie", parameters, FullScreenOption);
			var result = await dialog.Result;

			if (!result.Cancelled)
			{
				UserProfil.UrlImage = result.Data.ToString();
			}
		}

		public async Task SaveProfil()
		{
			try
			{
				if (string.IsNullOrEmpty(UserProfil.UrlImage)) return;
				await Context.AddImgProfil(UserName, UserProfil.UrlImage);
			}
			catch (Exception ex)
			{
				Snack.Add("Erreur sur la sauvegarde du profil", Severity.Error);
				Log.Error(ex, "ProfilViewModel - SaveProfil");
			}
		}

		#endregion
	}
}

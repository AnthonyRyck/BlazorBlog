using BlazorBlog.Composants;
using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorBlog.ViewModels
{
	public class SettingsViewModel : ISettingsViewModel
	{
		private readonly ISnackbar Snack;
		private readonly SettingsSvc SvcSettings;
		private readonly IDialogService DialogService;
		private readonly DialogOptions FullScreenOption;

		public SettingsViewModel(ISnackbar snackbar, SettingsSvc settingsSvc, IDialogService dialogService)
		{
			Snack = snackbar;
			SvcSettings = settingsSvc;

			FullScreenOption = new DialogOptions() { FullScreen = true, CloseButton = true };			
			DialogService = dialogService;
			LoadSettings();

			EditContextValidation = new EditContext(Settings);
		}

		/// <summary>
		/// Vérifie que le dernier caratère à bien le "/"
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		private string VerifyUrl(string url)
		{
			url = url.Last() == '/' 
					? url
					: url + '/';

			return url;
		}

		#region ISettingsViewModel

		public SettingsValidation Settings { get; private set; }
		public EditContext EditContextValidation { get; set; }

		public string LogoSite { get; private set; }

		public void LoadSettings()
		{
			Settings = new SettingsValidation();
			Settings.BlogName = SvcSettings.BlogName;
			Settings.BlogDescription = SvcSettings.BlogDescription;
			Settings.BlogUrl = SvcSettings.BlogUrl;
			Settings.BlogImage = SvcSettings.BlogImage;
		}

		public async Task SaveSettings()
		{
			if (!EditContextValidation.Validate())
			{
				return;
			}
			else
			{
				try
				{
					List<Settings> settings = new List<Settings>();

					Settings settingName = new Settings()
					{
						SettingName = ConstantesApp.SETTINGS_BLOG_NAME,
						Value = Settings.BlogName
					};
					settings.Add(settingName);

					Settings settingDescription = new Settings()
					{
						SettingName = ConstantesApp.SETTINGS_BLOG_DESCRIPTION,
						Value = Settings.BlogDescription
					};
					settings.Add(settingDescription);

					Settings settingUrl = new Settings()
					{
						SettingName = ConstantesApp.SETTINGS_BLOG_URL,
						Value = VerifyUrl(Settings.BlogUrl)
					};
					settings.Add(settingUrl);

					Settings settingImage = new Settings()
					{
						SettingName = ConstantesApp.SETTINGS_BLOG_IMAGE,
						Value = Settings.BlogImage.Replace("../", Settings.BlogUrl)
					};
					settings.Add(settingImage);

					await SvcSettings.UpadateSettings(settings);
					Snack.Add("Sauvegarde des paramètres - OK", Severity.Success);
				}
				catch (Exception ex)
				{
					Snack.Add("Erreur sur la sauvegarde du post", Severity.Error);
					Log.Error(ex, "NewPostViewModel - SavePost");
				}
			}
		}



		public async Task OpenGalerie()
		{
			var dialog = DialogService.Show<GalerieComponent>("Galerie", FullScreenOption);
			var result = await dialog.Result;

			if (!result.Cancelled)
			{
				LogoSite = result.Data.ToString();
				Settings.BlogImage = LogoSite;
			}
		}

		#endregion
	}
}

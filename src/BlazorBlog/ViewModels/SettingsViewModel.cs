using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorBlog.ViewModels
{
	public class SettingsViewModel : ISettingsViewModel
	{
		private readonly ISnackbar Snack;
		private readonly SettingsSvc SvcSettings;

		public SettingsViewModel(ISnackbar snackbar, SettingsSvc settingsSvc)
		{
			Snack = snackbar;
			SvcSettings = settingsSvc;
			LoadSettings();

			EditContextValidation = new EditContext(Settings);
		}

		#region ISettingsViewModel

		public SettingsValidation Settings { get; private set; }
		public EditContext EditContextValidation { get; set; }
		
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
						Value = Settings.BlogUrl
					};
					settings.Add(settingUrl);

					Settings settingImage = new Settings()
					{
						SettingName = ConstantesApp.SETTINGS_BLOG_IMAGE,
						Value = Settings.BlogImage
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

		#endregion
	}
}

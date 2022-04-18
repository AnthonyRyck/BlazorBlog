using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public interface ISettingsViewModel
	{
		SettingsValidation Settings { get; }

		EditContext EditContextValidation { get; set; }

		string LogoSite { get; }

		void LoadSettings();

		Task SaveSettings();

		Task OpenGalerie();
	}
}

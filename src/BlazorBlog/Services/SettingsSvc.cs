namespace BlazorBlog.Services
{
	public class SettingsSvc
	{
		public string BlogName { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_NAME); } }

		public string BlogDescription { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_DESCRIPTION); } }

		public string BlogUrl { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_URL); } }

		public string BlogImage { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_IMAGE); } }


		private List<Settings> AllSettings;
		private readonly BlogContext Context;

		public SettingsSvc(BlogContext context)
		{
			Context = context;
			AllSettings = LoadSettings().GetAwaiter().GetResult();
		}

		private async Task<List<Settings>> LoadSettings()
		{
			var settings = await Context.GetSettings();

			if (settings.Count == 0)
			{
				// Cas ou la talbe Setting n'est pas encore rempli.
				settings = await FirstUseSetting();
			}

			return settings;
		}

		private async Task<List<Settings>> FirstUseSetting()
		{
			List<Settings> settings = new List<Settings>();

			Settings settingName = new Settings()
			{
				SettingName = ConstantesApp.SETTINGS_BLOG_NAME,
				Value = "My Blog"
			};
			settings.Add(settingName);

			Settings settingDescription = new Settings()
			{
				SettingName = ConstantesApp.SETTINGS_BLOG_DESCRIPTION,
				Value = string.Empty
			};
			settings.Add(settingDescription);

			Settings settingUrl = new Settings()
			{
				SettingName = ConstantesApp.SETTINGS_BLOG_URL,
				Value = string.Empty
			};
			settings.Add(settingUrl);

			Settings settingImage = new Settings()
			{
				SettingName = ConstantesApp.SETTINGS_BLOG_IMAGE,
				Value = string.Empty
			};
			settings.Add(settingImage);

			await Context.AddDefaultSettings(settings);

			return settings;
		}

		public string GetSetting(string settingName)
		{
			var selected = AllSettings.FirstOrDefault(x => x.SettingName == settingName);
			if (selected != null)
			{
				return selected.Value;
			}
			else return "ERROR_SETTING";
		}

		public async Task UpadateSettings(List<Settings> settings)
		{
			await Context.UpdateSettings(settings);
		}
	}
}

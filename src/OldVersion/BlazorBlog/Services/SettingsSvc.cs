namespace BlazorBlog.Services
{
	public class SettingsSvc
	{
		public string BlogName { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_NAME); } }

		public string BlogDescription { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_DESCRIPTION); } }

		public string BlogUrl { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_URL); } }

		public string BlogImage { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_IMAGE); } }

		public string BlogIcon { get { return GetSetting(ConstantesApp.SETTINGS_BLOG_ICONE); } }

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
			settings = await FirstUseSetting(settings);
			return settings;
		}

		private async Task<List<Settings>> FirstUseSetting(List<Settings> settingsBdd)
		{
			List<Settings> settings = new List<Settings>();

			if (!settingsBdd.Any(x => x.SettingName == ConstantesApp.SETTINGS_BLOG_NAME))
			{
				Settings settingName = new Settings()
				{
					SettingName = ConstantesApp.SETTINGS_BLOG_NAME,
					Value = "My Blog"
				};
				settings.Add(settingName);
				settingsBdd.Add(settingName);
			}
			if (!settingsBdd.Any(x => x.SettingName == ConstantesApp.SETTINGS_BLOG_DESCRIPTION))
			{
				Settings settingDescription = new Settings()
				{
					SettingName = ConstantesApp.SETTINGS_BLOG_DESCRIPTION,
					Value = string.Empty
				};
				settings.Add(settingDescription);
				settingsBdd.Add(settingDescription);
			}
			if (!settingsBdd.Any(x => x.SettingName == ConstantesApp.SETTINGS_BLOG_URL))
			{
				Settings settingUrl = new Settings()
				{
					SettingName = ConstantesApp.SETTINGS_BLOG_URL,
					Value = string.Empty
				};
				settings.Add(settingUrl);
				settingsBdd.Add(settingUrl);
			}
			if (!settingsBdd.Any(x => x.SettingName == ConstantesApp.SETTINGS_BLOG_IMAGE))
			{
				Settings settingImage = new Settings()
				{
					SettingName = ConstantesApp.SETTINGS_BLOG_IMAGE,
					Value = string.Empty
				};
				settings.Add(settingImage);
				settingsBdd.Add(settingImage);
			}
			if (!settingsBdd.Any(x => x.SettingName == ConstantesApp.SETTINGS_BLOG_ICONE))
			{
				Settings settingIcone = new Settings()
				{
					SettingName = ConstantesApp.SETTINGS_BLOG_ICONE,
					Value = string.Empty
				};
				settings.Add(settingIcone);
				settingsBdd.Add(settingIcone);
			}

			await Context.AddDefaultSettings(settings);

			return settingsBdd;
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
			AllSettings = settings;
		}


		public string GetUrlImagePost(string imageName)
		{
			return !string.IsNullOrEmpty(BlogUrl)
				? imageName.Replace("../", BlogUrl)
				: imageName;
		}
	}
}

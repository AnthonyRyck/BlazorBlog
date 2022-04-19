namespace BlazorBlog.ViewModels
{
	public class DisplayPostViewModel : IDisplayPostViewModel
	{
		private readonly BlogContext Context;
		private readonly SettingsSvc Settings;

		public DisplayPostViewModel(BlogContext blogContext, SettingsSvc settingSvc)
		{
			Context = blogContext;
			Settings = settingSvc;
		}


		#region IDisplayPostViewModel

		public bool IsLoading { get; private set; }

		public Post Article { get; private set; }

		public async Task LoadPost(int idpost)
		{
			IsLoading = true;
			var temp = await Context.GetPostAsync(idpost);
			temp.Image = Settings.GetUrlImagePost(temp.Image);

			Article = temp;
			IsLoading = false;
		}

		#endregion

	}
}
namespace BlazorBlog.ViewModels
{
	public class DisplayPostViewModel : IDisplayPostViewModel
	{
		private readonly BlogContext Context;
		private readonly SettingsSvc Settings;
		private readonly NavigationManager NavigationManager;

		public DisplayPostViewModel(BlogContext blogContext, SettingsSvc settingSvc, NavigationManager navigationManager)
		{
			Context = blogContext;
			Settings = settingSvc;
			DisabledShare = true;
			NavigationManager = navigationManager;
		}


		#region IDisplayPostViewModel

		public bool IsLoading { get; private set; }

		public Post Article { get; private set; }

		public string UrlShareLinkedIn { get; private set; }

		public string UrlShareTwitter { get; private set; }

		public bool DisabledShare { get; private set; }

		public IEnumerable<Categorie> Categories { get; private set; }

		public async Task LoadPost(int idpost)
		{
			IsLoading = true;
			var temp = await Context.GetPostAsync(idpost);
			temp.Image = Settings.GetUrlImagePost(temp.Image);

			Article = temp;
			Categories = await Context.GetCategories(idpost);

			if (!string.IsNullOrEmpty(Settings.BlogUrl))
			{
				string urlPost = Settings.BlogUrl + "post/" + idpost;
				UrlShareLinkedIn = "https://www.linkedin.com/shareArticle?mini=true&url=" + urlPost;
				UrlShareTwitter = "https://twitter.com/intent/tweet?url=" + urlPost;

				DisabledShare = false;
			}
			IsLoading = false;
		}

		public void OpenCategoriePosts(int idCategorie)
		{
			NavigationManager.NavigateTo($"/categorie/{idCategorie}", true);
		}

		#endregion

	}
}
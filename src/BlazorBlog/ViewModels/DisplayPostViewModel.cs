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

		public string MetaKeywords { get; private set; }
		public string MetaDescription { get; private set; }

		public string Avatar { get; private set; }

		public async Task LoadPost(int idpost, bool isPreview)
		{
			IsLoading = true;
			var temp = await Context.GetPostAsync(idpost);

			// Cas ou le post n'est pas publié
			// et n'est pas une vue en preview
			if (!temp.IsPublished && !isPreview)
			{
				Article = null;
				IsLoading = false;
				return; 
			}

			temp.Image = Settings.GetUrlImagePost(temp.Image);

			Article = temp;
			Categories = await Context.GetCategories(idpost);

			MetaKeywords = Categories.Select(c => c.Nom).Aggregate((a, b) => a + ", " + b);
			IEnumerable<char> descr = Article.Content.Take(130);
			MetaDescription = string.Join("", descr);

			if (!string.IsNullOrEmpty(Settings.BlogUrl))
			{
				string urlPost = Settings.BlogUrl + "post/" + idpost;
				UrlShareLinkedIn = "https://www.linkedin.com/shareArticle?mini=true&url=" + urlPost;
				UrlShareTwitter = "https://twitter.com/intent/tweet?url=" + urlPost;

				DisabledShare = false;
			}

			Avatar = await Context.GetProfilImage(Article.UserId);
			IsLoading = false;
		}

		public void OpenCategoriePosts(int idCategorie)
		{
			NavigationManager.NavigateTo($"/categorie/{idCategorie}");
		}

		#endregion

	}
}
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorBlog.ViewModels
{
	public class ArticlesViewModel : IArticlesViewModel
	{
		private readonly BlogContext Context;
		private readonly string UserId;
		private readonly ISnackbar snack;
		private readonly NavigationManager Nav;

		public ArticlesViewModel(BlogContext blogContext, IHttpContextAccessor httpContextAccessor, ISnackbar snackbar,
								NavigationManager navigationManager)
		{
			Context = blogContext;
			snack = snackbar;
			UserId = httpContextAccessor.HttpContext.User.Identity.Name;
			Nav = navigationManager;
			AllPosts = new List<Post>();
		}

		#region IArticlesViewModel
		
		public List<Post> AllPosts { get; private set; }
		

		public async Task GetArticles()
		{
			try
			{
				AllPosts = await Context.GetPostsAsync(UserId);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error GetArticles");
				snack.Add("Erreur sur le chargement des articles", Severity.Error);
			}
		}


		public async Task DeletePost(int idPost)
		{
			try
			{
				await Context.DeletePostAsync(idPost);
				snack.Add("Article supprimé", Severity.Success);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error DeletePost");
				snack.Add("Erreur sur la suppression de l'article", Severity.Error);
			}
		}


		public async Task EditPost(int idPost)
		{
			Nav.NavigateTo($"/editpost/{idPost}", true);
		}

		#endregion

	}
}

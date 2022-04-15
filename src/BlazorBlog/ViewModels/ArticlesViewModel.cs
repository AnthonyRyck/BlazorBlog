using BlazorBlog.Composants;
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
		private readonly IDialogService DialogSvc;

		public ArticlesViewModel(BlogContext blogContext, IHttpContextAccessor httpContextAccessor, ISnackbar snackbar,
								NavigationManager navigationManager, IDialogService dialogService)
		{
			Context = blogContext;
			snack = snackbar;
			UserId = httpContextAccessor.HttpContext.User.Identity.Name;
			Nav = navigationManager;
			AllPosts = new List<Post>();
			DialogSvc = dialogService;
			FiltrerPost = Recherche;
			PostRecherche = String.Empty;
		}

		private bool Recherche(Post post)
		{
			if (string.IsNullOrEmpty(PostRecherche))
				return true;

			return post.Title.Contains(PostRecherche, StringComparison.OrdinalIgnoreCase);
		}

		#region IArticlesViewModel

		public List<Post> AllPosts { get; private set; }
		
		public Func<Post, bool> FiltrerPost { get; private set; }
		public string PostRecherche { get; set; }

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
				var parameters = new DialogParameters();
				parameters.Add("ContentText", "Etes vous sur de vouloir supprimer cet article ? Pas de retour en arrière possible.");
				parameters.Add("ButtonText", "Supprimer");
				parameters.Add("Color", Color.Error);
				var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };

				var dialog = DialogSvc.Show<DialogTemplate>("Suppression", parameters, options);
				var result = await dialog.Result;
				if (!result.Cancelled)
				{
					await Context.DeletePostAsync(idPost);
					snack.Add("Article supprimé", Severity.Success);
				}
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

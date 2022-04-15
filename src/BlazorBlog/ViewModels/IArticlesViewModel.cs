namespace BlazorBlog.ViewModels
{
	public interface IArticlesViewModel
	{
		/// <summary>
		/// Tous les posts.
		/// </summary>
		List<Post> AllPosts { get; }

		/// <summary>
		/// Charge les articles de l'auteur
		/// </summary>
		/// <returns></returns>
		Task GetArticles();

		/// <summary>
		/// Post à supprimer
		/// </summary>
		/// <param name="idPost"></param>
		/// <returns></returns>
		Task DeletePost(int idPost);

		/// <summary>
		/// Edit le post
		/// </summary>
		/// <param name="idPost"></param>
		/// <returns></returns>
		Task EditPost(int idPost);

		/// <summary>
		/// Nom de post recherché
		/// </summary>
		string PostRecherche { get; set; }

		/// <summary>
		/// Méthode de recherche
		/// </summary>
		Func<Post, bool> FiltrerPost { get; }
	}
}

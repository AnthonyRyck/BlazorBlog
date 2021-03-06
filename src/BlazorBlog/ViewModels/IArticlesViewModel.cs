namespace BlazorBlog.ViewModels
{
	public interface IArticlesViewModel
	{
		/// <summary>
		/// Tous les posts.
		/// </summary>
		List<PostView> AllPosts { get; }

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
		Func<PostView, bool> FiltrerPost { get; }

		/// <summary>
		/// Ouvre le post dans un nouvel onglet
		/// </summary>
		/// <param name="idPost"></param>
		void OpenPostToRead(int idPost);

		/// <summary>
		/// Sauvegarde le post
		/// </summary>
		/// <param name="idPost"></param>
		/// <returns></returns>
		Task SaveThePost(int idPost);
	}
}

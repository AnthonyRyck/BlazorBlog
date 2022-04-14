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
	}
}

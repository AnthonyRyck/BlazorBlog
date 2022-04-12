namespace BlazorBlog.ViewModels
{
	public interface IIndexViewModel
	{
		/// <summary>
		/// Liste de tous les posts publiés.
		/// </summary>
		IEnumerable<Post> Posts { get; }

		/// <summary>
		/// Charge tous les posts publiés
		/// </summary>
		/// <returns></returns>
		Task GetAllPosts();

		/// <summary>
		/// Ouvre la page de l'article
		/// </summary>
		/// <param name="id"></param>
		void OpenPost(int id);
	}
}

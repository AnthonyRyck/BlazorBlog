namespace BlazorBlog.ViewModels
{
	public interface IIndexViewModel
	{
		/// <summary>
		/// Liste de tous les posts publiés.
		/// </summary>
		IEnumerable<Post> PostsToDisplay { get; }

		/// <summary>
		/// Catégorie sélectionnée pour voir les posts
		/// </summary>
		Categorie CategorieSelected { get; }

		/// <summary>
		/// Charge tous les posts publiés
		/// </summary>
		/// <returns></returns>
		Task GetAllPosts(int? idCategorie);

		/// <summary>
		/// Ouvre la page de l'article
		/// </summary>
		/// <param name="id"></param>
		void OpenPost(int id);

		/// <summary>
		/// Nombre de page
		/// </summary>
		int CounterPage { get; }

		/// <summary>
		/// Change l'affichage de la page
		/// </summary>
		/// <param name="page"></param>
		void PageChanged(int page);
	}
}

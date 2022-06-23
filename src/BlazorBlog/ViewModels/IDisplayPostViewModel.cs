namespace BlazorBlog.ViewModels
{
	public interface IDisplayPostViewModel
	{
		/// <summary>
		/// Indique un chargement du post
		/// </summary>
		bool IsLoading { get; }

		/// <summary>
		/// C'est tout le post
		/// </summary>
		Post Article { get; }

		/// <summary>
		/// Charge le post demandé.
		/// </summary>
		/// <param name="idpost"></param>
		/// <param name="isPreview"></param>
		/// <returns></returns>
		Task LoadPost(int idpost, bool isPreview);

		/// <summary>
		/// URL pour partager sur LinkedIn
		/// </summary>
		string UrlShareLinkedIn { get; }

		/// <summary>
		/// URL pour partager sur Twitter
		/// </summary>
		string UrlShareTwitter { get; }

		/// <summary>
		/// Indique si le partage est possible.
		/// </summary>
		bool DisabledShare { get; }

		/// <summary>
		/// Avatar de l'auteur.
		/// </summary>
		string Avatar { get; }

		/// <summary>
		/// Mots clé pour les metadonnées.
		/// </summary>
		string MetaKeywords { get; }

		/// <summary>
		/// Description pour les metadonnées.
		/// </summary>
		string MetaDescription { get; }

		/// <summary>
		/// Liste toutes les catégories pour ce post.
		/// </summary>
		IEnumerable<Categorie> Categories { get; }

		/// <summary>
		/// Ouvre la page des posts pour cette catégorie.
		/// </summary>
		/// <param name="idCategorie"></param>
		void OpenCategoriePosts(int idCategorie);
	}
}
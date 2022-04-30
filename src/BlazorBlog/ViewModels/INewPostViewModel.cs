using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public interface INewPostViewModel
	{
		/// <summary>
		/// Entité de validation d'un post.
		/// </summary>
		PostValidation ValidationPost { get; set; }

		/// <summary>
		/// Permet la validation du Forms
		/// </summary>
		EditContext EditContextValidation { get; set; }

		/// <summary>
		/// Entité de validation pour une catégorie
		/// </summary>
		CategorieValidation ValidationCategorie { get; set; }

		/// <summary>
		/// Permet la validation du Forms
		/// </summary>
		EditContext EditCtxCategorie { get; set; }

		/// <summary>
		/// Image mis en avant.
		/// </summary>
		string ImageEnAvant { get; }

		/// <summary>
		/// Indication si le post est sauvegardé.
		/// </summary>
		bool PublishDisabled { get; }

		/// <summary>
		/// Liste des catégories pour le post
		/// </summary>
		List<Categorie> Categories {get; }

		/// <summary>
		/// Ajoute une nouvelle catégorie
		/// </summary>
		/// <returns></returns>
		Task AjouterCategorie();

		/// <summary>
		/// Sauvegarde le post ou ses changements
		/// </summary>
		/// <returns></returns>
		Task SavePost();

		/// <summary>
		/// Publie le post, le met en public
		/// </summary>
		/// <returns></returns>
		Task PublishPost();

		/// <summary>
		/// Ouvre le Dialog pour voir la galerie d'images
		/// </summary>
		/// <returns></returns>
		Task OpenGalerie();

		/// <summary>
		/// Ouvre un onglet pour voir une preview du post
		/// </summary>
		/// <returns></returns>
		Task OpenPreview();
	}
}

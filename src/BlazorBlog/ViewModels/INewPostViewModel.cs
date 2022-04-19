using BlazorBlog.ModelsValidation;
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
		/// Image mis en avant.
		/// </summary>
		string ImageEnAvant { get; }

		/// <summary>
		/// Indication si le post est sauvegardé.
		/// </summary>
		bool PublishDisabled { get; }

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
	}
}

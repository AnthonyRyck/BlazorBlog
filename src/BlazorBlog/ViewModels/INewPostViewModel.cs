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
		/// Pour la validation d'un post
		/// </summary>
		Task OnValidSubmitPost();

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
		/// Permet au ViewModel d'indiquer un changement.
		/// </summary>
		void SetStateHasChanged(Action state);
	}
}

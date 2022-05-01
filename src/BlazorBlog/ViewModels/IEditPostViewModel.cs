using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public interface IEditPostViewModel
	{

		EditContext EditContextValidation { get; set; }
		string ImageEnAvant { get; }
		PostValidation ValidationPost { get; set; }

		List<Categorie> Categories { get; }
		CategorieValidation ValidationCategorie { get; set; }
		EditContext EditCtxCategorie { get; set; }

		Task OpenGalerie();
		Task PublishPost();
		Task SavePost();

		/// <summary>
		/// Charge le post pour l'édition
		/// </summary>
		/// <param name="idpost"></param>
		/// <returns></returns>
		Task LoadPost(int idpost);

		Task AjouterCategorie();

		/// <summary>
		/// Ouvre un onglet pour voir une preview du post
		/// </summary>
		/// <returns></returns>
		Task OpenPreview();


		Task AddImage(IBrowserFile file);

		string ImageUploaded { get; set; }
	}
}

using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public interface IGalerieViewModel
	{
		List<string> PathImages { get; }

		List<string> ImagesToDisplay { get; }

		Task OnInputFileChanged(InputFileChangeEventArgs e);

		void SetExtensions(string extensions);

		/// <summary>
		/// Nombre de page
		/// </summary>
		int CounterPage { get; }

		/// <summary>
		/// Change l'affichage de la page
		/// </summary>
		/// <param name="page"></param>
		void PageChanged(int page);

		string ImageRecherche { get; set; }
	}
}

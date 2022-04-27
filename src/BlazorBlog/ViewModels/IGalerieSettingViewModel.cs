namespace BlazorBlog.ViewModels
{
	public interface IGalerieSettingViewModel
	{
		List<string> PathImages { get; }

		List<string> ImagesToDisplay { get; }

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

		void LoadImages();


	}
}

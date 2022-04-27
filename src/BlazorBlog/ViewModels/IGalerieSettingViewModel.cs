namespace BlazorBlog.ViewModels
{
	public interface IGalerieSettingViewModel
	{
		List<ImageSetting> PathImages { get; }

		List<ImageSetting> ImagesToDisplay { get; }

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

		Task LoadImages();
		
		/// <summary>
		/// Indique la sélection de l'image.
		/// </summary>
		/// <param name="image"></param>
		void SelectImage(ImageSetting image);
	}
}

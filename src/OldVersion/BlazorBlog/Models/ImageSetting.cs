namespace BlazorBlog.Models
{
	public class ImageSetting
	{	
		/// <summary>
		/// Chemin de l'image
		/// </summary>
		public string UrlImage { get; set; }

		/// <summary>
		/// Nom du fichier de l'image
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Nombre d'utilisation de cette image dans les posts
		/// </summary>
		public int CounterUse { get; set; }

		/// <summary>
		/// Indicateur si l'image est sélectionné
		/// </summary>
		public bool IsSelected { get; set; }

		/// <summary>
		/// Donne le style CSS si l'image est sélectionnée ou non
		/// </summary>
		public string BorderSelection { get { return IsSelected ? "border-2-px-solid-red" : "border-1-px-solid-black"; } }

		public Color Couleur { get { return CounterUse > 0 ? Color.Primary : Color.Default; } }
	}
}

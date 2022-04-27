namespace BlazorBlog.Models
{
	public class ImageSetting
	{	
		/// <summary>
		/// Chemin de l'image
		/// </summary>
		public string UrlImage { get; set; }

		/// <summary>
		/// Nombre d'utilisation de cette image dans les posts
		/// </summary>
		public int CounterUse { get; set; }
	}
}

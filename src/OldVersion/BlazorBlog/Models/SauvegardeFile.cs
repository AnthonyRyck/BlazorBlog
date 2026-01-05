namespace BlazorBlog.Models
{
	public class SauvegardeFile
	{
		public string FileName { get; set; }

		public DateTime Created { get; set; }

		public long Size { get; set; }

		public string Taille { get { return  Helpers.GetSize(Size); } }
	}
}

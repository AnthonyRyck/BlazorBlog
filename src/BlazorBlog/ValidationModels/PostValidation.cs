using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.ValidationModels
{
	public class PostValidation
	{
		[Required(ErrorMessage = "Il faut un titre")]
		[MaxLength(100, ErrorMessage ="Maximum 100 caractères")]
		public string Titre { get; set; }

		public string Content { get; set; }

		[Required(ErrorMessage = "Il faut une image de mise en avant")]
		public string Image { get; internal set; }

		public bool Published { get; set; }
	}
}

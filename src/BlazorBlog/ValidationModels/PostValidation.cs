using BlazorBlog.Core;
using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.ModelsValidation
{
	public class PostValidation
	{
		[Required(ErrorMessage = "Il faut un titre")]
		[MaxLength(30, ErrorMessage ="Maximum 30 caractères")]
		public string Titre { get; set; }

		public string Content { get; set; }

		[Required(ErrorMessage = "Il faut une image de mise en avant")]
		public string Image { get; internal set; }

		public bool Published { get; set; }
	}


	public static class PostValidationExtension
	{
		public static Post ToPost(this PostValidation source, string userId)
		{
			return new Post()
			{
				Content = source.Content,
				UpdatedAt = DateTime.Now,
				Title = source.Titre,
				UserId = userId,
				Image = source.Image
			};
		}
	}
}

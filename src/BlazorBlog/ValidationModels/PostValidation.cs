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
	}


	public static class PostValidationExtension
	{
		public static Post ToPost(this PostValidation source, Guid userId)
		{
			return new Post()
			{
				Content = source.Content,
				UpdatedAt = DateTime.Now,
				Title = source.Titre,
				UserId = userId
			};
		}
	}
}

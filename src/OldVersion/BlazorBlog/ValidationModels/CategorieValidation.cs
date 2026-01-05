using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.ValidationModels
{
	public class CategorieValidation
	{
		[Required(ErrorMessage = "Il faut un nom de catégorie")]
		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string Nom { get; set; }
	}
}

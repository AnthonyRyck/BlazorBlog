using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.ValidationModels
{
	public class SettingsValidation
	{
		[Required(ErrorMessage = "Il faut un nom de Blog")]
		[MaxLength(50, ErrorMessage = "Maximum 50 caractères")]
		public string BlogName { get; set; }

		[MaxLength(100, ErrorMessage = "Maximum 100 caractères")]
		public string BlogDescription { get; set; }

		[Required(ErrorMessage = "Indiquer l'URL du site")]
		[MaxLength(250, ErrorMessage = "Maximum 250 caractères")]
		public string BlogUrl { get; set; }

		public string BlogImage { get; set; }
	}
}

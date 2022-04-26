using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public interface IGalerieViewModel
	{
		List<string> PathImages { get; }

		Task OnInputFileChanged(InputFileChangeEventArgs e);

		void SetExtensions(string extensions);
	}
}

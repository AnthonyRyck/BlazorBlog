using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.Services
{
	public interface IServiceImage
	{
		Task<string> SaveImage(IBrowserFile imgFile);
	}
}

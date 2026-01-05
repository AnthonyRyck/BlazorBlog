using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.Services
{
	public class ImageService : IServiceImage
	{
		private readonly string UserName;
		private readonly string PathImagesUser;


		public ImageService(IHttpContextAccessor httpContextAccessor)
		{
			UserName = httpContextAccessor.HttpContext.User.Identity.Name;
			PathImagesUser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, UserName);
		}


		public async Task<string> SaveImage(IBrowserFile imgFile)
		{
			string pathImage = Path.Combine(PathImagesUser, imgFile.Name);

			try
			{
				string extensionFile = new FileInfo(imgFile.Name).Extension.ToLower();

				if (extensionFile == ConstantesApp.EXTENSION_IMAGE_JPEG
					|| extensionFile == ConstantesApp.EXTENSION_IMAGE_JPG
					|| extensionFile == ConstantesApp.EXTENSION_IMAGE_GIF
					|| extensionFile == ConstantesApp.EXTENSION_IMAGE_PNG
					|| extensionFile == ConstantesApp.EXTENSION_IMAGE_ICO)
				{
					using FileStream fs = new(pathImage, FileMode.Create);
					// Max 3 Mo
					await imgFile.OpenReadStream(3000000).CopyToAsync(fs);

					return SetUrlImageName(imgFile.Name);
				}
				else
				{
					return "NOT_GOOD_EXTENSION";
				}
			}
			catch (Exception ex)
			{
				if (File.Exists(pathImage))
				{
					File.Delete(pathImage);
				}
				Log.Error(ex, "Error OnInputFileChanged");

				throw;
			}
		}

		private string SetUrlImageName(string imageName)
		{
			return $"..{ConstantesApp.USERIMG}/{UserName}/{imageName}";
		}
	}
}

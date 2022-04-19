using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace BlazorBlog.ViewModels
{
	public class GalerieViewModel : IGalerieViewModel
	{
		private readonly string UserName;
		private readonly string PathImagesUser;
        private readonly ISnackbar Snack;

        public GalerieViewModel(IHttpContextAccessor httpContextAccessor, ISnackbar snackbar)
		{
			UserName = httpContextAccessor.HttpContext.User.Identity.Name;
			PathImagesUser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, UserName);
            PathImages = GetFilesFromPath(PathImagesUser);

			Snack = snackbar;
		}

        private List<string> GetFilesFromPath(string path)
        {
            List<string> files = new List<string>();

            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] filesInfo = di.GetFiles();
                
                foreach (FileInfo file in filesInfo)
				{
					files.Add(SetUrlImageName(file.Name));
				}
			}
            catch (Exception ex)
            {
                Log.Error(ex, "Error GetFilesFromPath");
                Snack.Add("Erreur sur le chargement des images", Severity.Error);
            }

            return files;
        }

		private string SetUrlImageName(string imageName)
		{
			return $"..{ConstantesApp.USERIMG}/{UserName}/{imageName}";
		}

		#region IGalerieViewModel

		public List<string> PathImages { get; private set; } = new List<string>();


		public async Task OnInputFileChanged(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles(1);
            
            foreach (var file in files)
            {
                string pathImage = Path.Combine(PathImagesUser, file.Name);

                try
                {
                    using FileStream fs = new(pathImage, FileMode.Create);
                    // Max 3 Mo
                    await file.OpenReadStream(3000000).CopyToAsync(fs);

                    Snack.Add($"Upload de {file.Name} réussi", Severity.Success);
                    PathImages.Add(SetUrlImageName(file.Name));
                }
                catch (Exception ex)
                {
					if (File.Exists(pathImage))
					{
						File.Delete(pathImage);
					}
                    Log.Error(ex, "Error OnInputFileChanged");
                    Snack.Add("Erreur lors de l'upload de l'image - Max 3 Mo", Severity.Error);
				}
            }
        }

        #endregion
    }
}

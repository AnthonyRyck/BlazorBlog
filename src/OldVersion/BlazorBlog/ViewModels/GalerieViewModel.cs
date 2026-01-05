using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public class GalerieViewModel : IGalerieViewModel
	{
		private readonly string UserName;
		private readonly string PathImagesUser;
        private readonly ISnackbar Snack;
		private readonly IServiceImage ImageService;

		private const int PageSize = 10;

		public GalerieViewModel(IHttpContextAccessor httpContextAccessor, ISnackbar snackbar, IServiceImage imageService)
		{
            ImageService = imageService;
			UserName = httpContextAccessor.HttpContext.User.Identity.Name;
			PathImagesUser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, UserName);

			Snack = snackbar;
		}

        private IEnumerable<string> extensionsImage;

        public void SetExtensions(string extensions)
		{
			extensionsImage = extensions.Split(", ");
			PathImages = GetFilesFromPath(PathImagesUser);

			SetImageToDisplay(PathImages);
		}

		private void SetImageToDisplay(List<string> listImage)
		{
			double tempCounter = Convert.ToDouble(listImage.Count()) / PageSize;
			if ((tempCounter - Math.Truncate(tempCounter)) > 0)
				CounterPage = Convert.ToInt32(Math.Truncate(tempCounter)) + 1;
			else
				CounterPage = Convert.ToInt32(Math.Truncate(tempCounter));

            int end = 0;
            if (listImage.Count < PageSize)
                end = listImage.Count;
            else
                end = PageSize;

            ImagesToDisplay = listImage.GetRange(0, end);
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
                    if(extensionsImage.Contains(file.Extension))
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

        private List<string> SelectImage(int page, List<string> listeImg)
        {
            int start = (page - 1) * PageSize;

            if ((start + PageSize) > listeImg.Count)
                return listeImg.GetRange(start, listeImg.Count - start);

            return listeImg.GetRange(start, PageSize);
        }
        
        public List<string> PathImages { get; private set; } = new List<string>();

        #region IGalerieViewModel

		public List<string> ImagesToDisplay { get; private set; } = new List<string>();

        public int CounterPage { get; private set; }

        private string _imageRecherche;
        public string ImageRecherche
        {
            get { return _imageRecherche; }
            set { _imageRecherche = value;
                RechecherImage();
             }
        }

		private void RechecherImage()
		{
            ImageRecheche = PathImages.Where(result => result.ToUpper().Contains(_imageRecherche.ToUpper()))
                           .ToList();

            SetImageToDisplay(ImageRecheche);
        }

        public async Task OnInputFileChanged(InputFileChangeEventArgs e)
        {
            var files = e.GetMultipleFiles(1);

            foreach (var file in files)
            {
                try
                {
                    string imgUrl = await ImageService.SaveImage(file);

				    if (imgUrl != "NOT_GOOD_EXTENSION")
				    {
                        Snack.Add($"Upload de {file.Name} réussi", Severity.Success);

                        // Pour qu'il soit connu dans le composant
                        PathImages.Add(imgUrl);
                        // Pour qu'il soit affiché
                        ImagesToDisplay.Add(imgUrl);						
                    }
                    else
				    {
                        Snack.Add($"Il faut une image JPG ou JPEG ou PNG ou ICO", Severity.Warning);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error OnInputFileChanged");
                    Snack.Add("Erreur lors de l'upload de l'image - Max 3 Mo", Severity.Error);
                }
            }
        }

        public void PageChanged(int page)
		{
            // Aucune recherche
            if (string.IsNullOrEmpty(_imageRecherche))
                ImagesToDisplay = SelectImage(page, PathImages);
            else
                ImagesToDisplay = SelectImage(page, ImageRecheche);
        }

        private List<string> ImageRecheche = new List<string>();

		#endregion
	}
}

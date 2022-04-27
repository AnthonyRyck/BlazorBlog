namespace BlazorBlog.ViewModels
{
	public class GalerieSettingViewModel : IGalerieSettingViewModel
    {
        private readonly string UserName;
        private readonly string PathImagesUser;
		private IEnumerable<string> extensionsImage;
		private const int PageSize = 10;
        private List<ImageSetting> ImageRecheche = new List<ImageSetting>();
        private readonly ISnackbar Snack;
		private BlogContext _blogContext;


		public GalerieSettingViewModel(IHttpContextAccessor httpContextAccessor, ISnackbar snackbar, BlogContext blogContext)
		{
			UserName = httpContextAccessor.HttpContext.User.Identity.Name;
			PathImagesUser = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, UserName);

			Snack = snackbar;
			_blogContext = blogContext;
			extensionsImage = new List<string> 
				{ 
					ConstantesApp.EXTENSION_IMAGE_GIF, 
					ConstantesApp.EXTENSION_IMAGE_JPG, 
					ConstantesApp.EXTENSION_IMAGE_PNG,
					ConstantesApp.EXTENSION_IMAGE_BMP
				};
		}

		private async Task<List<ImageSetting>> GetFilesFromPath(string path)
		{
			List<ImageSetting> files = new List<ImageSetting>();

			try
			{
				DirectoryInfo di = new DirectoryInfo(path);
				FileInfo[] filesInfo = di.GetFiles();

				foreach (FileInfo file in filesInfo)
				{
					if (extensionsImage.Contains(file.Extension))
					{
						int counter = await _blogContext.GetCounterImage(file.Name);

						string url = SetUrlImageName(file.Name);
						ImageSetting imageSetting = new ImageSetting()
						{
							UrlImage = url,
							CounterUse = counter
						};

						files.Add(imageSetting);
					}
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

		private void SetImageToDisplay(List<ImageSetting> listImage)
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

		private void RechecherImage()
		{
			ImageRecheche = PathImages.Where(result => result.UrlImage.ToUpper().Contains(_imageRecherche.ToUpper()))
						   .ToList();

			SetImageToDisplay(ImageRecheche);
		}

		private List<ImageSetting> SelectImage(int page, List<ImageSetting> listeImg)
		{
			int start = (page - 1) * PageSize;

			if ((start + PageSize) > listeImg.Count)
				return listeImg.GetRange(start, listeImg.Count - start);

			return listeImg.GetRange(start, PageSize);
		}

		#region IGalerieSettingViewModel


		public List<ImageSetting> PathImages { get; private set; } = new List<ImageSetting>();

		public List<ImageSetting> ImagesToDisplay { get; private set; } = new List<ImageSetting>();

		public int CounterPage { get; private set; }

		private string _imageRecherche;
		public string ImageRecherche
		{
			get { return _imageRecherche; }
			set
			{
				_imageRecherche = value;
				RechecherImage();
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

		public async Task LoadImages()
		{
			PathImages = await GetFilesFromPath(PathImagesUser);
			SetImageToDisplay(PathImages);
		}

		#endregion
	}
}

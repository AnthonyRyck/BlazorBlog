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
						
						string url = SetUrlImageName(file.Name);
						int counter = await _blogContext.GetCounterImage(url);

						ImageSetting imageSetting = new ImageSetting()
						{
							FileName = file.Name,
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

		public List<ImageSetting> ImagesSelectedToDelete { get; private set; } = new List<ImageSetting>();

		public bool IsLoading { get; private set; } = true;

		public int CounterPage { get; private set; }

		private string _imageRecherche;
		private Action StateChanged;

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

			IsLoading = false;
			StateChanged?.Invoke();
		}

		public void SelectImage(ImageSetting image)
		{
			var img = ImagesToDisplay.FirstOrDefault(x => x.UrlImage == image.UrlImage);
			if(img != null)
			{
				img.IsSelected = !img.IsSelected;

				if(img.IsSelected)
					ImagesSelectedToDelete.Add(img);
				else ImagesSelectedToDelete.Remove(img);
			}

			var imgSelected = PathImages.FirstOrDefault(x => x.UrlImage == img.UrlImage);
			if(img == null && imgSelected != null)
			{
				// Si déjà mis à jour dans ImageToDisplay, pas la peine de mettre à
				// jour dans PathImages --> passage par référence.
				imgSelected.IsSelected = !imgSelected.IsSelected;

				if (imgSelected.IsSelected)
					ImagesSelectedToDelete.Add(imgSelected);
				else ImagesSelectedToDelete.Remove(imgSelected);
			}
		}

		public void DeleteImage()
		{
			foreach (var img in PathImages.Where(x => x.IsSelected))
			{
				string imageToDelete = Path.Combine(PathImagesUser, img.FileName);
				File.Delete(imageToDelete);
			}

			PathImages.RemoveAll(x => x.IsSelected);
			Snack.Add($"Suppression de {ImagesSelectedToDelete.Count} images", Severity.Success);
			ImagesSelectedToDelete = new List<ImageSetting>();

			SetImageToDisplay(PathImages);
		}

		public void ResetSelection()
		{
			ImagesSelectedToDelete = new List<ImageSetting>();
			var imgToDisplayReset = ImagesToDisplay.Where(x => x.IsSelected);
			foreach (var item in imgToDisplayReset)
			{
				item.IsSelected = false;
			}

			var imgTotal = PathImages.Where(x => x.IsSelected);
			foreach (var item in imgTotal)
			{
				item.IsSelected=false;
			}
		}


		public void SetStateChanged(Action stateHasChanged)
		{
			StateChanged = stateHasChanged;
		}

		#endregion
	}
}

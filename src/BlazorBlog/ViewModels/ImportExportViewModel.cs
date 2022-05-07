using BlazorBlog.Composants;
using BlazorDownloadFile;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace BlazorBlog.ViewModels
{
	public class ImportExportViewModel : IImportExportViewModel
	{
		private readonly BlogContext Context;
		private readonly ISnackbar Snackbar;
		private readonly IDialogService SvcDialog;
		private readonly IBlazorDownloadFileService downloadSvc;
		private string PathImages;
		

		public ImportExportViewModel(BlogContext blogContext, ISnackbar snackbar, IDialogService dialogSvc,
									IBlazorDownloadFileService svcDownload)
		{
			Context = blogContext;
			Snackbar = snackbar;
			SvcDialog = dialogSvc;
			downloadSvc = svcDownload;
			PathImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES);
		}


		public async Task InitAsync()
		{
			var saveFiles = Directory.GetFiles(PathImages, "*.zip", SearchOption.TopDirectoryOnly);

			List<SauvegardeFile> temp = new List<SauvegardeFile>();
			foreach (var item in saveFiles)
			{
				FileInfo fileInfo = new FileInfo(item);

				SauvegardeFile save = new SauvegardeFile()
				{
					FileName = fileInfo.Name,
					Created = fileInfo.CreationTime,
					Size = fileInfo.Length
				};

				temp.Add(save);
			}

			Sauvegardes = temp.OrderBy(x => x.Created).ToList();
		}

		#region Implement IImportExportViewModel

		public List<SauvegardeFile> Sauvegardes { get; private set; } = new List<SauvegardeFile>();


		public async Task ExportDatabase()
		{
			// Créer un zip de la base de données et du répertoire Image.
			string dateToday = DateTime.Now.ToString("dd-MM-yyyy");
			string pathZip = Path.Combine(PathImages, $"export-{dateToday}.zip");

			if (File.Exists(pathZip))
			{
				var parameters = new DialogParameters();
				parameters.Add("ContentText", "Un fichier de sauvegarde à ce jour existe déjà, l'écraser pour en faire un nouveau ?");
				parameters.Add("ButtonText", "Ecraser");
				parameters.Add("Color", Color.Error);

				var opt = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
				var dialog = SvcDialog.Show<DialogTemplate>("Attention", parameters, opt);
				var result = await dialog.Result;

				if (result.Cancelled)
				{
					return;
				}
				
				File.Delete(pathZip);
			}

			try
			{
				JsonSerializerOptions options = new()
				{
					WriteIndented = true,
					Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
				};

				//Charger tous les items de bdd
				List<Post> allPosts = await Context.GetAllPostsAsync();
				string jsonPosts = JsonSerializer.Serialize(allPosts, options);

				List<Categorie> allCategories = await Context.GetCategories();
				string jsonCategories = JsonSerializer.Serialize(allCategories, options);

				List<CategorieToPost> categoriesPosts = await Context.GetCategoriesPosts();
				string jsonCategoriesToPost = JsonSerializer.Serialize(categoriesPosts, options);

				List<Settings> allSettings = await Context.GetSettings();
				string jsonSettings = JsonSerializer.Serialize(allSettings, options);

				using (FileStream zipToOpen = new FileStream(pathZip, FileMode.OpenOrCreate))
			{
				using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
				{
					ZipArchiveEntry postsEntry = archive.CreateEntry(ConstantesApp.EXPORT_POSTS);
					using (StreamWriter writer = new StreamWriter(postsEntry.Open()))
					{
						writer.Write(jsonPosts);
					}

					ZipArchiveEntry categoriesEntry = archive.CreateEntry(ConstantesApp.EXPORT_CATEGORIES);
					using (StreamWriter writer = new StreamWriter(categoriesEntry.Open()))
					{
						writer.Write(jsonCategories);
					}

					ZipArchiveEntry categoriesToPost = archive.CreateEntry(ConstantesApp.EXPORT_CATEGORIES_TO_POSTS);
					using (StreamWriter writer = new StreamWriter(categoriesToPost.Open()))
					{
						writer.Write(jsonCategoriesToPost);
					}

					ZipArchiveEntry settingsEntry = archive.CreateEntry(ConstantesApp.EXPORT_SETTINGS);
					using (StreamWriter writer = new StreamWriter(settingsEntry.Open()))
					{
						writer.Write(jsonSettings);
					}

					// Ajouter les images
					string pathImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES);
					var repUsers = Directory.GetDirectories(pathImages, "*", SearchOption.TopDirectoryOnly);

					foreach (var repUser in repUsers)
					{
						var nameRepUser = repUser.Replace(Path.GetDirectoryName(repUser) + @"\", string.Empty);

						foreach (string file in Directory.EnumerateFiles(repUser))
						{
							string nameImgFile = Path.GetFileName(file);
							string fileImg = Path.Combine(nameRepUser, nameImgFile);
							
							ZipArchiveEntry entry = archive.CreateEntry(fileImg);
							using (Stream stream = entry.Open())
							{
								using (FileStream fs = new FileStream(file, FileMode.Open))
								{
									fs.CopyTo(stream);
								}
							}
						}
					}					
				}
			}

				FileInfo fileInfo = new FileInfo(pathZip);
				SauvegardeFile save = new SauvegardeFile()
				{
					FileName = fileInfo.Name,
					Created = fileInfo.CreationTime,
					Size = fileInfo.Length
				};
				Sauvegardes.Add(save);
				Snackbar.Add("Sauvegarde effectuée avec succès !", Severity.Success);
			}
			catch (Exception ex)
			{
				string message = "Erreur sur création de la sauvegarde";
				Log.Error(ex, message);
				Snackbar.Add(message, Severity.Error);
			}
		}

		public async Task Delete(SauvegardeFile file)
		{
			try
			{
				string fileToDelete = Path.Combine(PathImages, file.FileName);
				await Task.Run(() => File.Delete(fileToDelete));
				Sauvegardes.Remove(file);
				Snackbar.Add("Sauvegarde supprimée avec succès !", Severity.Success);
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Erreur lors de la suppression de la sauvegarde - {file.FileName}");
				Snackbar.Add("Erreur lors de la suppression de la sauvegarde", Severity.Error);
			}			
		}

		public async Task Download(SauvegardeFile file)
		{
			try
			{
				string pathToDownload = Path.Combine(PathImages, file.FileName);
				byte[] content = File.ReadAllBytes(pathToDownload);
				await downloadSvc.DownloadFile(file.FileName, content, "application/octet-stream");
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Erreur lors du téléchargement de la sauvegarde - {file.FileName}");
				Snackbar.Add("Erreur lors du téléchargement de la sauvegarde", Severity.Error);
			}
		}

		#endregion
	}
}

using BlazorBlog.Composants;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
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
		private Action StateChanged;
		private readonly SettingsSvc setting;
		private readonly UserManager<IdentityUser> UserManager;

		public ImportExportViewModel(BlogContext blogContext, ISnackbar snackbar, IDialogService dialogSvc,
									IBlazorDownloadFileService svcDownload, SettingsSvc settingsSvc, UserManager<IdentityUser> userManager)
		{
			Context = blogContext;
			Snackbar = snackbar;
			SvcDialog = dialogSvc;
			downloadSvc = svcDownload;
			setting = settingsSvc;
			UserManager = userManager;
			PathImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES);
		}

		private async Task<T> GetEntry<T>(ZipArchive archive, string nameEntry)
		{
			T result = default(T);
			try
			{
				ZipArchiveEntry entry = archive.GetEntry(nameEntry);
				using (var categorieStream = new StreamReader(entry.Open(), Encoding.UTF8))
				{
					result = await JsonSerializer.DeserializeAsync<T>(categorieStream.BaseStream);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return result;
		}

		#region Implement IImportExportViewModel

		public async Task InitAsync(Action stateChanged)
		{
			StateChanged = stateChanged;
			await Task.Run(() => 
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
				InLoading = false;
			});
		}

		public List<SauvegardeFile> Sauvegardes { get; private set; } = new List<SauvegardeFile>();

		public bool InLoading { get; private set; } = true;

		public bool InUploadFile { get; private set; } = false;

		public double ProgressUpload { get; private set; } = 0;

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

				var users = await UserManager.GetUsersInRoleAsync(ConstantesApp.ROLE_AUTEUR);
				List<Auteur> allUsers = users.Select(x => new Auteur() { Email= x.Email, UserName = x.UserName }).ToList();
				string jsonUsers = JsonSerializer.Serialize(allUsers, options);

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

						ZipArchiveEntry usersEntry = archive.CreateEntry(ConstantesApp.EXPORT_USERS);
						using (StreamWriter writer = new StreamWriter(usersEntry.Open()))
						{
							writer.Write(jsonUsers);
						}

						// Ajouter les images
						string pathImages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES);
						Log.Information($"EXPORT - Repertoire des images : {pathImages}");
						var repUsers = Directory.GetDirectories(pathImages, "*", SearchOption.TopDirectoryOnly);
						Log.Information($"EXPORT - Nombre de répertoire : {repUsers.Length}");

						foreach (var repUser in repUsers)
						{
							Log.Information($"EXPORT - Répertoire {repUser}");

							// TODO : Voir ici !
							// Juste pour avoir le nom du répertoire utiliser, sans tout le chemin avant.
							var nameRepUser = repUser.Replace(Path.GetDirectoryName(repUser) + Path.DirectorySeparatorChar, string.Empty);
							Log.Information($"EXPORT - Nom du répertoire de l'utilisateur : {nameRepUser}");

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
										Log.Information($"EXPORT - Ajout de l'image {fileImg}");
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
				Snackbar.Add("EXPORT - Sauvegarde effectuée avec succès !", Severity.Success);
			}
			catch (Exception ex)
			{
				string message = "EXPORT - Erreur sur création de la sauvegarde";
				Log.Error(ex, message);
				Snackbar.Add(message, Severity.Error);
			}
		}

		public async Task Delete(SauvegardeFile file)
		{
			try
			{
				var parameters = new DialogParameters();
				parameters.Add("ContentText", "Etes vous sûr de vouloir supprimer cette sauvegarde ?" 
							+ Environment.NewLine
							+ file.FileName);

				parameters.Add("ButtonText", "Supprimer");
				parameters.Add("Color", Color.Warning);
				
				var opt = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
				var dialog = SvcDialog.Show<DialogTemplate>("Attention", parameters, opt);
				var result = await dialog.Result;

				if (result.Cancelled)
				{
					return;
				}

				string fileToDelete = Path.Combine(PathImages, file.FileName);
				await Task.Run(() => File.Delete(fileToDelete));
				Sauvegardes.RemoveAll(x => x.FileName == file.FileName);
				StateChanged?.Invoke();
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

		public async Task ImportDatabase(InputFileChangeEventArgs e)
		{
			var files = e.GetMultipleFiles(1);
			InLoading = true;

			foreach (IBrowserFile file in files)
			{
				string pathZip = Path.Combine(PathImages, file.Name);
				
				try
				{
					string extensionFile = new FileInfo(file.Name).Extension.ToLower();

					if (extensionFile == ".zip")
					{
						
						if (File.Exists(pathZip))
						{
							var parameters = new DialogParameters();
							parameters.Add("ContentText", "Un fichier de sauvegarde à ce jour existe déjà, le remplacer ?");
							parameters.Add("ButtonText", "Remplacer");
							parameters.Add("Color", Color.Error);

							var opt = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
							var dialog = SvcDialog.Show<DialogTemplate>("Attention", parameters, opt);
							var result = await dialog.Result;

							if (result.Cancelled)
							{
								return;
							}
							else
							{
								File.Delete(pathZip);
								Sauvegardes.RemoveAll(x => x.FileName == file.Name);
							}
						}

						InUploadFile = true;

						byte[] buffer = System.Buffers.ArrayPool<byte>.Shared.Rent(4096);
						
						using (var stream = file.OpenReadStream(file.Size + 1000))
						{
							using (FileStream fs = new(pathZip, FileMode.Create))
							{
								long totalRead = 0;
								while (await stream.ReadAsync(buffer) is int read && read > 0)
								{
									totalRead += read;
									
									ProgressUpload = (double)totalRead / (double)file.Size * 100d;
									StateChanged?.Invoke();
									
									fs.Write(buffer, 0, read);
								}
							}
						}

						System.Buffers.ArrayPool<byte>.Shared.Return(buffer);

						FileInfo fileInfo = new FileInfo(pathZip);
						SauvegardeFile save = new SauvegardeFile()
						{
							FileName = fileInfo.Name,
							Created = fileInfo.CreationTime,
							Size = fileInfo.Length
						};
						Sauvegardes.Add(save);
						Snackbar.Add($"Upload de {file.Name} réussi", Severity.Success);
					}
					else
					{
						Snackbar.Add($"Il faut un fichier zip", Severity.Warning);
						return;
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex, "Error ImportDatabase");
					Snackbar.Add("Erreur lors de l'upload du fichier de sauvegarde", Severity.Error);
					File.Delete(pathZip);
				}
			}

			InUploadFile = false;
			InLoading = false;
			ProgressUpload = 0;
		}

		public async Task Restore(SauvegardeFile file)
		{
			var parameters = new DialogParameters();
			parameters.Add("ContentText", "Etes vous sûr de vouloir restorer cette sauvegarde ?"
						+ Environment.NewLine
						+ "Cela supprimera toutes les données (images, posts, utilisateurs,...) pour remettre à cette date.");

			parameters.Add("ButtonText", "Restaurer");
			parameters.Add("Color", Color.Warning);

			var opt = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
			var dialog = SvcDialog.Show<DialogTemplate>("Attention", parameters, opt);
			var result = await dialog.Result;

			if (result.Cancelled)
			{
				// Pas de restoration
				InLoading = false;
				return;
			}

			InLoading = true;
			ProgressUpload = 0;
			InUploadFile = true;
			StateChanged?.Invoke();

			List<Post> allPosts = new List<Post>();
			List<Categorie> allCategories = new List<Categorie>();
			List<CategorieToPost> categoriesPosts = new List<CategorieToPost>();
			List<Settings> allSettings = new List<Settings>();
			List<Auteur> allAuteurs = new List<Auteur>();

			// Dezip le fichier, et trouver les bons fichiers
			string pathZip = Path.Combine(PathImages, file.FileName);

			try
			{
				// Etape 1 : dézipper les fichiers
				using (ZipArchive archive = ZipFile.Open(pathZip, ZipArchiveMode.Read))
				{
					allCategories = await GetEntry<List<Categorie>>(archive, ConstantesApp.EXPORT_CATEGORIES);
					Log.Information("RESTORE - Zip Categories loaded");
					ProgressUpload = 8;
					StateChanged?.Invoke();

					allPosts = await GetEntry<List<Post>>(archive, ConstantesApp.EXPORT_POSTS);
					Log.Information("RESTORE - Zip Posts loaded");
					ProgressUpload = 17;
					StateChanged?.Invoke();

					categoriesPosts = await GetEntry<List<CategorieToPost>>(archive, ConstantesApp.EXPORT_CATEGORIES_TO_POSTS);
					Log.Information("RESTORE - Zip CategoriesToPosts loaded");
					ProgressUpload = 28;
					StateChanged?.Invoke();

					allSettings = await GetEntry<List<Settings>>(archive, ConstantesApp.EXPORT_SETTINGS);
					Log.Information("RESTORE - Zip Settings loaded");
					ProgressUpload = 37;
					StateChanged?.Invoke();

					allAuteurs = await GetEntry<List<Auteur>>(archive, ConstantesApp.EXPORT_USERS);
					Log.Information("RESTORE - Zip Users loaded");
					ProgressUpload = 46;
					StateChanged?.Invoke();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "RESTORE - Error fichier zip");
				Snackbar.Add("Erreur lors de la restoration.", Severity.Error);
				InLoading = false;
				InUploadFile = false;
				return;
			}

			// Etape 2 : Supprimer/Injecter dans la base de donnée.
			await Context.DeleteAllDataForRestore();
			Log.Information("RESTORE - Delete all data for restore");
			ProgressUpload = 55;
			StateChanged?.Invoke();

			// Ajouter les categories
			await Context.InsertRestore(allCategories, allPosts, categoriesPosts);
			Log.Information("RESTORE - Insert all data for restore");
			ProgressUpload = 64;
			StateChanged?.Invoke();

			await Context.AddDefaultSettings(allSettings);
			await setting.UpadateSettings(allSettings);
			Log.Information("RESTORE - Update settings");
			ProgressUpload = 73;
			StateChanged?.Invoke();

			// Etape 3 : Supprimer les répertoires image
			// et mettre les images
			try
			{
				var repUser = new DirectoryInfo(PathImages);
				repUser.GetDirectories().ToList().ForEach(x => x.Delete(true));
				Log.Information("RESTORE - Delete all images");

				using (ZipArchive archive = ZipFile.Open(pathZip, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						string extension = Path.GetExtension(entry.FullName);
						if (extension != ConstantesApp.EXPORT_EXTENSION)
						{
							var directory = Path.GetDirectoryName(entry.FullName);
							string repUserImg = Path.Combine(PathImages, directory);

							if (!Directory.Exists(repUserImg))
							{
								Directory.CreateDirectory(repUserImg);
								Log.Information($"RESTORE - Create directory {repUserImg}");
							}

							string path = Path.Combine(PathImages, entry.FullName);
							entry.ExtractToFile(path);
						}
					}
					Log.Information("RESTORE - Extract all images");
					ProgressUpload = 82;
					StateChanged?.Invoke();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "RESTORE - Error Restore Images");
				Snackbar.Add("Erreur lors de la restoration sur les Images.", Severity.Error);
				InLoading = false;
				InUploadFile = false;
				return;
			}

			try
			{
				// Etape 4 : Supprimer les auteurs
				List<IdentityUser> usersToDelete = new List<IdentityUser>();
				foreach (var user in UserManager.Users)
				{
					if (user.UserName != "root")
					{
						usersToDelete.Add(user);
					}
				}
				foreach (var user in usersToDelete)
				{
					await UserManager.DeleteAsync(user);
					Log.Information($"RESTORE - Delete user {user.UserName}");
				}

				Log.Information("RESTORE - Delete all users - OK");
				ProgressUpload = 91;
				StateChanged?.Invoke();

				// Etape 5 : Ajout des auteurs
				foreach (var user in allAuteurs)
				{
					var userToCreate = new IdentityUser() { UserName = user.UserName, Email = user.Email, EmailConfirmed = true };
					var resultUser = await UserManager.CreateAsync(userToCreate, "Azerty123!");

					if (resultUser.Succeeded)
					{
						Log.Information($"RESTORE - Create user {userToCreate.UserName}");
					}
					else
					{
						Log.Error($"RESTORE - Error create user {userToCreate.UserName}");
					}

					// Ajout des roles
					await UserManager.AddToRoleAsync(userToCreate, ConstantesApp.ROLE_AUTEUR);
					Log.Information($"RESTORE - Add role {ConstantesApp.ROLE_AUTEUR} to user {userToCreate.UserName}");
				}
				ProgressUpload = 100;
				StateChanged?.Invoke();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "RESTORE - Error Restore Users");
				Snackbar.Add("Erreur sur la création des auteurs.", Severity.Error);
				InLoading = false;
				InUploadFile = false;
				return;
			}
			
			InLoading = false;
			Snackbar.Add("Restoration réussie.", Severity.Success);
			InUploadFile = false;
			ProgressUpload = 0;

			var param = new DialogParameters();
			parameters.Add("ContentText", "Restoration terminé."
						+ Environment.NewLine
						+ "Note : Les mots de passe des auteurs ont été réinitilisés, nouveau MDP : Azerty123! " 
						+ Environment.NewLine
						+ "Le mot de passe de root n'a pas changé.");

			parameters.Add("ButtonText", "Ok");
			parameters.Add("Color", Color.Success);

			var optFin = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large };
			var dialogFin = SvcDialog.Show<DialogTemplate>("Restoration", parameters, opt);
		}

		#endregion
	}
}

using BlazorBlog.Composants;
using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Toolbelt.Blazor.HotKeys;

namespace BlazorBlog.ViewModels
{
	public class NewPostViewModel : INewPostViewModel, IDisposable
	{
		private readonly BlogContext ContextBlog;
		private Post PostEnCours;
		private readonly ISnackbar Snack;
		private readonly HotKeys KeysContext;
		private readonly IDialogService DialogService;
		private readonly DialogOptions FullScreenOption;
		private readonly string LoginUser;
		private readonly IJSRuntime JSRuntime;

		public NewPostViewModel(BlogContext blogContext, ISnackbar snackbar, HotKeys hotKeys, IDialogService dialogService, 
								IHttpContextAccessor httpContextAccessor, IJSRuntime js)
		{
			ContextBlog = blogContext;
			DialogService = dialogService;
			LoginUser = httpContextAccessor.HttpContext.User.Identity.Name;
			JSRuntime = js;

			FullScreenOption = new DialogOptions() { FullScreen = true, CloseButton = true };

			PostEnCours = new Post();
			PublishDisabled = true;
			
			ValidationPost = new PostValidation();
			EditContextValidation = new EditContext(ValidationPost);
			Snack = snackbar;
			KeysContext = hotKeys;
			KeysContext.CreateContext()
				.Add(ModKeys.Ctrl, Keys.S, SavePost, "Sauvegarde du post.", exclude: Exclude.ContentEditable);

			InitViewModel().GetAwaiter().GetResult();
			ValidationCategorie = new CategorieValidation();
			EditCtxCategorie = new EditContext(ValidationCategorie);
		}

		private async Task InitViewModel()
		{
			List<Categorie> temp = await ContextBlog.GetCategories();
			List<int> idCategoriesSelected = await ContextBlog.GetCategoriesByPost(PostEnCours.Id);

			foreach (var item in temp)
			{
				if (idCategoriesSelected.Contains(item.IdCategorie))
				{
					item.IsSelected = true;
				}
			}

			Categories = temp;
		}

		#region INewPostViewModel

		public PostValidation ValidationPost { get; set; }

		public EditContext EditContextValidation { get; set; }

		public bool PublishDisabled { get; private set; }

		public string ImageEnAvant { get; private set; }

		public List<Categorie> Categories { get; private set;  }
		
		public CategorieValidation ValidationCategorie { get; set; }
		
		public EditContext EditCtxCategorie { get; set; }

		public async Task SavePost()
		{
			if (!EditContextValidation.Validate())
			{
				return;
			}
			else
			{
				try
				{
					// Post pas encore sauvegardé en base
					if (PublishDisabled)
					{
						// Sauvegarde du post
						PostEnCours.Title = ValidationPost.Titre;
						PostEnCours.Content = ValidationPost.Content;
						PostEnCours.UpdatedAt = DateTime.Now;
						PostEnCours.UserId = LoginUser;
						PostEnCours.Image = ValidationPost.Image;
						PostEnCours.Id = await ContextBlog.AddPostAsync(PostEnCours);

						// Sauvegarde des catégories
						var allCategoriesSelected = Categories.Where(x => x.IsSelected).Select(x => x.IdCategorie).ToList();
						await ContextBlog.AddCategorieToPost(PostEnCours.Id, allCategoriesSelected);

						Snack.Add("Sauvegarde du post - OK", Severity.Success);
						PublishDisabled = false;
					}
					else
					{
						PostEnCours.Content = ValidationPost.Content;
						PostEnCours.Title = ValidationPost.Titre;
						PostEnCours.Image = ValidationPost.Image;
						PostEnCours.UpdatedAt = DateTime.Now;
						await ContextBlog.UpdatePostAsync(PostEnCours);
						Snack.Add($"Post mis à jour {PostEnCours.UpdatedAt.ToString("f")}", Severity.Success);
					}
				}
				catch (Exception ex)
				{
					Snack.Add("Erreur sur la sauvegarde du post", Severity.Error);
					Log.Error(ex, "NewPostViewModel - SavePost");
				}
			}
		}

		public async Task PublishPost()
		{
			if (!EditContextValidation.Validate())
			{
				return;
			}
			else
			{
				try
				{
					if(!PublishDisabled)
					{
						PostEnCours.Title = ValidationPost.Titre;
						PostEnCours.Content = ValidationPost.Content;
						if (PostEnCours.Posted == null)
						{
							// La date du post ne doit plus changer.
							PostEnCours.Posted = DateTime.Now;
						}
						PostEnCours.UpdatedAt = DateTime.Now;
						PostEnCours.Image = ValidationPost.Image;
						PostEnCours.IsPublished = true;

						// Sauvegarde des catégories
						var allCategoriesSelected = Categories.Where(x => x.IsSelected).Select(x => x.IdCategorie).ToList();
						await ContextBlog.AddCategorieToPost(PostEnCours.Id, allCategoriesSelected);

						await ContextBlog.PublishPostAsync(PostEnCours);
						Snack.Add($"Post publié {PostEnCours.UpdatedAt.ToString("f")}", Severity.Success);
					}
					else
					{
						Snack.Add($"Il faut enregistrer le post avant de le publier.", Severity.Warning);
					}
				}
				catch (Exception ex)
				{
					Snack.Add("Erreur sur la publication du post", Severity.Error);
					Log.Error(ex, "NewPostViewModel - PublishPost");
				}
			}
		}

		public async Task OpenGalerie()
		{
			string extensionImg = ".jpg, .jpeg, .png";
			var parameters = new DialogParameters();
			parameters.Add("AcceptExtensions", extensionImg);

			var dialog = DialogService.Show<GalerieComponent>("Galerie", parameters, FullScreenOption);
			var result = await dialog.Result;
			
			if(!result.Cancelled)
			{
				ImageEnAvant = result.Data.ToString();
				ValidationPost.Image = ImageEnAvant;
			}
		}

		public async Task AjouterCategorie()
		{
			if (!EditCtxCategorie.Validate())
			{
				return;
			}
			else
			{
				try
				{
					var categorie = new Categorie()
					{
						Nom = ValidationCategorie.Nom
					};

					categorie.IdCategorie = await ContextBlog.AddCategorie(categorie);
					categorie.IsSelected = true;
					Snack.Add($"Catégorie {categorie.Nom} ajoutée", Severity.Success);

					Categories.Add(categorie);
					ValidationCategorie = new CategorieValidation();
				}
				catch (Exception ex)
				{
					Snack.Add("Erreur sur l'ajout de la catégorie", Severity.Error);
					Log.Error(ex, "NewPostViewModel - AjouterCategorie");
				}
			}
		}


		public async Task OpenPreview()
		{
			await SavePost();
			await JSRuntime.InvokeAsync<object>("open", $"/preview/{PostEnCours.Id}", "_blank");
		}

		#endregion

		#region IDisposable

		public async void Dispose()
		{
			await KeysContext.DisposeAsync();
		}

		#endregion
	}
}

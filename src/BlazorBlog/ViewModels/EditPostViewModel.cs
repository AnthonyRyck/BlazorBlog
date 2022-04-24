using BlazorBlog.Composants;
using BlazorBlog.ValidationModels;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Toolbelt.Blazor.HotKeys;

namespace BlazorBlog.ViewModels
{
	public class EditPostViewModel : IEditPostViewModel, IDisposable
	{
		private readonly BlogContext ContextBlog;
		private Post PostEnCours;
		private readonly ISnackbar Snack;
		private readonly HotKeys KeysContext;
		private readonly IDialogService DialogService;
		private readonly DialogOptions FullScreenOption;


		public EditPostViewModel(BlogContext blogContext, ISnackbar snackbar, HotKeys hotKeys, IDialogService dialogService)
		{
			ContextBlog = blogContext;
			DialogService = dialogService;

			FullScreenOption = new DialogOptions() { FullScreen = true, CloseButton = true };

			ValidationPost = new PostValidation();
			EditContextValidation = new EditContext(ValidationPost);
			Snack = snackbar;
			KeysContext = hotKeys;
			KeysContext.CreateContext()
				.Add(ModKeys.Ctrl, Keys.S, SavePost, "Sauvegarde du post.", exclude: Exclude.ContentEditable);

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

		#region IEditPostViewModel


		public PostValidation ValidationPost { get; set; }

		public EditContext EditContextValidation { get; set; }

		public List<Categorie> Categories { get; private set; }

		public CategorieValidation ValidationCategorie { get; set; }

		public EditContext EditCtxCategorie { get; set; }

		public string ImageEnAvant { get; private set; }

		public async Task LoadPost(int idpost)
		{
			PostEnCours = await ContextBlog.GetPostAsync(idpost);
			ValidationPost.Content = PostEnCours.Content;
			ValidationPost.Titre = PostEnCours.Title;
			ValidationPost.Image = PostEnCours.Image;
			ValidationPost.Published = PostEnCours.IsPublished;
			ImageEnAvant = PostEnCours.Image;

			await InitViewModel();
		}

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
					PostEnCours.Content = ValidationPost.Content;
					PostEnCours.Title = ValidationPost.Titre;
					PostEnCours.UpdatedAt = DateTime.Now;
					PostEnCours.Image = ValidationPost.Image;
					await ContextBlog.UpdatePostAsync(PostEnCours);

					// Sauvegarde des catégories
					var allCategoriesSelected = Categories.Where(x => x.IsSelected).Select(x => x.IdCategorie).ToList();
					await ContextBlog.AddCategorieToPost(PostEnCours.Id, allCategoriesSelected);

					Snack.Add($"Post mis à jour {PostEnCours.UpdatedAt.ToString("f")}", Severity.Success);
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
					PostEnCours.Content = ValidationPost.Content;
					PostEnCours.Title = ValidationPost.Titre;
					PostEnCours.Image = ValidationPost.Image;
					if (PostEnCours.Posted == null)
					{
						PostEnCours.Posted = DateTime.Now;
					}
					PostEnCours.UpdatedAt = DateTime.Now;
					PostEnCours.IsPublished = true;
					await ContextBlog.PublishPostAsync(PostEnCours);

					// Sauvegarde des catégories
					var allCategoriesSelected = Categories.Where(x => x.IsSelected).Select(x => x.IdCategorie).ToList();
					await ContextBlog.AddCategorieToPost(PostEnCours.Id, allCategoriesSelected);

					Snack.Add($"Post publié {PostEnCours.UpdatedAt.ToString("f")}", Severity.Success);
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
			var dialog = DialogService.Show<GalerieComponent>("Galerie", FullScreenOption);
			var result = await dialog.Result;

			if (!result.Cancelled)
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

		#endregion

		#region IDisposable

		public async void Dispose()
		{
			await KeysContext.DisposeAsync();
		}

		#endregion
	}
}
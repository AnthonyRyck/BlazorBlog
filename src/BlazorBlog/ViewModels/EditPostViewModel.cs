using BlazorBlog.Composants;
using BlazorBlog.ModelsValidation;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Toolbelt.Blazor.HotKeys;

namespace BlazorBlog.ViewModels
{
	public class EditPostViewModel : IEditPostViewModel
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
		}

		#region IEditPostViewModel


		public PostValidation ValidationPost { get; set; }

		public EditContext EditContextValidation { get; set; }


		public string ImageEnAvant { get; private set; }

		public async Task LoadPost(int idpost)
		{
			PostEnCours = await ContextBlog.GetPostAsync(idpost);
			ValidationPost.Content = PostEnCours.Content;
			ValidationPost.Titre = PostEnCours.Title;
			ValidationPost.Image = PostEnCours.Image;
			ValidationPost.Published = PostEnCours.IsPublished;
			ImageEnAvant = PostEnCours.Image;
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
					PostEnCours.Posted = DateTime.Now;
					PostEnCours.UpdatedAt = DateTime.Now;
					PostEnCours.IsPublished = true;
					await ContextBlog.PublishPostAsync(PostEnCours);

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

		#endregion
	}
}
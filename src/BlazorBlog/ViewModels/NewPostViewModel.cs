using BlazorBlog.Composants;
using BlazorBlog.Core;
using BlazorBlog.ModelsValidation;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
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

		public NewPostViewModel(BlogContext blogContext, ISnackbar snackbar, HotKeys hotKeys, IDialogService dialogService, 
								IHttpContextAccessor httpContextAccessor)
		{
			ContextBlog = blogContext;
			DialogService = dialogService;
			LoginUser = httpContextAccessor.HttpContext.User.Identity.Name;

			FullScreenOption = new DialogOptions() { FullScreen = true, CloseButton = true };

			PostEnCours = new Post();
			PublishDisabled = true;
			
			ValidationPost = new PostValidation();
			EditContextValidation = new EditContext(ValidationPost);
			Snack = snackbar;
			KeysContext = hotKeys;
			KeysContext.CreateContext()
				.Add(ModKeys.Ctrl, Keys.S, SavePost, "Sauvegarde du post.", exclude: Exclude.ContentEditable);
		}

		#region INewPostViewModel

		public PostValidation ValidationPost { get; set; }

		public EditContext EditContextValidation { get; set; }

		public bool PublishDisabled { get; private set; }

		public string ImageEnAvant { get; private set; }


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
						PostEnCours.Title = ValidationPost.Titre;
						PostEnCours.Content = ValidationPost.Content;
						PostEnCours.UpdatedAt = DateTime.Now;
						PostEnCours.UserId = LoginUser;
						PostEnCours.Image = ValidationPost.Image;
						PostEnCours.Id = await ContextBlog.AddPostAsync(PostEnCours);
						
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
						PostEnCours.Posted = DateTime.Now;
						PostEnCours.UpdatedAt = DateTime.Now;
						PostEnCours.Image = ValidationPost.Image;
						PostEnCours.IsPublished = true;
						
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
			var dialog = DialogService.Show<GalerieComponent>("Galerie", FullScreenOption);
			var result = await dialog.Result;
			
			if(!result.Cancelled)
			{
				ImageEnAvant = result.Data.ToString();
				ValidationPost.Image = ImageEnAvant;
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

using BlazorBlog.AccessData;
using BlazorBlog.Core;
using BlazorBlog.ModelsValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public class NewPostViewModel : INewPostViewModel
	{
		private Action StateHasChanged;
		private BlogContext ContextBlog;
		private Post PostEnCours;

		public NewPostViewModel(BlogContext blogContext)
		{
			ContextBlog = blogContext;
			PostEnCours = new Post()
			{
				Id = -1
			};
			ValidationPost = new PostValidation();
			EditContextValidation = new EditContext(ValidationPost);
		}

		#region INewPostViewModel

		public PostValidation ValidationPost { get; set; }

		public EditContext EditContextValidation { get; set; }


		public async Task OnValidSubmitPost()
		{
			// Faire la sauvegarde dans la base de données
			try
			{
				// Post pas encore sauvegardé en base
				if(PostEnCours.Id == -1)
				{
					PostEnCours = ValidationPost.ToPost(new Guid());
					await ContextBlog.AddPostAsync(PostEnCours);
				}
				else
				{
					PostEnCours.Content = ValidationPost.Content;
					PostEnCours.Title = ValidationPost.Titre;
					PostEnCours.UpdatedAt = DateTime.Now;
					await ContextBlog.UpdatePostAsync(PostEnCours);
				}
			}
			catch (Exception ex)
			{
				
			}

			StateHasChanged.Invoke();
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
					// Post pas encore sauvegardé en base
					if (PostEnCours.Id == -1)
					{
						PostEnCours = ValidationPost.ToPost(new Guid());
						await ContextBlog.AddPostAsync(PostEnCours);
					}
					else
					{
						PostEnCours.Content = ValidationPost.Content;
						PostEnCours.Title = ValidationPost.Titre;
						PostEnCours.UpdatedAt = DateTime.Now;
						await ContextBlog.UpdatePostAsync(PostEnCours);
					}
				}
				catch (Exception ex)
				{

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
					// Post pas encore sauvegardé en base
					if (PostEnCours.Id == -1)
					{
						PostEnCours = ValidationPost.ToPost(new Guid());
						PostEnCours.IsPublished = true;
						await ContextBlog.AddPostAsync(PostEnCours);
					}
					else
					{
						PostEnCours.Content = ValidationPost.Content;
						PostEnCours.Title = ValidationPost.Titre;
						PostEnCours.UpdatedAt = DateTime.Now;
						PostEnCours.IsPublished = true;
						await ContextBlog.UpdatePostAsync(PostEnCours);
					}
				}
				catch (Exception ex)
				{

				}
			}
		}


		public void SetStateHasChanged(Action state)
		{
			StateHasChanged = state;
		}

		#endregion

		public NewPostViewModel()
		{
			ValidationPost = new PostValidation();
		}
	}
}

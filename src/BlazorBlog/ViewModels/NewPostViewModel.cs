﻿using BlazorBlog.Core;
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

		public NewPostViewModel(BlogContext blogContext, ISnackbar snackbar, HotKeys hotKeys)
		{
			ContextBlog = blogContext;
			PostEnCours = new Post()
			{
				Id = -1
			};
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
						Snack.Add("Sauvegarde du post - OK", Severity.Success);
					}
					else
					{
						PostEnCours.Content = ValidationPost.Content;
						PostEnCours.Title = ValidationPost.Titre;
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
					Snack.Add("Erreur sur la publication du post", Severity.Error);
					Log.Error(ex, "NewPostViewModel - PublishPost");
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

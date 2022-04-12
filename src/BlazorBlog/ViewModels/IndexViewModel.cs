using Microsoft.AspNetCore.Components;

namespace BlazorBlog.ViewModels
{
	public class IndexViewModel : IIndexViewModel
	{
		private readonly BlogContext ContextBlog;
		private readonly NavigationManager navigationManager;


		public IndexViewModel(BlogContext context, NavigationManager navigation)
		{
			ContextBlog = context;
			navigationManager = navigation;
			Posts = new List<Post>();
		}
		
		#region IIndexViewModel


		public IEnumerable<Post> Posts { get; private set; }

		public async Task GetAllPosts()
		{
			Posts = await ContextBlog.GetPublishedPostsAsync();
		}


		public void OpenPost(int id)
		{
			navigationManager.NavigateTo($"/post/{id}", true);
		}

		#endregion

	}
}

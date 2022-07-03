using Microsoft.AspNetCore.Components;

namespace BlazorBlog.ViewModels
{
	public class IndexViewModel : IIndexViewModel
	{
		private readonly BlogContext ContextBlog;
		private readonly NavigationManager navigationManager;
		private List<Post> AllPosts;
		private const int PageSize = 10;

		public IndexViewModel(BlogContext context, NavigationManager navigation)
		{
			ContextBlog = context;
			navigationManager = navigation;
			AllPosts = new List<Post>();
			PostsToDisplay = new List<Post>();
		}
		
		private List<Post> SelectPost(int page)
		{
			int start = (page - 1) * PageSize;

			if((start + PageSize) > AllPosts.Count)
				return AllPosts.GetRange(start, AllPosts.Count - start);

			return AllPosts.GetRange(start, PageSize);
		}


		#region IIndexViewModel
		
		public IEnumerable<Post> PostsToDisplay { get; private set; }

		public int CounterPage { get; private set; }

		public Categorie CategorieSelected { get; private set; }

		public async Task GetAllPosts(int? idCategorie = null)
		{
			AllPosts = await ContextBlog.GetPublishedPostsAsync(idCategorie);
			if (idCategorie.HasValue)
				CategorieSelected = await ContextBlog.GetCategorie(idCategorie.Value);
			else CategorieSelected = null;

			double tempCounter = Convert.ToDouble(AllPosts.Count()) / PageSize;
			if ((tempCounter - Math.Truncate(tempCounter)) > 0)
				CounterPage = Convert.ToInt32(Math.Truncate(tempCounter)) + 1;
			else
				CounterPage = Convert.ToInt32(Math.Truncate(tempCounter));

			PostsToDisplay = SelectPost(1);
		}


		public void OpenPost(int id)
		{
			navigationManager.NavigateTo($"/post/{id}");
		}


		public void PageChanged(int page)
		{
			PostsToDisplay = SelectPost(page);
		}

		#endregion

	}
}

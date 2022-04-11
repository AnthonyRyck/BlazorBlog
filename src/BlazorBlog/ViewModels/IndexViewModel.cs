namespace BlazorBlog.ViewModels
{
	public class IndexViewModel : IIndexViewModel
	{
		private readonly BlogContext ContextBlog;



		public IndexViewModel(BlogContext context)
		{
			ContextBlog = context;
			Posts = new List<Post>();
		}

		#region IIndexViewModel


		public IEnumerable<Post> Posts { get; private set; }

		public async Task GetAllPosts()
		{
			Posts = await ContextBlog.GetPublishedPostsAsync();
		}

		#endregion

	}
}

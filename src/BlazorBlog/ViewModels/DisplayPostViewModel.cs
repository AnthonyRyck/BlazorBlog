namespace BlazorBlog.ViewModels
{
	public class DisplayPostViewModel : IDisplayPostViewModel
	{
		private readonly BlogContext Context;

		public DisplayPostViewModel(BlogContext blogContext)
		{
			Context = blogContext;
		}


		#region IDisplayPostViewModel

		public bool IsLoading { get; private set; }

		public Post Article { get; private set; }

		public async Task LoadPost(int idpost)
		{
			IsLoading = true;
			Article = await Context.GetPostAsync(idpost);
			IsLoading = false;
		}

		#endregion

	}
}
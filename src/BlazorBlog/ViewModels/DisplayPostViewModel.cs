
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


		public Task LoadPost(int idpost)
		{
			IsLoading = true;



			IsLoading = false;
		}

	}
}
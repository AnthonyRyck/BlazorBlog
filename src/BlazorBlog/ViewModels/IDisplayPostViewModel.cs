namespace BlazorBlog.ViewModels
{
	public interface IDisplayPostViewModel
	{
		/// <summary>
		/// Indique un chargement du post
		/// </summary>
		bool IsLoading { get; }


		/// <summary>
		/// Charge le post demandé.
		/// </summary>
		/// <param name="idpost"></param>
		/// <returns></returns>
		Task LoadPost(int idpost);

		
	}
}
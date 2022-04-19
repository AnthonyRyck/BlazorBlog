namespace BlazorBlog.ViewModels
{
	public interface IDisplayPostViewModel
	{
		/// <summary>
		/// Indique un chargement du post
		/// </summary>
		bool IsLoading { get; }

		/// <summary>
		/// C'est tout le post
		/// </summary>
		Post Article { get; }

		/// <summary>
		/// Charge le post demandé.
		/// </summary>
		/// <param name="idpost"></param>
		/// <returns></returns>
		Task LoadPost(int idpost);

		/// <summary>
		/// URL pour partager sur LinkedIn
		/// </summary>
		string UrlShareLinkedIn { get; }

		/// <summary>
		/// URL pour partager sur Twitter
		/// </summary>
		string UrlShareTwitter { get; }

		/// <summary>
		/// URL pour partager sur Facebook
		/// </summary>
		string UrlShareFacebook { get; }

		/// <summary>
		/// Indique si le partage est possible.
		/// </summary>
		bool DisabledShare { get; }
	}
}
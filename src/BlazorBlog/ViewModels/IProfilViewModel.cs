namespace BlazorBlog.ViewModels
{
	public interface IProfilViewModel
	{
		/// <summary>
		/// URL de l'image du profil
		/// </summary>
		Profil UserProfil { get; }

		/// <summary>
		/// Charge le profil de l'utilisateur.
		/// </summary>
		/// <returns></returns>
		Task LoadProfil();

		/// <summary>
		/// Ouvre la galerie d'image pour le profil.
		/// </summary>
		/// <returns></returns>
		Task OpenGalerieProfil();

		/// <summary>
		/// Sauvegarde en BDD le profil utilisateur.
		/// </summary>
		/// <returns></returns>
		Task SaveProfil();
	}
}

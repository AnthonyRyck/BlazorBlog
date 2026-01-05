namespace BlazorBlog.ViewModels
{
	public interface ICategoriesViewModel
	{
		List<Categorie> Categories { get; }

		Task InitializeAsync();



        Task EditCategorie(Categorie item);

        Task DeleteCategorie(int idCategorie);
	}
}

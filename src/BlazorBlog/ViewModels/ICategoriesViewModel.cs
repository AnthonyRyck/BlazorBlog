namespace BlazorBlog.ViewModels
{
	public interface ICategoriesViewModel
	{
		List<Categorie> Categories { get; }

		Task InitializeAsync();



        Task CommittedItemChanges(Categorie item);

        Task DeleteCategorie(int idCategorie);
	}
}

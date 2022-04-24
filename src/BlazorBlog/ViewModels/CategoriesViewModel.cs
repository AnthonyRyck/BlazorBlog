namespace BlazorBlog.ViewModels
{
	public class CategoriesViewModel : ICategoriesViewModel
	{
        private readonly BlogContext ContextBlog;
		private readonly ISnackbar Snack;

		public CategoriesViewModel(BlogContext blogContext, ISnackbar snackbar)
		{
			ContextBlog = blogContext;
            Categories = new List<Categorie>();
			Snack = snackbar;
			//InitializeAsync().GetAwaiter().GetResult();
		}
        
	    
		#region Implement ICategoriesViewModel


		public List<Categorie> Categories { get; private set; }


        public async Task InitializeAsync()
        {
            Categories = await ContextBlog.GetCategories();
        }

        public async Task CommittedItemChanges(Categorie item)
        {
			try
			{
				await ContextBlog.UpdateCategorie(item);
				Snack.Add("Modification de la catégorie - OK", Severity.Success);				
			}
			catch (Exception ex)
			{
				Snack.Add("Modification de la catégorie - Erreur", Severity.Error);
				Log.Error(ex, "CategoriesViewModel - CommittedItemChanges");
			}
        }

		public async Task DeleteCategorie(int idCategorie)
		{
			try
			{
				await ContextBlog.DeleteCategorie(idCategorie);
				Categories.RemoveAll(x => x.IdCategorie == idCategorie);
				Snack.Add("Suppression de la catégorie - OK", Severity.Success);
			}
			catch (Exception ex)
			{
				Snack.Add("Erreur sur la suppression de la catégorie", Severity.Error);
				Log.Error(ex, "CategoriesViewModel - DeleteCategorie");
			}
		}

		#endregion
	}
}

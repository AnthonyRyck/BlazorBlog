using BlazorBlog.Composants;

namespace BlazorBlog.ViewModels
{
	public class CategoriesViewModel : ICategoriesViewModel
	{
        private readonly BlogContext ContextBlog;
		private readonly ISnackbar Snack;
		private readonly IDialogService DialogService;

		public CategoriesViewModel(BlogContext blogContext, ISnackbar snackbar, IDialogService dialogService)
		{
			ContextBlog = blogContext;
            Categories = new List<Categorie>();
			Snack = snackbar;
			DialogService = dialogService;
			//InitializeAsync().GetAwaiter().GetResult();
		}
        
	    
		#region Implement ICategoriesViewModel


		public List<Categorie> Categories { get; private set; }


        public async Task InitializeAsync()
        {
            Categories = await ContextBlog.GetCategories();
        }

        public async Task EditCategorie(Categorie item)
        {
			try
			{
				// Dans le cas ou il dépasserait 100 caractères
				string saveName = item.Nom;

				var options = new DialogOptions() { FullScreen = false, CloseButton = true };
				var parameters = new DialogParameters();
				parameters.Add("CategorieToEdit", item);

				var dialog = DialogService.Show<EditCategorieDialog>("Edit Catégorie", parameters, options);
				var result = await dialog.Result;
				
				if (!result.Cancelled)
				{
					Categorie categorieUpdated = (Categorie) result.Data;
					if(categorieUpdated.Nom.Count() > 100)
					{
						Snack.Add("Le nom de la catégorie est trop long", Severity.Warning);
						item.Nom = saveName;
						
						return;
					}

					await ContextBlog.UpdateCategorie(categorieUpdated);
					Snack.Add("Modification de la catégorie - OK", Severity.Success);				
				}
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

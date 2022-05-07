using Microsoft.AspNetCore.Components.Forms;

namespace BlazorBlog.ViewModels
{
	public interface IImportExportViewModel
	{
		/// <summary>
		/// Liste des sauvegardes sur le serveur
		/// </summary>
		List<SauvegardeFile> Sauvegardes { get; }


		Task InitAsync();

		/// <summary>
		/// Export les données vers un fichier
		/// </summary>
		/// <returns></returns>
		Task ExportDatabase();

		/// <summary>
		/// Télécharge le fichier
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		Task Download(SauvegardeFile file);

		/// <summary>
		/// Supprime le fichier
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		Task Delete(SauvegardeFile file);

		/// <summary>
		/// Importe un fichier de sauvegarde
		/// </summary>
		/// <returns></returns>
		Task ImportDatabase(InputFileChangeEventArgs e);
	}
}

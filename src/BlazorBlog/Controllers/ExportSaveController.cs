using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace BlazorBlog.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize(Roles = ConstantesApp.ROLE_ADMIN)]
	public class ExportSaveController : ControllerBase
	{
        // GET api/values/5
        [HttpGet("{fileNameExport}")]
        public IActionResult Download(string fileNameExport)
        {
            var temp = DownloadExport(fileNameExport);
            Log.Information("Récupération de l'export par API : " + fileNameExport);

            return temp;
        }


        #region Private Methods

        private IActionResult DownloadExport(string exportFile)
        {
            IActionResult exportFileResult = null;

            try
            {
                Log.Information("Téléchargement de " + exportFile);

                string pathFileExport = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, exportFile);

                FileStream file = new FileStream(pathFileExport, FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
                    true);

                Func<Stream, ActionContext, Task> funcTemp = async (outputStream, context) =>
                {
                    using (var fileStream = new WriteOnlyStreamWrapper(outputStream))
                    {
                        using (var stream = file)
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                };

                exportFileResult = new FileCallbackResult("application/octet-stream", funcTemp)
                {
                    FileDownloadName = exportFile
                };
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Erreur sur la récupération de l'export : " + exportFile);
                exportFileResult = NoContent();
            }

            return exportFileResult;
        }

        #endregion
    }
}

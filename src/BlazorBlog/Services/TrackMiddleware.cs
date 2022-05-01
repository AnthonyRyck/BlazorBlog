using System.Text;

namespace BlazorBlog.Middlewares
{
    public class TrackMiddleware
    {
        private readonly RequestDelegate _next;
        public TrackMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext httpContext, IHttpContextAccessor accessor)
        {
            string ipAppelant = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string leBrowser = httpContext.Request.Headers["User-Agent"].ToString();
            string urlRecherche = httpContext.Request.Path.ToString();

            // Avoir le nom du système d'exploitation
            string os = httpContext.Request.Headers["sec-ch-ua-platform"].ToString();

            // Avoir l'IP de la requête
            string ip = httpContext.Connection?.RemoteIpAddress.ToString();

            //string method = httpContext.Request.Method.ToString();

            // Indique d'ou vient le trafic, identifie l'adresse de la page précédente.
            string referer = httpContext.Request.Headers["Referer"].ToString();
            var deviceModel = httpContext.Request.Headers["sec-ch-ua-model"].ToString();

            StringBuilder sb = new StringBuilder();
			foreach (var item in httpContext.Request.Headers)
			{
                var key = item.Key;
                string value = item.Value;

                sb.AppendLine($"KEY : {key}")
                    .AppendLine($"VALUE : {value}")
                    .AppendLine("------------------");
			}
            sb.AppendLine($"IP : {ip}");
            sb.AppendLine("################################");
            File.AppendAllText(@"C:\123\httpContextRequestHeader.txt", sb.ToString());

            // Appel au prochaine delegate/middleware du pipeline
            // Si pas fait, tout s'arrête !
            await _next(httpContext);
        }

    }
}
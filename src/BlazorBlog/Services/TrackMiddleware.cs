using BlazorBlog.ViewModels;
using System.Text;

namespace BlazorBlog.Middlewares
{
    public class TrackMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BlogContext contextDb;

        public TrackMiddleware(RequestDelegate next, BlogContext db)
        {
            _next = next;
            contextDb = db;
		}

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Savoir si c'est un nouveau visiteur
                string visitorId = context.Request.Cookies["VisitorId"];
                if (visitorId == null)
                {
                    // Nouveau visiteur.
                    context.Response.Cookies.Append("VisitorId", Guid.NewGuid().ToString(), new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = true,
                        Secure = false,
                        Expires = DateTimeOffset.Now.AddYears(1)
                    });
                }

                if (context.Request.Method == "GET")
                {
                    // URL voulue
                    string postVoulu = context.Request.Path;

                    // Ne pas prendre les appels aux fichiers statiques, images des utilisateur.
                    if (postVoulu.StartsWith(ConstantesApp.USERIMG)
                        || postVoulu.StartsWith(ConstantesApp.COUNTER_KEY_EXCEPT)
                        || !postVoulu.StartsWith(ConstantesApp.COUNTER_KEY_POSTS))
                    {
                        await _next(context);
                        return;
                    }

                    Track track = new Track();
                    track.VisitorId = context.Request.Cookies["VisitorId"].ToString();
                    track.DateRequest = DateTime.Now;
                    track.PostId = Convert.ToInt32(postVoulu.Replace(ConstantesApp.COUNTER_KEY_POSTS, String.Empty));

                    await contextDb.SaveTracks(track);

                    // Indique d'ou vient le trafic, identifie l'adresse de la page précédente.
                    // garde au cas ou
                    //string referer = context.Request.Headers["Referer"].ToString();

                    // Plateforme du client (garde au cas ou)
                    // sec-ch-ua-platform --> plateforme (ex : Windows)
                    // string platform = context.Request.Headers["sec-ch-ua-platform"].ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error sur Track");
            }

            await _next(context);
        }

    }
}
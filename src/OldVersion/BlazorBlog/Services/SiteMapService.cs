using System.Xml;
using System.Xml.Serialization;

namespace BlazorBlog.Services
{
	public class SiteMapService
	{
		private BlogContext BlogContext;
		private SettingsSvc SettingSvc;


		public SiteMapService(BlogContext context, SettingsSvc settingSvc)
		{
			BlogContext = context;
			SettingSvc = settingSvc;
		}

		public async Task CreateSitemapAsync()
		{
			try
			{
				var posts = await BlogContext.GetPublishedPostsSitemapAsync();
				List<url> urlset = new List<url>();

				string frequence = "weekly";

				if (posts.Count > 0)
				{
					Uri baseUri = new Uri(SettingSvc.BlogUrl);
					urlset.Add(new url()
					{
						loc = baseUri.AbsoluteUri,
						changefreq = frequence,
						lastmod = String.Format("{0:yyyy-MM-dd}", DateTime.Now)
					});
					
					foreach (var post in posts)
					{
						var uriPost = new Uri(baseUri, "/post/" + post.Id);
						urlset.Add(new url()
						{
							loc = uriPost.AbsoluteUri,
							changefreq = frequence,
							lastmod = String.Format("{0:yyyy-MM-dd}", post.UpdatedAt)
						});
					}

					XmlSerializer xmlSer = new XmlSerializer(urlset.GetType(), null, null, new XmlRootAttribute("urlset"), defaultNamespace: "http://www.sitemaps.org/schemas/sitemap/0.9");
					XmlSerializerNamespaces emptyNS = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

					string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstantesApp.IMAGES, "sitemap", "sitemap.xml");

					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					settings.IndentChars = ("\t");
					settings.OmitXmlDeclaration = true;

					using (TextWriter sw = new StreamWriter(path))
					{
						using (XmlWriter xw = XmlWriter.Create(sw, settings))
						{
							var xmlSeria = new XmlSerializer(urlset.GetType(), null, null, new XmlRootAttribute("urlset"), defaultNamespace: "http://www.sitemaps.org/schemas/sitemap/0.9");
							xmlSeria.Serialize(xw, urlset, emptyNS);
						}
					}

					// Une fois le fichier créé, faire un ping à Google.
					// https://www.google.com/ping?sitemap=FULL_URL_OF_SITEMAP
					Uri uriSitemap = new Uri(baseUri, "sitemap.xml");
					string urlPingGoogle = "https://www.google.com/ping?sitemap=" + uriSitemap.AbsoluteUri;

					HttpClient httpClient = new HttpClient();
					var responseGoogle = await httpClient.GetAsync(urlPingGoogle);
					if(responseGoogle.IsSuccessStatusCode)
						Log.Information("Sitemap pingé à Google : " + urlPingGoogle);
					else Log.Information("Sitemap avec status : " + responseGoogle.StatusCode);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ERREUR - Création du fichier Sitemap");
			}
		}
	}
}

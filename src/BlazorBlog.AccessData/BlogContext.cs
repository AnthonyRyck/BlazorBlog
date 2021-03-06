using BlazorBlog.Core;
using BlazorBlog.Core.EntityView;
using MySql.Data.MySqlClient;

namespace BlazorBlog.AccessData
{
	public class BlogContext
	{
		private string ConnectionString;

		public BlogContext(string connectionString)
		{
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Permet de créer les tables pour le blog
		/// </summary>
		/// <returns></returns>
		public async Task CreateTablesAsync(string pathSql)
		{
			try
			{
				string cmd = await File.ReadAllTextAsync(pathSql);
				await ExecuteCoreAsync(cmd);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Permet d'éxécuter un script pour mettre à jour la base de donnée.
		/// </summary>
		/// <param name="pathSql"></param>
		/// <returns></returns>
		public async Task UpdateDatabaseAsync(string pathSql)
		{
			try
			{
				string cmd = await File.ReadAllTextAsync(pathSql);
				await ExecuteCoreAsync(cmd);
			}
			catch (Exception)
			{
				throw;
			}
		}


		#region Posts

		/// <summary>
		/// Récupère tous les posts d'un auteur, si null, récupère tous les posts
		/// </summary>
		/// <returns></returns>
		public async Task<List<PostView>> GetPostsAsync(string userId)
		{
			//var commandText = @"SELECT idpost, title, posted, updateat, userid, ispublished "
			//				 + $"FROM posts WHERE userid='{userId}' ORDER BY posted DESC;";

			var commandText = "SELECT post.idpost, post.title, post.posted, post.updateat, post.userid, post.ispublished, COUNT(trk.postid) "
								+ "FROM posts post "
								+ "LEFT JOIN tracks trk "
								+ "ON trk.postid = post.idpost "
								+ $"WHERE userid = '{userId}' "
								+ "GROUP BY post.idpost "
								+ "ORDER BY posted DESC;";
			
			Func<MySqlCommand, Task<List<PostView>>> funcCmd = async (cmd) =>
			{
				List<PostView> posts = new List<PostView>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						object tempDate = reader.GetValue(2);
						DateTime? datePosted = ConvertFromDBVal<DateTime?>(tempDate);

						var post = new PostView()
						{
							Id = reader.GetInt32(0),
							Title = reader.GetString(1),
							Posted = datePosted,
							UpdatedAt = reader.GetDateTime(3),
							UserId = reader.GetString(4),
							IsPublished = reader.GetBoolean(5),
							CompteurVisite = reader.GetInt32(6)
						};

						posts.Add(post);
					}
				}

				return posts;
			};

			List<PostView> posts = new List<PostView>();

			try
			{
				posts = await GetCoreAsync(commandText, funcCmd);

				// Maintenant récupérer pour chaque post, les catégories associés
				foreach (var post in posts)
				{
					post.Categories = new List<Categorie>();
					List<int> allCategories = await GetCategoriesByPost(post.Id);
					foreach (var idCategory in allCategories)
					{
						Categorie categorie = await GetCategorie(idCategory);
						post.Categories.Add(categorie);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			return posts;
		}

		/// <summary>
		/// Supprime le post.
		/// </summary>
		/// <param name="idPost"></param>
		/// <returns></returns>
		public async Task DeletePostAsync(int idPost)
		{
			var commandText = $"DELETE FROM posts WHERE idpost={idPost};";
			try
			{
				await ExecuteCoreAsync(commandText);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Récupère tous les posts qui sont publiés
		/// </summary>
		/// <returns></returns>
		public async Task<List<Post>> GetPublishedPostsAsync(int? idCategorie = null)
		{
			string commandText = string.Empty;
			if (!idCategorie.HasValue)
			{
				commandText = @"SELECT idpost, title, image, posted, userid "
								 + "FROM posts "
								 + "WHERE ispublished=1 "
								 + "ORDER BY posted DESC;";
			}
			else
			{
				commandText = @"SELECT article.idpost, article.title, article.image, article.posted, article.userid"
								+ " FROM posts article"
								+ " INNER JOIN categorietopost cat"
								+ " ON article.idpost = cat.postid"
								+ $" WHERE cat.categorieid={idCategorie.Value}"
								+ " AND article.ispublished=1"
								+ " ORDER BY posted DESC;";
			}

			Func<MySqlCommand, Task<List<Post>>> funcCmd = async (cmd) =>
			{
				List<Post> posts = new List<Post>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						var post = new Post()
						{
							Id = reader.GetInt32(0),
							Title = reader.GetString(1),
							Image = reader.GetString(2),
							Posted = ConvertFromDBVal<DateTime?>(reader.GetValue(3)),
							UserId = reader.GetString(4)
						};

						posts.Add(post);
					}
				}

				return posts;
			};
			List<Post> posts = new List<Post>();

			try
			{
				posts = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return posts;
		}

		/// <summary>
		/// Récupère tous les posts qui sont publiés
		/// </summary>
		/// <returns></returns>
		public async Task<List<Post>> GetPublishedPostsSitemapAsync()
		{
			string commandText = @"SELECT idpost, posted, updateat "
								 + "FROM posts "
								 + "WHERE ispublished=1 "
								 + "ORDER BY posted DESC;";
			
			Func<MySqlCommand, Task<List<Post>>> funcCmd = async (cmd) =>
			{
				List<Post> posts = new List<Post>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						var post = new Post()
						{
							Id = reader.GetInt32(0),
							Posted = ConvertFromDBVal<DateTime?>(reader.GetValue(1)),
							UpdatedAt = reader.GetDateTime(2)
						};

						posts.Add(post);
					}
				}

				return posts;
			};
			List<Post> posts = new List<Post>();

			try
			{
				posts = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return posts;
		}

		/// <summary>
		/// Récupère le post par rapport à son id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Post> GetPostAsync(int id)
		{
			var commandText = @"SELECT idpost, title, content, image, posted, updateat, userid, ispublished "
							 + "FROM posts "
							 + $"WHERE idpost={id};";

			Func<MySqlCommand, Task<Post>> funcCmd = async (cmd) =>
			{
				Post post = null;
				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						post = new Post()
						{
							Id = reader.GetInt32(0),
							Title = reader.GetString(1),
							Content = reader.GetString(2),
							Image = reader.GetString(3),
							Posted = ConvertFromDBVal<DateTime?>(reader.GetValue(4)),
							UpdatedAt = reader.GetDateTime(5),
							UserId = reader.GetString(6),
							IsPublished = reader.GetBoolean(7)
						};
					}
				}

				return post;
			};

			Post postSelected = null;
			try
			{
				postSelected = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return postSelected;
		}

		/// <summary>
		/// Ajout d'un nouveau post
		/// </summary>
		/// <param name="nouveauPost"></param>
		/// <returns>Retourne l'ID du post</returns>
		public async Task<int> AddPostAsync(Post nouveauPost)
		{
			try
			{
				int idNewPost = 0;
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO posts (title, content, image, posted, updateat, userid, ispublished)"
									+ " VALUES(@title, @content, @image, @posted, @updateat, @userid, @ispublished);";

					nouveauPost.UpdatedAt = DateTime.Now;

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@title", nouveauPost.Title);
						cmd.Parameters.AddWithValue("@content", nouveauPost.Content);
						cmd.Parameters.AddWithValue("@image", nouveauPost.Image);
						cmd.Parameters.AddWithValue("@posted", nouveauPost.Posted);
						cmd.Parameters.AddWithValue("@updateat", nouveauPost.UpdatedAt);
						cmd.Parameters.AddWithValue("@userid", nouveauPost.UserId.ToString());
						cmd.Parameters.AddWithValue("@ispublished", nouveauPost.IsPublished);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();

						// Récupération de l'ID.
						string commandId = "SELECT LAST_INSERT_ID();";
						cmd.CommandText = commandId;
						idNewPost = Convert.ToInt32(await cmd.ExecuteScalarAsync());

						conn.Close();
					}
				}

				return idNewPost;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Met à jour ce post.
		/// </summary>
		/// <param name="postEnCours"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public async Task UpdatePostAsync(Post postEnCours)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE posts SET title=@titre, content=@contenu, image=@image, updateat=@update, ispublished=@ispublish"
									  + $" WHERE idpost={postEnCours.Id};";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@titre", postEnCours.Title);
					cmd.Parameters.AddWithValue("@contenu", postEnCours.Content);
					cmd.Parameters.AddWithValue("@image", postEnCours.Image);
					cmd.Parameters.AddWithValue("@update", postEnCours.UpdatedAt);
					cmd.Parameters.AddWithValue("@ispublish", postEnCours.IsPublished);

					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}

		public async Task PublishPostAsync(Post postEnCours)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE posts SET title=@titre, content=@contenu, image=@image, posted=@posted, updateat=@update, ispublished=@ispublish"
									  + $" WHERE idpost={postEnCours.Id};";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@titre", postEnCours.Title);
					cmd.Parameters.AddWithValue("@contenu", postEnCours.Content);
					cmd.Parameters.AddWithValue("@image", postEnCours.Image);
					cmd.Parameters.AddWithValue("@posted", postEnCours.Posted);
					cmd.Parameters.AddWithValue("@update", postEnCours.UpdatedAt);
					cmd.Parameters.AddWithValue("@ispublish", postEnCours.IsPublished);

					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}


		public async Task<int> GetCounterImage(string name)
		{
			// Récupération de toutes les catégories
			var cmdTxtEnAvant = "SELECT COUNT(idpost)" 
							+ " FROM posts"
							+ $" WHERE image = '{name}';";

			var cmdTxtInPost = "SELECT COUNT(idpost)"
							+ " FROM posts"
							+ $" WHERE content LIKE '%]({name})%';";


			int counter;

			try
			{
				int counterImgEnAvant = await GetIntCore(cmdTxtEnAvant);
				int counterImgInPost = await GetIntCore(cmdTxtInPost);

				counter = counterImgEnAvant + counterImgInPost;
			}
			catch (Exception)
			{
				throw;
			}

			return counter;
		}

		#endregion

		#region Settings

		public async Task<List<Settings>> GetSettings()
		{
			var commandText = @"SELECT settingname, settingvalue "
							 + "FROM settings;";

			Func<MySqlCommand, Task<List<Settings>>> funcCmd = async (cmd) =>
			{
				List<Settings> allSettings = new List<Settings>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						var setting = new Settings()
						{
							SettingName = reader.GetString(0),
							Value = reader.GetString(1)
						};

						allSettings.Add(setting);
					}
				}

				return allSettings;
			};
			List<Settings> settings;

			try
			{
				settings = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return settings;
		}

		public async Task AddDefaultSettings(List<Settings> settings)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO settings (settingname, settingvalue)"
									+ " VALUES(@settingname, @settingvalue);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						conn.Open();
						foreach (var item in settings)
						{
							if (cmd.Parameters.Count > 0)
							{
								cmd.Parameters.Clear();
							}

							cmd.Parameters.AddWithValue("@settingname", item.SettingName);
							cmd.Parameters.AddWithValue("@settingvalue", item.Value);

							await cmd.ExecuteNonQueryAsync();
						}
						
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		
		public async Task UpdateSettings(List<Settings> settings)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE settings SET settingvalue=@settingvalue"
									  + " WHERE settingname=@settingName;";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					conn.Open();
					foreach (var item in settings)
					{
						if (cmd.Parameters.Count > 0)
						{
							cmd.Parameters.Clear();
						}

						cmd.Parameters.AddWithValue("@settingName", item.SettingName);
						cmd.Parameters.AddWithValue("@settingvalue", item.Value);

						await cmd.ExecuteNonQueryAsync();
					}
					conn.Close();
				}
			}
		}

		#endregion

		#region Categories

		public async Task<List<Categorie>> GetCategories()
		{
			// Récupération de toutes les catégories
			var commandText = @"SELECT idcategorie, nom "
							 + "FROM categories;";

			Func<MySqlCommand, Task<List<Categorie>>> funcCmd = async (cmd) =>
			{
				List<Categorie> categories = new List<Categorie>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						var categorie = new Categorie()
						{
							IdCategorie = reader.GetInt32(0),
							Nom = reader.GetString(1)
						};

						categories.Add(categorie);
					}
				}

				return categories;
			};
			List<Categorie> categories;
			
			try
			{
				categories = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return categories;
		}

		public async Task<Categorie> GetCategorie(int idCategorie)
		{
			// Récupération de toutes les catégories
			var commandText = @"SELECT idcategorie, nom "
							 + "FROM categories "
							 + $"WHERE idcategorie={idCategorie}";

			Func<MySqlCommand, Task<Categorie>> funcCmd = async (cmd) =>
			{
				Categorie categorie = new Categorie();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						categorie.IdCategorie = reader.GetInt32(0);
						categorie.Nom = reader.GetString(1);
					}
				}

				return categorie;
			};
			Categorie categorie;

			try
			{
				categorie = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return categorie;
		}

		public async Task<List<int>> GetCategoriesByPost(int idPost)
		{
			// Récupération de toutes les catégories
			var commandText = @"SELECT categorieid "
							 + "FROM categorietopost "
							 + $"WHERE postid = {idPost};";

			Func<MySqlCommand, Task<List<int>>> funcCmd = async (cmd) =>
			{
				List<int> idCategorie = new List<int>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						int id = reader.GetInt32(0);
						idCategorie.Add(id);
					}
				}

				return idCategorie;
			};
			List<int> categories;

			try
			{
				categories = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return categories;
		}

		public async Task<List<Categorie>> GetCategories(int idPost)
		{
			// Récupération de toutes les catégories
			var commandText = @"SELECT cat.idcategorie, cat.nom "
							+ "FROM categorietopost cp "
							+ "INNER JOIN categories cat "
							+ "ON cp.categorieid = cat.idcategorie "
							+ $"WHERE cp.postid = {idPost};";

			Func<MySqlCommand, Task<List<Categorie>>> funcCmd = async (cmd) =>
			{
				List<Categorie> categories = new List<Categorie>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						Categorie categorie = new Categorie()
						{
							IdCategorie = reader.GetInt32(0),
							Nom = reader.GetString(1)
						};

						categories.Add(categorie);
					}
				}

				return categories;
			};
			List<Categorie> categories;

			try
			{
				categories = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return categories;
		}

		public async Task AddCategorieToPost(int idPost, IEnumerable<int> idCategorie)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					// Suppression de toutes lec catégories pour ce post
					string command = $"DELETE FROM categorietopost WHERE postid = {idPost};";
					
					using (var cmd = new MySqlCommand(command, conn))
					{
						conn.Open();
						await cmd.ExecuteNonQueryAsync();

						// Ajout des catégories pour le post.
						string commandInsert = "INSERT INTO categorietopost (postid, categorieid)"
									+ " VALUES(@idpost, @idcategorie);";

						cmd.CommandText = commandInsert;
						foreach (var categorie in idCategorie)
						{
							if (cmd.Parameters.Count > 0)
							{
								cmd.Parameters.Clear();
							}

							cmd.Parameters.AddWithValue("@idpost", idPost);
							cmd.Parameters.AddWithValue("@idcategorie", categorie);

							await cmd.ExecuteNonQueryAsync();
						}

						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<int> AddCategorie(Categorie nouvelleCategorie)
		{
			int idNewCategorie = 0;

			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					var command = @"INSERT INTO categories (nom) VALUES(@nom);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@nom", nouvelleCategorie.Nom);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();

						// Récupération de l'ID.
						string commandId = "SELECT LAST_INSERT_ID();";
						cmd.CommandText = commandId;
						idNewCategorie = Convert.ToInt32(await cmd.ExecuteScalarAsync());

						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			return idNewCategorie;
		}

		public async Task UpdateCategorie(Categorie item)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				var commandUpdateCompetence = @$"UPDATE categories SET nom=@name"
									  + $" WHERE idcategorie=@idcat;";

				using (var cmd = new MySqlCommand(commandUpdateCompetence, conn))
				{
					cmd.Parameters.AddWithValue("@name", item.Nom);
					cmd.Parameters.AddWithValue("@idcat", item.IdCategorie);
					
					conn.Open();
					await cmd.ExecuteNonQueryAsync();
					conn.Close();
				}
			}
		}

		public async Task DeleteCategorie(int idCategorie)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					// Suppression de la catégorie
					string command = $"DELETE FROM categories WHERE idcategorie = @idcat;";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@idcat", idCategorie);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		#endregion

		#region Export/Import Database

		public async Task<List<CategorieToPost>> GetCategoriesPosts()
		{
			// Récupération de toutes les catégories
			var commandText = @"SELECT postid, categorieid "
							 + "FROM categorietopost;";

			Func<MySqlCommand, Task<List<CategorieToPost>>> funcCmd = async (cmd) =>
			{
				List<CategorieToPost> allCategorieToPosts = new List<CategorieToPost>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						CategorieToPost item = new CategorieToPost()
						{
							PostId = reader.GetInt32(0),
							CategorieId = reader.GetInt32(1)
						};

						allCategorieToPosts.Add(item);
					}
				}

				return allCategorieToPosts;
			};
			List<CategorieToPost> categories;

			try
			{
				categories = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return categories;
		}

		public async Task<List<Post>> GetAllPostsAsync()
		{
			var commandText = @"SELECT idpost, title, content, image, posted, updateat, userid, ispublished "
							 + $"FROM posts;";

			Func<MySqlCommand, Task<List<Post>>> funcCmd = async (cmd) =>
			{
				List<Post> posts = new List<Post>();

				using (var reader = await cmd.ExecuteReaderAsync())
				{
					while (reader.Read())
					{
						// Cas ou le post n'est pas encore publié.
						object tempDate = reader.GetValue(4);
						DateTime? datePosted = ConvertFromDBVal<DateTime?>(tempDate);

						// Cas ou le post n'a pas de contenu.
						object tempContent = reader.GetValue(2);
						string content = ConvertFromDBVal<string>(tempContent);

						var post = new Post()
						{
							Id = reader.GetInt32(0),
							Title = reader.GetString(1),
							Content = content,
							Image = reader.GetString(3),
							Posted = datePosted,
							UpdatedAt = reader.GetDateTime(5),
							UserId = reader.GetString(6),
							IsPublished = reader.GetBoolean(7)
						};

						posts.Add(post);
					}
				}

				return posts;
			};

			List<Post> posts = new List<Post>();

			try
			{
				posts = await GetCoreAsync(commandText, funcCmd);
			}
			catch (Exception)
			{
				throw;
			}

			return posts;
		}


		public async Task DeleteAllDataForRestore()
		{
			List<string> cmdToDelet = new List<string>()
			{
				@"Delete from categorietopost;",
				@"Delete from categories;",
				@"Delete from posts;",
				@"Delete from settings;",
				@"Delete from Logs;"
			};

			using (var conn = new MySqlConnection(ConnectionString))
			{
				using (var cmd = new MySqlCommand())
				{
					cmd.Connection = conn;
					conn.Open();
				
					foreach (var deleteCmd in cmdToDelet)
					{
						cmd.CommandText = deleteCmd;
						await cmd.ExecuteNonQueryAsync();
					}
					conn.Close();
				}
			}
		}

		public async Task InsertRestore(List<Categorie> allCategories, List<Post> allPosts, 
						List<CategorieToPost> categoriesPosts)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					using (var cmd = new MySqlCommand())
					{
						cmd.Connection = conn;

						conn.Open();
						
						var cmdInsertCategories = @"INSERT INTO categories (idcategorie, nom) VALUES(@idcat, @nom);";
						cmd.CommandText = cmdInsertCategories;
						foreach (var cat in allCategories)
						{
							cmd.Parameters.AddWithValue("@idcat", cat.IdCategorie);
							cmd.Parameters.AddWithValue("@nom", cat.Nom);
							await cmd.ExecuteNonQueryAsync();
							cmd.Parameters.Clear();
						}

						var cmdInsertPosts = @"INSERT INTO posts (idpost, title, content, image, posted, updateat, userid, ispublished) "
									+ "VALUES(@idpost, @title, @content, @image, @posted, @updateat, @userid, @ispublished);";
						cmd.CommandText = cmdInsertPosts;
						foreach (var post in allPosts)
						{
							cmd.Parameters.AddWithValue("@idpost", post.Id);
							cmd.Parameters.AddWithValue("@title", post.Title);
							cmd.Parameters.AddWithValue("@content", post.Content);
							cmd.Parameters.AddWithValue("@image", post.Image);
							cmd.Parameters.AddWithValue("@posted", post.Posted);
							cmd.Parameters.AddWithValue("@updateat", post.UpdatedAt);
							cmd.Parameters.AddWithValue("@userid", post.UserId);
							cmd.Parameters.AddWithValue("@ispublished", post.IsPublished);
							await cmd.ExecuteNonQueryAsync();
							cmd.Parameters.Clear();
						}

						var cmdInsertCategorieToPost = @"INSERT INTO categorietopost (postid, categorieid) VALUES(@postid, @categorieid);";
						cmd.CommandText = cmdInsertCategorieToPost;
						foreach (var catPost in categoriesPosts)
						{
							cmd.Parameters.AddWithValue("@postid", catPost.PostId);
							cmd.Parameters.AddWithValue("@categorieid", catPost.CategorieId);
							await cmd.ExecuteNonQueryAsync();
							cmd.Parameters.Clear();
						}

						//var cmdInsertSettings = @"INSERT INTO settings (settingname, settingvalue) VALUES(@nom, @valeur);";						
						//cmd.CommandText = cmdInsertSettings;
						//foreach (var setting in allSettings)
						//{
						//	cmd.Parameters.AddWithValue("@nom", setting.SettingName);
						//	cmd.Parameters.AddWithValue("@valeur", setting.Value);
						//	await cmd.ExecuteNonQueryAsync();
						//	cmd.Parameters.Clear();
						//}

						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		#endregion

		#region Profils

		public async Task<Profil> GetProfil(string userid)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					var command = @$"SELECT userid, imgprofil, profilcontent FROM profils WHERE userid='{userid}';";

					using (var cmd = new MySqlCommand(command, conn))
					{
						Profil profil = null;
						await conn.OpenAsync();
						using (var reader = await cmd.ExecuteReaderAsync())
						{
							while (reader.Read())
							{
								profil = new Profil()
								{
									UserId = reader.GetString(0),
									UrlImage = reader.GetString(1),
									ContentProfil = reader.GetString(2)
								};
							}
						}

						await conn.CloseAsync();
						return profil;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Récupère l'avatar de cet auteur.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<string> GetProfilImage(string userId)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					var command = @$"SELECT imgprofil FROM profils WHERE userid='{userId}';";

					using (var cmd = new MySqlCommand(command, conn))
					{
						await conn.OpenAsync();
						string img = string.Empty;
						using (var reader = await cmd.ExecuteReaderAsync())
						{
							while (reader.Read())
							{
								img = reader.GetString(0);
							}
						}

						await conn.CloseAsync();
						return img;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task AddImgProfil(string userid, string urlImg)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string cmdIfExist = $"SELECT COUNT(userid) FROM profils WHERE userid='{userid}'";

					int counter = await GetIntCore(cmdIfExist);
					if (counter > 0)
					{
						// Utilisateur déjà créé, donc faire un UPDATE
						var command = @"UPDATE profils SET imgprofil=@img WHERE userid=@user;";

						using (var cmd = new MySqlCommand(command, conn))
						{
							cmd.Parameters.AddWithValue("@user", userid);
							cmd.Parameters.AddWithValue("@img", urlImg);

							conn.Open();
							await cmd.ExecuteNonQueryAsync();
							conn.Close();
						}
					}
					else
					{
						var command = @"INSERT INTO profils (userid, imgprofil) VALUES(@user, @img);";

						using (var cmd = new MySqlCommand(command, conn))
						{
							cmd.Parameters.AddWithValue("@user", userid);
							cmd.Parameters.AddWithValue("@img", urlImg);

							conn.Open();
							await cmd.ExecuteNonQueryAsync();
							conn.Close();
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task AddProfilContent(string userid, string content)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string cmdIfExist = $"SELECT COUNT(userid) FROM profils WHERE userid='{userid}'";

					int counter = await GetIntCore(cmdIfExist);
					if (counter > 0)
					{
						// Utilisateur déjà créé, donc faire un UPDATE
						var command = @"UPDATE profils SET profilcontent=@content WHERE userid=@user);";

						using (var cmd = new MySqlCommand(command, conn))
						{
							cmd.Parameters.AddWithValue("@user", userid);
							cmd.Parameters.AddWithValue("@content", content);

							conn.Open();
							await cmd.ExecuteNonQueryAsync();
							conn.Close();
						}
					}
					else
					{
						var command = @"INSERT INTO profils (userid, profilcontent) VALUES(@user, @content);";

						using (var cmd = new MySqlCommand(command, conn))
						{
							cmd.Parameters.AddWithValue("@user", userid);
							cmd.Parameters.AddWithValue("@content", content);

							conn.Open();
							await cmd.ExecuteNonQueryAsync();
							conn.Close();
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion

		#region Tracks

		public async Task SaveTracks(Track track)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					var command = @"INSERT INTO tracks (visitorid, postid, daterequested) VALUES(@visitor, @idpost, @date);";

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@visitor", track.VisitorId);
						cmd.Parameters.AddWithValue("@idpost", track.PostId);
						cmd.Parameters.AddWithValue("@date", track.DateRequest);

						conn.Open();
						await cmd.ExecuteNonQueryAsync();
						conn.Close();
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<Counter> GetCounterDay(string userId)
		{
			try
			{
				return await GetCounterCore("DAY", userId);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<Counter> GetCounterWeek(string userId)
		{
			try
			{
				return await GetCounterCore("WEEK", userId);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<Counter> GetCounterMonth(string userId)
		{
			try
			{
				return await GetCounterCore("MONTH", userId);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<Counter> GetCounterYear(string userId)
		{
			try
			{
				return await GetCounterCore("YEAR", userId);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private async Task<Counter> GetCounterCore(string interval, string userId)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					// Pour avoir un compteur sur les dernières 24h
					var command = $"SELECT COUNT(id) "
					+ "FROM tracks trk "
					+ "INNER JOIN posts post "
					+ "ON trk.postid = post.idpost "
					+ $"WHERE post.userid = '{userId}' "
					+ $"AND daterequested > DATE_SUB(NOW(), INTERVAL 1 {interval});";

					using (var cmd = new MySqlCommand(command, conn))
					{
						Counter counter = new Counter();
						conn.Open();
						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (reader.Read())
								{
									counter.CompteurActuel = reader.GetInt32(0);
								}
							}
						}

						string cmdJourAvant = @"SELECT COUNT(id) "
											+ "FROM tracks trk "
											+ "INNER JOIN posts post "
											+ "ON trk.postid = post.idpost "
											+ $"WHERE post.userid = '{userId}' "
											+ $"AND daterequested BETWEEN DATE_SUB((DATE_SUB(NOW(), INTERVAL 1 {interval})), INTERVAL 1 {interval}) AND DATE_SUB(NOW(), INTERVAL 1 {interval});";
						cmd.CommandText = cmdJourAvant;

						int compteurComparaison = 0;
						using (var readerDiff = await cmd.ExecuteReaderAsync())
						{
							if (readerDiff.HasRows)
							{
								while (readerDiff.Read())
								{
									compteurComparaison = readerDiff.GetInt32(0);
								}
							}
						}

						counter.Difference = counter.CompteurActuel - compteurComparaison;

						await conn.CloseAsync();
						return counter;
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}


		public async Task<CounterPost> GetCounterPostDay(string userId)
		{
			return await GetCounterPostCore("DAY", userId);
		}

		public async Task<CounterPost> GetCounterPostMonth(string userId)
		{
			return await GetCounterPostCore("MONTH", userId);
		}


		private async Task<CounterPost> GetCounterPostCore(string interval, string userId)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					var command = "SELECT post.title, COUNT(trk.postid) as Compteur "
									+ "FROM tracks trk "
									+ "INNER JOIN posts post "
									+ "ON trk.postid = post.idpost "
									+ $"WHERE post.userid = '{userId}' "
									+ $"AND daterequested > DATE_SUB(NOW(), INTERVAL 1 {interval}) "
									+ "GROUP BY trk.postid "
									+ "ORDER BY Compteur DESC LIMIT 1;";

					using (var cmd = new MySqlCommand(command, conn))
					{
						CounterPost counter = new CounterPost();
						conn.Open();
						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (reader.Read())
								{
									counter.Title = reader.GetString(0);
									counter.Count = reader.GetInt32(1);
								}
							}
						}

						await conn.CloseAsync();
						return counter;
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}


		
		public async Task<List<KeyValuePair<string, double>>> GetCounter(string userId, int jour)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					var command = "SELECT COUNT(trk.id), DATE_FORMAT(trk.daterequested, '%d-%b') "
									+ "FROM posts post "
									+ "LEFT JOIN tracks trk "
									+ "ON trk.postid = post.idpost "
									+ $"WHERE post.userid = '{userId}' "
									+ $"AND trk.daterequested > DATE_SUB(NOW(), INTERVAL {jour} DAY) "
									+ "GROUP BY DATE_FORMAT(trk.daterequested, '%d-%b');";

					using (var cmd = new MySqlCommand(command, conn))
					{
						List<KeyValuePair<string, double>> valuePairs = new List<KeyValuePair<string, double>>();
						
						conn.Open();
						using (var reader = await cmd.ExecuteReaderAsync())
						{
							if (reader.HasRows)
							{
								while (reader.Read())
								{
									KeyValuePair<string, double> item = KeyValuePair.Create(reader.GetString(1), Convert.ToDouble(reader.GetInt32(0)));
									valuePairs.Add(item);
								}
							}
						}

						await conn.CloseAsync();
						return valuePairs;
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		
		#endregion

		#region Private methods

		private async Task<List<T>> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<List<T>>> func)
			where T : new()
		{
			List<T> result = new List<T>();

			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					MySqlCommand cmd = new MySqlCommand(commandSql, conn);
					conn.Open();
					result = await func.Invoke(cmd);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return result;
		}

		/// <summary>
		/// Execute une commande avec le retour passé.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="commandSql"></param>
		/// <param name="func"></param>
		/// <returns></returns>
		private async Task<T> GetCoreAsync<T>(string commandSql, Func<MySqlCommand, Task<T>> func)
			where T : new()
		{
			T result = new T();

			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					MySqlCommand cmd = new MySqlCommand(commandSql, conn);
					conn.Open();
					result = await func.Invoke(cmd);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return result;
		}

		/// <summary>
		/// Execute une commande qui n'attend pas de retour.
		/// </summary>
		/// <param name="commandSql"></param>
		/// <returns></returns>
		private async Task<int> ExecuteCoreAsync(string commandSql)
		{
			using (var conn = new MySqlConnection(ConnectionString))
			{
				MySqlCommand cmd = new MySqlCommand(commandSql, conn);

				conn.Open();
				return await cmd.ExecuteNonQueryAsync();
			}
		}

		/// <summary>
		/// Permet la récupération d'un BLOB uniquement !
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		private async Task<byte[]> GetBytesCore(string commandText)
		{
			byte[] file = null;

			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					conn.Open();
					MySqlCommand cmd = new MySqlCommand(commandText, conn);

					using (var reader = cmd.ExecuteReader())
					{
						while (await reader.ReadAsync())
						{
							file = (byte[])reader[0];

						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			return file;
		}

		/// <summary>
		/// Permet de gérer les retours de valeur null de la BDD
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static T ConvertFromDBVal<T>(object obj)
		{
			if (obj == null || obj == DBNull.Value)
			{
				return default(T);
			}
			else
			{
				return (T)obj;
			}
		}

		/// <summary>
		/// Permet la récupération d'un ID type int uniquement !
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns></returns>
		private async Task<int> GetIntCore(string commandText)
		{
			int id = 0;

			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					conn.Open();
					MySqlCommand cmd = new MySqlCommand(commandText, conn);

					UInt64 idTemp = 0;

					using (var reader = cmd.ExecuteReader())
					{
						while (await reader.ReadAsync())
						{
							idTemp = reader.GetUInt64(0);
						}
					}

					id = Convert.ToInt32(idTemp);
				}
			}
			catch (Exception)
			{
				throw;
			}

			return id;
		}

		#endregion
	}
}
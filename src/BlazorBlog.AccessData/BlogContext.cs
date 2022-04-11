using BlazorBlog.Core;
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

		#region Posts

		/// <summary>
		/// Récupère tous les posts
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Post>> GetPostsAsync()
		{
			var commandText = @"SELECT idpost, title, content, image, createat, updateat, userid, ispublished "
							 + "FROM posts;";

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
							Content = reader.GetString(2),
							Image = reader.GetString(3),							
							CreatedAt = reader.GetDateTime(4),
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

		/// <summary>
		/// Récupère tous les posts qui sont publiés
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
		{
			var commandText = @"SELECT idpost, title, image, createat, userid "
							 + "FROM posts "
							 + "WHERE ispublished=1;";

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
							CreatedAt = reader.GetDateTime(3),
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
		/// Récupère le post par rapport à son id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Post> GetPostAsync(int id)
		{
			var commandText = @"SELECT idpost, title, content, image, createat, updateat, userid, ispublished "
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
							CreatedAt = reader.GetDateTime(4),
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
		/// <returns></returns>
		public async Task<Post> AddPostAsync(Post nouveauPost)
		{
			try
			{
				using (var conn = new MySqlConnection(ConnectionString))
				{
					string command = "INSERT INTO posts (title, content, image, createat, updateat, userid, ispublished)"
									+ " VALUES(@title, @content, @image, @createat, @updateat, @userid, @ispublished);";

					nouveauPost.CreatedAt = DateTime.Now;
					nouveauPost.UpdatedAt = nouveauPost.CreatedAt;

					using (var cmd = new MySqlCommand(command, conn))
					{
						cmd.Parameters.AddWithValue("@title", nouveauPost.Title);
						cmd.Parameters.AddWithValue("@content", nouveauPost.Content);
						cmd.Parameters.AddWithValue("@image", nouveauPost.Image);
						cmd.Parameters.AddWithValue("@createat", nouveauPost.CreatedAt);
						cmd.Parameters.AddWithValue("@updateat", nouveauPost.UpdatedAt);
						cmd.Parameters.AddWithValue("@userid", nouveauPost.UserId.ToString());
						cmd.Parameters.AddWithValue("@ispublished", nouveauPost.IsPublished);

						conn.Open();
						int result = await cmd.ExecuteNonQueryAsync();
						conn.Close();
					}
				}

				string commandId = " SELECT LAST_INSERT_ID();";
				nouveauPost.Id = await GetIntCore(commandId);
			}
			catch (Exception)
			{
				throw;
			}

			return nouveauPost;
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
							idTemp = (UInt64)reader[0];
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
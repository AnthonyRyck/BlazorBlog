namespace BlazorBlog.Codes
{
	public static class ConstantesApp
	{
		public const string IMAGES = "images";
		public const string USERIMG = "/userimg";

		#region Markdown Syntax

		public const string MARKDOWN_SYNTAX_BOLD = "**";
		public const string MARKDOWN_SYNTAX_ITALIC = "*";
		public const string MARKDOWN_SYNTAX_STRIKETHROUGH = "~~";
		public const string MARKDOWN_SYNTAX_H1 = "#";
		public const string MARKDOWN_SYNTAX_H2 = "##";
		public const string MARKDOWN_SYNTAX_H3 = "###";
		public const string MARKDOWN_SYNTAX_H4 = "####";
		public const string MARKDOWN_SYNTAX_H5 = "#####";
		public const string MARKDOWN_SYNTAX_H6 = "######";
		public const string MARKDOWN_SYNTAX_UNDERLINE = "__";
		public const string MARKDOWN_SYNTAX_CODE_IN_LINE = "`";
		public const string MARKDOWN_SYNTAX_BLOCK_CODE = "```";
		public const string MARKDOWN_SYNTAX_QUOTE = ">";
		public const string MARKDOWN_SYNTAX_LINK = "[]()";
		public const string MARKDOWN_SYNTAX_LINK_START = "[";
		public const string MARKDOWN_SYNTAX_LINK_END = "]()";
		public const string MARKDOWN_SYNTAX_IMAGE_START = "![";
		public const string MARKDOWN_SYNTAX_IMAGE_END = "]()";
		public const string MARKDOWN_SYNTAX_IMAGE = "![]()";
		public const string MARKDOWN_SYNTAX_LIST_BULLET = "- ";
		public const string MARKDOWN_SYNTAX_LIST_ORDERED = "1. ";

		#endregion

		#region Settings

		public const string SETTINGS_BLOG_NAME = "blogname";
		public const string SETTINGS_BLOG_DESCRIPTION = "blogdescription";
		public const string SETTINGS_BLOG_IMAGE = "blogimage";
		public const string SETTINGS_BLOG_URL = "blogurl";
		public const string SETTINGS_BLOG_ICONE = "blogicone";

		#endregion

		#region Extensions Image

		public const string EXTENSION_IMAGE_JPG = ".jpg";
		public const string EXTENSION_IMAGE_JPEG = ".jpeg";
		public const string EXTENSION_IMAGE_PNG = ".png";
		public const string EXTENSION_IMAGE_GIF = ".gif";
		public const string EXTENSION_IMAGE_BMP = ".bmp";
		public const string EXTENSION_IMAGE_ICO = ".ico";
		public const string EXTENSION_IMAGE_SVG = ".svg";

		#endregion

		#region MIME Types Images

		public const string MIME_JPG = "image/jpg";
		public const string MIME_JPEG = "image/jpeg";
		public const string MIME_GIF = "image/gif";
		public const string MIME_PNG = "image/png";

		#endregion

		#region Nom de fichier pour Export

		public const string EXPORT_EXTENSION = ".json";
		public const string EXPORT_POSTS = "Posts.json";
		public const string EXPORT_SETTINGS = "Settings.json";
		public const string EXPORT_CATEGORIES = "Categories.json";
		public const string EXPORT_USERS = "Users.json";
		public const string EXPORT_CATEGORIES_TO_POSTS = "CategorieToPosts.json";

		#endregion

		#region Role pour les comptes

		public const string ROLE_ADMIN = "Admin";
		public const string ROLE_AUTEUR = "Compositeur";

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorBlog.Core.EntityView
{
	public class PostView : Post
	{
		public List<Categorie> Categories { get; set; }
	}
}

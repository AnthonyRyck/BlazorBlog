using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorBlog.Core
{
	public class Categorie
	{
		public int IdCategorie { get; set; }

		public string Nom { get; set; }

		public bool IsSelected { get; set; }
	}
}

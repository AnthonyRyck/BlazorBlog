using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorBlog.Core
{
	public class Track
	{
		public uint Id { get; set; }
		public string VisitorId { get; set; }
		public int PostId { get; set; }
		public DateTime DateRequest { get; set; }
		
	}
}

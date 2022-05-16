namespace BlazorBlog.Core
{
	public struct Counter
	{
		public int CompteurActuel { get; set; }
		public int Difference { get; set; }


		public string Sign { get { return Difference > 0 ? "+" : "-"; } }
	}
}

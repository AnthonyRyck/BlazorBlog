namespace BlazorBlog.Codes
{
	public static class Helpers
	{
		/// <summary>
		/// Retourne la taille d'un fichier en Mo, Go,...
		/// en fonction de ça taille.
		/// </summary>
		/// <param name="octets"></param>
		/// <returns></returns>
		public static string GetSize(long octets)
		{
			string[] Suffix = { "octets", "Ko", "Mo", "Go", "To" };
			int i;
			double dblSOctet = octets;

			for (i = 0; i < Suffix.Length && octets >= 1024; i++, octets /= 1024)
			{
				dblSOctet = octets / 1024.0;
			}

			return String.Format("{0:0.##} {1}", dblSOctet, Suffix[i]);
		}
	}
}

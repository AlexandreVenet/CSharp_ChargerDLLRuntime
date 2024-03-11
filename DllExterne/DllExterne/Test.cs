namespace DllExterne
{
	public class Test
	{
		public string Methode()
		{
			return $"Je suis la méthode {this.GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod()?.Name}().";
		}

		public string RenvoyerTexte(string texte)
		{
			return texte;
		}
	}
}

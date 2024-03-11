using System.Reflection;

namespace App
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			string nomApp = typeof(Program).Namespace;
			Console.Title = nomApp;
			EcrireDetailProgramme("Début du programme : " + nomApp);

			//new Exemple1().Lancer();
			new Exemple2().Lancer();

			EcrireDetailProgramme("Fin du programme : " + nomApp);
			Console.Read();
		}



		#region Méthodes pour l'UI

		internal static void EcrireTitre1(string texte)
		{
			EcrireEnCouleur(texte, ConsoleColor.DarkYellow);
		}

		internal static void EcrireTitre2(string texte)
		{
			EcrireEnCouleur(texte, ConsoleColor.DarkCyan);
		}

		internal static void EcrireDetail(string texte)
		{
			EcrireEnCouleur(texte, ConsoleColor.DarkGray);
		}

		internal static void EcrireEnCouleur(string texte, ConsoleColor couleur)
		{
			Console.ForegroundColor = couleur;
			Console.WriteLine(texte);
			Console.ResetColor();
		}

		internal static void Ecrire(string texte)
		{
            Console.WriteLine(texte);
        }

		internal static void EcrireDetailProgramme(string texte)
		{
			int longueurTexte = texte.Length;
			string separateur = new string('=', longueurTexte);

			EcrireDetail(separateur);
			EcrireDetail(texte);
			EcrireDetail(separateur);
			EcrireNouvelleLigne();
		}

		internal static void EcrireNouvelleLigne()
		{
            Console.WriteLine();
        }

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App
{
	internal class Exemple1
	{
		internal void Lancer()
		{
			// Obtenir le chemin du fichier DLL du projet DllExterne.
			string dossierDuProgramme = ObtenirCheminDuProgramme();
			//string dossierDuProgramme = ObtenirCheminDuProgrammeAlternative();
			string dossierDeTravail = ObtenirCheminDossierDeTravail(dossierDuProgramme);
			string cheminDLLExterne = ObtenirCheminDLLExterne(dossierDeTravail);

			// Charger le fichier DLL
			Assembly dllExterne = Assembly.LoadFile(cheminDLLExterne);

			// Explorer le premier et seul type exporté : nom et namespace
			Program.EcrireTitre1("Types exportés");
			Type[] typesExportes = dllExterne.GetExportedTypes();
			foreach (Type item in typesExportes)
			{
				Program.Ecrire(item.Name);
			}
			Program.EcrireNouvelleLigne();

			Type typeExporte = typesExportes[0];
			string typeExporteNom = typeExporte.Name;
			string typeExporteNamespace = typeExporte.Namespace;
			Program.EcrireTitre1("Type exporté " + typeExporteNom);
			Program.Ecrire("Nom du type : " + typeExporteNom);
			Program.Ecrire("Nom du namespace : " + typeExporteNamespace);
			Program.EcrireNouvelleLigne();

			// Instancier ce type (Activator requiert le type).
			Type type = dllExterne.GetType($"{typeExporteNamespace}.{typeExporteNom}");
			object instance = Activator.CreateInstance(type);
			Program.EcrireTitre1("Instanciation du type");
			Program.Ecrire("OK");
			Program.EcrireNouvelleLigne();

			// Explorer du type ses membres d'instance, publics, hors héritage (comprend le constructeur)
			Program.EcrireTitre1("Exploration des membres du type");
			MemberInfo[] membres = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			foreach (MemberInfo mi in membres)
			{
				Program.Ecrire($"{mi.Name} ({mi.MemberType.ToString()})");
			}
			Program.EcrireNouvelleLigne();

			// Appeler la méthode Test.Methode()
			MethodInfo methode = type.GetMethod(membres[0].Name);
			string resultat = methode.Invoke(instance, null).ToString();
			Program.EcrireTitre1("Appel de Test.Methode()");
			Program.Ecrire(resultat);
			Program.EcrireNouvelleLigne();

			// Appeler la méthode Test.RenvoyerTexte(string texte)
			methode = type.GetMethod(membres[1].Name);
			resultat = methode.Invoke(instance, new object[] { "Super texte !" }).ToString();
			Program.EcrireTitre1("Appel de Test.RenvoyerTexte(string texte)");
			Program.Ecrire(resultat);
			Program.EcrireNouvelleLigne();
		}

		private string ObtenirCheminDuProgramme()
		{
			string dossierDuProgramme = AppDomain.CurrentDomain.BaseDirectory;

			Program.EcrireTitre1("Dossier du programme");
			Program.Ecrire(dossierDuProgramme);
			Program.EcrireNouvelleLigne();

			return dossierDuProgramme;
		}

		private string ObtenirCheminDuProgrammeAlternative()
		{
			// Obtenir le chemin de dossier du fichier manifeste.
			// Puis, en obtenir le chemin de dossier.

			string cheminDossierManifeste = Assembly.GetExecutingAssembly().Location;

			Program.EcrireTitre1("Chemin du dossier du manifeste");
			Program.Ecrire(cheminDossierManifeste);

			string dossierDuProgramme = Path.GetDirectoryName(cheminDossierManifeste);

			Program.EcrireTitre1("Dossier du programme");
			Program.Ecrire(dossierDuProgramme);

			return dossierDuProgramme;
		}

		private string ObtenirCheminDossierDeTravail(string cheminDossierProgramme)
		{
			DirectoryInfo cheminProgrammeDirectoryInfo = new(cheminDossierProgramme);
			DirectoryInfo racineDossierDeTravail = cheminProgrammeDirectoryInfo.Parent.Parent.Parent.Parent.Parent;

			Program.EcrireTitre1("Dossier de travail");
			Program.Ecrire(racineDossierDeTravail.FullName);
			Program.EcrireNouvelleLigne();

			return racineDossierDeTravail.FullName;
		}

		private string ObtenirCheminDLLExterne(string cheminDossierDeTravail)
		{
			string cheminComplet = Path.Combine(cheminDossierDeTravail, @"DllExterne\DllExterne\bin\Debug\net8.0\DllExterne.dll");

			Program.EcrireTitre1("Chemin du fichier DLL externe");
			Program.Ecrire(cheminComplet);
			Program.EcrireNouvelleLigne();

			return cheminComplet;
		}
	}
}

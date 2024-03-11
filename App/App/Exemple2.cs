using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App
{
	internal class Exemple2
	{
		private string _espacedeNomDeLaDLL;
		private Assembly _dll;

		internal void Lancer()
		{
			// On admet que le fichier DLL a été copié préalablement dans le dossier "/Modules" 

			DefinirEspaceDeNomDeLaDLL("TrollTolkien");

			// Charger la librairie
			string cheminVersDLL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", $"{_espacedeNomDeLaDLL}.dll");
			_dll = Assembly.LoadFile(cheminVersDLL);

			AfficherTypesExportes();

			// Explorer le type Test
			Type typeTest = ObtenirType("Test");
			AfficherMembres(typeTest);
			
			// Instancier Test
			object instanceTest = Instancier(typeTest);
			Program.EcrireTitre2("Instanciation de " + typeTest.FullName);
			Program.Ecrire("OK");
			Program.EcrireNouvelleLigne();

			// Lancer une méthode de l'instance de Test
			string resultatTest = LancerMethode(typeTest, instanceTest, "AnalyserTexte", new object[] { "Coucou !" }).ToString();
			Program.EcrireTitre2("AnalyserMethode()");
			Program.Ecrire("> " + resultatTest);
			Program.EcrireNouvelleLigne();

			// Le type de TrollSauron
			Type trollSauronType = ObtenirType("Trolls.TrollSauron");

			// Explorer TrollSauron en tant qu'héritant de Troll
			Troll troll = (Troll) Instancier(trollSauronType);
			Program.EcrireTitre2("Instance de TrollSauron en tant que Troll");
			Program.Ecrire(troll.Dire());
			Program.EcrireNouvelleLigne();

			// Explorer TrollSauron avec Reflection
			AfficherMembres(trollSauronType);
		}

		private void DefinirEspaceDeNomDeLaDLL(string espaceDeNom)
		{
			_espacedeNomDeLaDLL = espaceDeNom;

			Program.EcrireTitre2("Espace de nom de la librairie");
			Program.Ecrire(_espacedeNomDeLaDLL);
			Program.EcrireNouvelleLigne();
		}

		private void AfficherTypesExportes()
		{
			Type[] typesExportes = _dll.GetExportedTypes();

			Program.EcrireTitre2("Types exportés");
			foreach (Type type in typesExportes)
			{
				Program.Ecrire(type.FullName);
			}
			Program.EcrireNouvelleLigne();
		}

		private Type ObtenirType(string nomType)
		{
			Type type = _dll.GetType($"{_espacedeNomDeLaDLL}.{nomType}");
			return type;
		}

		private void AfficherMembres(Type type)
		{
			Program.EcrireTitre2("Membres du type " + type.FullName);

			// Explorer du type ses membres d'instance, publics, hors héritage (comprend le constructeur)
			MemberInfo[] membres = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

			// v.1
			
			//foreach (MemberInfo mi in membres)
			//{
			//	Program.Ecrire($"{mi.Name} ({mi.MemberType})");
			//}
			// Program.EcrireNouvelleLigne();

			// v.2

			for (int i = 0; i < membres.Length; i++)
			{
				MemberInfo x = membres[i];

				if (x.MemberType != MemberTypes.Method) continue; 
				// On ne veut que les méthodes.
				// On pourrait dès le départ utiliser type.GetMethods() plutôt que type.GetMembers().

				MethodInfo methode = (MethodInfo) x;

				StringBuilder sb = new();

				if (methode.IsPublic) sb.Append("public ");
				if (methode.IsAbstract) sb.Append("abstract ");
				if (methode.IsStatic) sb.Append("static ");
				if (methode.IsFinal) sb.Append("sealed ");

				string typeRetour = methode.ReturnType.Name;
				// Exemples : String, Tuple`2, String[]...
				// On doit donc déterminer le type de retour selon la présence ou non de l'accent grave.
				string typeRetourneOK = null;
				int indexAccentGrave = typeRetour.IndexOf('`');
				if(indexAccentGrave == -1)
				{
					typeRetourneOK = typeRetour;
				}
				else
				{
					// Exemple : Tuple`2
					StringBuilder sbRetour = new();
					sbRetour.Append(typeRetour.Substring(0, indexAccentGrave)); // Tuple
					sbRetour.Append('<');
					// Chercher les types utilisés
					string types = string.Empty;
					PropertyInfo[] proprietesDeRetour = methode.ReturnType.GetProperties();
					for (int j = 0; j < proprietesDeRetour.Length; j++)
					{
						PropertyInfo item = proprietesDeRetour[j];
						//Trace.WriteLine(item.PropertyType); // Ex : System.Int32
						string typeNom = item.PropertyType.Name; // Ex : Int32
						types += typeNom;
						if (j < proprietesDeRetour.Length - 1)
						{
							types += ", ";
						}
					}
					sbRetour.Append(types);
					sbRetour.Append('>');
					typeRetourneOK = sbRetour.ToString();
				}
				sb.Append(typeRetourneOK);

				sb.Append(' ');

				sb.Append(methode.Name);
				sb.Append('(');
				string parametresSignature = string.Empty;
				ParameterInfo[] parametres = methode.GetParameters();
				for (int j = 0; j < parametres.Length; j++)
				{
					ParameterInfo item = parametres[j];

					string nomDuParam = item.ParameterType.Name;

					if(item.IsOut) parametresSignature += "out ";
					if (item.ParameterType.IsByRef)
					{
						parametresSignature += nomDuParam.Substring(0, nomDuParam.Length - 1); // Ex : boolean (et non pas boolean& signifiant le passage par référence)
					}
					else
					{
						parametresSignature += nomDuParam;
					}
					if (item.IsOptional) parametresSignature += "?";
					parametresSignature += " ";
					parametresSignature += item.Name;
					if (item.HasDefaultValue)
					{
						object valeurParDefaut = item.DefaultValue;
						string valeurParDefautSortie = null;
						if (valeurParDefaut == null)
						{
							valeurParDefautSortie = "null";
						}
						else
						{
							valeurParDefautSortie = valeurParDefaut.ToString();
						}
						parametresSignature += $" = {valeurParDefautSortie}";
					}

					if (j < parametres.Length - 1)
					{
						parametresSignature += ", ";
					}

				}
				sb.Append(parametresSignature);
				sb.Append(')');

				Program.Ecrire(sb.ToString());
			}

			Program.EcrireNouvelleLigne();
		}

		private object Instancier(Type type)
		{
			return Activator.CreateInstance(type);
		}

		private object LancerMethode(Type type, object instance, string nom, object[] parametres = null)
		{
			MethodInfo methode = type.GetMethod(nom);

			if (methode == null) // exemple si appel invalide
			{
				throw new InvalidOperationException($"La méthode {methode} est introuvable.");
			}

			return methode.Invoke(instance, parametres);
		}
	}
}

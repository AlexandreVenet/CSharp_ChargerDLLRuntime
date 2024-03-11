using App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrollTolkien.Trolls
{
	public class TrollSauron : Troll
	{
		public override sealed string Dire()
		{
			return "Je suis un troll de Sauron. Je suis très méchant. Roar !";
		}

		// Autres tests

		public string Rugir() => "Roar !";
	
		private void MethodePrivee() { }

		public static void MethodePriveeStatique() { }

		public void Autre(int p1, out bool etat, string p2 = null, bool p3 = true)
		{
			etat = false;
		}

		internal void MethodeInternal() { }

		public Tuple<string, int> youpi() => null; 

		public string[] Hoho(string[] chaines) => null; 
	}
}

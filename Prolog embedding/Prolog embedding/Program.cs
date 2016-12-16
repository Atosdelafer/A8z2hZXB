using System;
using System.Collections.Generic;

namespace PrologEmbedding
{
	class Program
    {
		// Index containing all term trees
		static TermTreeIndex index = new TermTreeIndex ();
		// Dictionary containing all clause tails, heads are in index
		static Dictionary<int, TermTree[]> clauseTails = new Dictionary<int, TermTree[]> ();

        static void Main(string[] args)
        {
			//storeClauseTail ("foo(bar),doz(coo)", 0);
			String[] filecontent = FileReader();
			int linenumber = 0;
			foreach (string line in filecontent) {
				var ClauseCheck = ClauseValidator.isValidAndDecomposesClause (line);
				if (ClauseValidator.isValidCompound (line)) {                        
					//Console.WriteLine ("Compound");
					index.addTermTree (new TermTree(line), linenumber);

					linenumber++;
				} else if (ClauseCheck.Item1) {                        
					//Console.WriteLine ("Clause");
					index.addTermTree (new TermTree(ClauseCheck.Item2), linenumber);
					storeClauseTail (ClauseCheck.Item3, linenumber);
					// new clause rail function: addToClauseTree(Newtree, ClauseCheck.Item3, linenumber, 0);
					linenumber++;
				}
			}

			// Outputing index
			Console.Write (index.toString ());
			// Outputing clause tails
			foreach (KeyValuePair<int, TermTree[]> treeArray in clauseTails) {
				for (int i = 0; i < treeArray.Value.Length; i++) {
					Console.WriteLine (treeArray.Value [i].toString () + ":" + treeArray.Key);
				}
			}
        }

		static private void storeClauseTail(string tail, int linenumber)
		{
			int level = 0;
			string buffer = "";
			List<TermTree> children = new List<TermTree> ();

			foreach (char c in tail) {
				if (c == '(')
					level++;
				else if (c == ')')
					level--;
				else if (c == ',' && level == 0) {
					children.Add (new TermTree (buffer));
					buffer = "";
				} 

				if (!(c == ',' && level == 0)) {
					buffer += c;
				}
			}

			if (buffer != "") {
				children.Add (new TermTree (buffer));
			}

			Program.clauseTails.Add (linenumber, children.ToArray ());
		}

        static private String[] FileReader()
        {
            bool endsInPeriod = true;
            char[] charsToTrim = {'.', ' '};
            string[] lines = System.IO.File.ReadAllLines(@"testfile.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                endsInPeriod = lines[i].EndsWith(".");
                if (endsInPeriod)
                {
                    lines[i] = lines[i].TrimEnd(charsToTrim);
                }
                else
                {
                    lines[i] = "";
                }
            }                     
            return lines;            
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;

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

				if (!line.Contains (":-")) {

					if (ClauseValidator.isValidCompound (line)) {
						Console.WriteLine ("Compound");
						TermTree test = new TermTree (line);

						index.addTermTree (test, linenumber);
						linenumber++;
					}
						
				} else {                        
					var ClauseCheck = ClauseValidator.isValidAndDecomposesClause (line);
					if (ClauseCheck.Item1) {
							
						Console.WriteLine ("Clause");

						index.addTermTree (new TermTree (ClauseCheck.Item2), linenumber);
						storeClauseTail (ClauseCheck.Item3, linenumber);
						// new clause rail function: addToClauseTree(Newtree, ClauseCheck.Item3, linenumber, 0);
						linenumber++;
					}
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
            inference();

        }

        static private void inference()
        {
            string inputline;
            int[] matchresult;
            //bool[] inKnowledgeBase;
            List<bool> inKnowledgeBase = new List<bool>();
            
            while (true)
            {
                inputline = Console.ReadLine();
				matchresult = index.getMatchingTrees(new TermTree(inputline));

                inKnowledgeBase = headRecursion(matchresult);
                foreach (bool item in inKnowledgeBase)
                {
                    Console.WriteLine(item);
                }
            }        
        }

        static private List<bool> headRecursion(int[] matchresult)
        {
            List<bool> recursionList = new List<bool>();
            if (matchresult.Length == 0)
            {
                recursionList.Add(false);
            }
            else
            {
                //for (int i = 0; i < matchresult.Length; i++)
                foreach (int i in matchresult)
                {
                    if (!clauseTails.ContainsKey(i))
                    {
                        recursionList.Add(true);
                    }
                    else 
                    {
                        int clauseTailsLength = clauseTails[i].Length;
                        if (clauseTailsLength > 1)
                        {
                            //split
                            List<bool>[] multipleClauses = new List<bool>[clauseTailsLength];
                            int[] countingmechanism = new int[clauseTailsLength];
                            int[] countingmechanismcap = new int[clauseTailsLength];
                            int numberofrounds = 1;
                            for (int j = 0; j < clauseTailsLength; j++)
                            {
                                multipleClauses[j] = headRecursion(index.getMatchingTrees(clauseTails[i][j]));
                                countingmechanismcap[j] = multipleClauses[j].Count;
                                numberofrounds *= multipleClauses[j].Count;
                            }
                            //merge
                            int currentslot = clauseTailsLength - 1;
                            for (int k = 0; k < numberofrounds; k++)
                            {
                                bool finalcheck = true;

                                for (int z = 0; z < clauseTailsLength; z++)
                                {
                                    if (!multipleClauses[z].ElementAt(countingmechanism[z]))
                                    {
                                        finalcheck = false;
                                    }
                                }
                                
                                if (finalcheck)
                                {
                                    recursionList.Add(true);
                                }
                                else
                                {
                                    recursionList.Add(false);
                                }
                               
                                if (countingmechanism[currentslot] < countingmechanismcap[currentslot] - 1)
                                {
                                    countingmechanism[currentslot]++;
                                }
                                else
                                {
                                    while (countingmechanism[currentslot] == countingmechanismcap[currentslot]- 1 && currentslot != 0)
                                    {
                                        currentslot--;
                                        
                                    }
                                    countingmechanism[currentslot]++;
                                    for (int l = clauseTailsLength - 1; l > currentslot; l--)
                                    {
                                        countingmechanism[l] = 0;
                                    }
                                    currentslot = clauseTailsLength - 1;
                                }
                            }
                        }
                        else
                        {
                            recursionList.AddRange(headRecursion(index.getMatchingTrees(clauseTails[i][0])));
                        }
                    }
                }
            }
            return recursionList;
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
            string[] lines = System.IO.File.ReadAllLines(@"testfile4.txt");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PrologEmbedding
{
	class Program
    {
		// Index containing all term trees
		static TermTreeIndex index = new TermTreeIndex ();
		// Dictionary containing all clause tails, heads are in index
		static Dictionary<int, TermTree[]> clauseTails = new Dictionary<int, TermTree[]> ();
		static String[] knowledgeBase;

        static void Main(string[] args)
        {
			knowledgeBase = FileReader();
			int linenumber = 0;
			foreach (string line in knowledgeBase) {

				if (!line.Contains (":-")) {

					if (ClauseValidator.isValidCompound (line) || ClauseValidator.isValidTerm (line)) {
						TermTree test = new TermTree (line);

						index.addTermTree (test, linenumber);
						linenumber++;
					} 
						
				} else {                        
					var ClauseCheck = ClauseValidator.isValidAndDecomposesClause (line);
					if (ClauseCheck.Item1) {

						index.addTermTree (new TermTree (ClauseCheck.Item2), linenumber);
						storeClauseTail (ClauseCheck.Item3, linenumber);
						// new clause rail function: addToClauseTree(Newtree, ClauseCheck.Item3, linenumber, 0);
						linenumber++;
					}
				}
			}

			// Outputing index
			Console.Write (index.toString ());

			foreach (int i in index.getMatchingTrees(new TermTree("human(X,b)"))) {
				Console.WriteLine("--" + i);
			}
                        
			// Outputing index
			Console.Write (index.toString ());

			// Outputing clause tails
			foreach (KeyValuePair<int, TermTree[]> treeArray in clauseTails) {
				for (int i = 0; i < treeArray.Value.Length; i++) {
					Console.WriteLine (treeArray.Value [i].toString () + ":" + treeArray.Key);
				}
			}


            List<string> resultshere = new List<string>();
            string inputline;
            string neverUsed;
            while (true)
            {
                inputline = Console.ReadLine();
                Tuple<TermTree, Dictionary<int, Dictionary<String, String>>> filterResult = index.getMatchingTrees2(new TermTree(inputline));
                Dictionary<string, string> filterDict = new Dictionary<string, string>();
                foreach (int k in filterResult.Item2.Keys)
                {
                    foreach (String k2 in filterResult.Item2[k].Keys)
                    {
                        if (!filterDict.TryGetValue(k2, out neverUsed))
                        {
                            filterDict.Add(k2, k2);
                        }
                        
                    }
                }
                filterDict.Add("True", "True");
                filterDict.Add("False", "False");
                resultshere = inference(inputline, 0);
                foreach (string printready in resultshere)
                {
                    //to remove unnecesary lines
                    if (printready != "True#True" && !printready.Contains('(') )
                    {
                        string[] splitResult = printready.Split('#');
                        if (splitResult.Length > 1)
                        {
                            if (filterDict.ContainsKey(splitResult[0]))
                            {
                                Console.WriteLine(splitResult[0] + " = " + splitResult[1] + ".");
                            }
                        }
                        else
                        {
                            Console.WriteLine(splitResult[0]);
                        }
                    }
                }
                Console.WriteLine("");
            }
        }
        

        static private List<string> inference(string inputline, int level)
        {
            // Todo: clauses cannot be queried with one var and one fact

            Tuple<TermTree, Dictionary<int, Dictionary<String, String>>> result = index.getMatchingTrees2(new TermTree(inputline));
                
            List<int> lineNumbers = new List<int>();
            //List<int> lineNumbersVar = new List<int>();
            List<bool> inKnowledgeBase = new List<bool>();
            //List<string> inKnowledgeBaseVar = new List<string>();
            List<string> resultString = new List<string>();
            
            // Distinguishes between with and without vars

            foreach (var item in result.Item2.Keys)
            {
                if (result.Item2[item].Count != 0)
                {
                    // The part for query'ing with variables
					resultString= headRecursionVar(result, 0);
                }
                else
                {
                    // The part for query'ing without variables
                    lineNumbers.Add(item);
                }
            }


            // For handling queries withour variables:
            
            int linenumbercount = lineNumbers.Count;
            if(linenumbercount != 0)
            {
                if (linenumbercount > 0)
                {
                    int[] matchresult = new int[linenumbercount];
                    int count = 0;
                    foreach (int item in lineNumbers)
                    {
                        matchresult[count] = item;
                        count++;
                    }
                    inKnowledgeBase = headRecursion(matchresult, level);
                    foreach (bool item in inKnowledgeBase)
                    {
                        resultString.Add(item.ToString());
                    }
                }
            }
            return resultString;
        }

        static List<string> Testmethod(int lineNumber, Dictionary<string, string> variableBindings)
        {
            // This method seperates the tail of a clause into queries
            List<string> resultlist = new List<string>();
            string temporary;

            foreach (var item in clauseTails[lineNumber])
            {
                temporary = "";
                //access the seperate tail terms
                temporary += item.term + "(";

                foreach (var item2 in item.branches)
                {
                    //acces their arguments
                    string intermediateresult = item2.term;
                    if (variableBindings.ContainsKey(intermediateresult))
                    {
                        intermediateresult = variableBindings[intermediateresult];
                    }
                    temporary += intermediateresult + ",";
                }
                temporary = temporary.TrimEnd(',');
                temporary += ")";
                resultlist.Add(temporary);
            }
            return resultlist;
        }

        static private List<string> headRecursionVar(Tuple<TermTree, Dictionary<int, Dictionary<String, String>>> result, int level)
        {
			level++;
            List<string> resultstring = new List<string>();

            foreach (int k in result.Item2.Keys)
            {
                if (!clauseTails.ContainsKey(k))
                {
                    foreach (String k2 in result.Item2[k].Keys) resultstring.Add(k2 + "#" + result.Item2[k][k2]);
                    if (result.Item2[k].Keys.Count < result.Item1.arity) resultstring.Add("True#True");
                }
                else
                {
                    int clauseTailsLength = clauseTails[k].Length;
                    Dictionary<string, string> variableBindings = new Dictionary<string, string>();
                    foreach (String k2 in result.Item2[k].Keys)
                    {
                        variableBindings.Add(result.Item2[k][k2], k2);
                    }
                    
                    List<string>[] resultstring2 = new List<string>[clauseTailsLength];
                    int count = 0;

                    resultstring = Testmethod(k, variableBindings);

                    int[] arityArray = new int[clauseTailsLength];

                    for (int tailobject = 0; tailobject < clauseTailsLength; tailobject++)
                    {
                        arityArray[tailobject] = clauseTails[k][tailobject].arity;
                    }

                    string tempstring;

                    foreach (string string2 in resultstring)
                    {
                        tempstring = Regex.Replace(string2, @"\s+", "");
                        resultstring2[count] = inference(tempstring, level);
                        count++;
                    }

                    if (clauseTailsLength == 1)
                    {
                        resultstring = resultstring2[0];
                    }
                    else
                    {
                        Dictionary<string, string> comparisonDict = new Dictionary<string, string>();

                        //split

                        int[] countingmechanism = new int[clauseTailsLength];
                        int[] countingmechanismcap = new int[clauseTailsLength];
                        int numberofrounds = 1;
                        for (int j = 0; j < clauseTailsLength; j++)
                        {
                            countingmechanismcap[j] = resultstring2[j].Count/arityArray[j];
                            numberofrounds *= (resultstring2[j].Count/arityArray[j]);
                        }

                        //merge

                        int currentslot = clauseTailsLength - 1;
                        for (int k2 = 0; k2 < numberofrounds; k2++)
                        {
                            bool finalcheck = true;
                            Char delimiter = '#';
                            for (int z = 0; z < clauseTailsLength; z++)
                            {
                                for (int zsub = 0; zsub < arityArray[z]; zsub++)
                                {
                                    string[] words = resultstring2[z].ElementAt(countingmechanism[z]*arityArray[z] + zsub).Split(delimiter);
                                    string test;
                                    if (comparisonDict.TryGetValue(words[0], out test))
                                    {
                                        if (test != words[1]) finalcheck = false;
                                    } 
                                    else
                                    {
                                       
                                        if (words.Length == 1)
                                        {
                                            Array.Resize(ref words, words.Length + 1);
                                            words[1] = words[0];
                                        }
                                        
                                        comparisonDict.Add(words[0], words[1]);
                                    }
                                }
                            }

                            if (finalcheck)
                            {
                                foreach (KeyValuePair<string, string> pair in comparisonDict)
                                {
                                    resultstring.Add(pair.Key + "#" + pair.Value);
									if (level == 1) Console.WriteLine (pair.Key + "#" + pair.Value);
                                }
                            }

                            comparisonDict.Clear();

							countingMechanism (ref currentslot, ref countingmechanism, ref countingmechanismcap, clauseTailsLength);
							/*
                            if (countingmechanism[currentslot] < countingmechanismcap[currentslot] - 1)
                            {
                                countingmechanism[currentslot]++;
                            }
                            else
                            {
                                while (countingmechanism[currentslot] == countingmechanismcap[currentslot] - 1 && currentslot != 0)
                                {
                                    currentslot--;
                                }
                                countingmechanism[currentslot]++;
                                for (int l = clauseTailsLength - 1; l > currentslot; l--) countingmechanism[l] = 0;
                                currentslot = clauseTailsLength - 1;
                            }
                            */
                        }

                    }
                }                


            }
            return resultstring;
        }

		static private void countingMechanism(ref int current, ref int[] counterArray1, ref int[] counterArray2, int size) {
			if (counterArray1[current] < counterArray2[current] - 1)
			{
				counterArray1[current]++;
			}
			else
			{
				while (counterArray1[current] == counterArray2[current] - 1 && current != 0)
				{
					current--;
				}
				counterArray1[current]++;
				for (int l = size - 1; l > size; l--) counterArray1[l] = 0;
				current = size - 1;
			}
		}

        static private List<bool> headRecursion(int[] matchresult, int level)
        {
            List<bool> recursionList = new List<bool>();

            if (matchresult.Length == 0)
            {
                recursionList.Add(false);
            }
            else
            {
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
                                multipleClauses[j] = headRecursion(index.getMatchingTrees(clauseTails[i][j]), level);
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
                                
                                recursionList.Add(finalcheck);
                                
								countingMechanism (ref currentslot, ref countingmechanism, ref countingmechanismcap, clauseTailsLength);

								/*
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
                                    for (int l = clauseTailsLength - 1; l > currentslot; l--) countingmechanism[l] = 0;
                                    currentslot = clauseTailsLength - 1;
                                }
                                */
                            }
                        }
                        else
                        {
                            recursionList.AddRange(headRecursion(index.getMatchingTrees(clauseTails[i][0]),level));
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
            string[] lines = System.IO.File.ReadAllLines(@"familytree.txt");
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
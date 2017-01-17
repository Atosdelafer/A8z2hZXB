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
			//Console.WriteLine (ClauseValidator.isValidMath ("X is B"));

			//return;
			//storeClauseTail ("foo(bar),doz(coo)", 0);

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
            while (true)
            {
                inputline = Console.ReadLine();
                resultshere = inference(inputline);
                foreach (string printready in resultshere)
                {
                    string[] splitResult = printready.Split('#');
                    if (splitResult.Length > 1)
                    {
                        Console.WriteLine(splitResult[0] + " = " + splitResult[1] + ".");
                    }
                    else
                    {
                        Console.WriteLine(splitResult[0]);
                    }
                }
            }
        }
        

        static private List<string> inference(string inputline)
        {
                Tuple<TermTree, Dictionary<int, Dictionary<String, String>>> result = index.getMatchingTrees2(new TermTree(inputline));
                
                List<int> lineNumbers = new List<int>();
                List<int> lineNumbersVar = new List<int>();
                List<bool> inKnowledgeBase = new List<bool>();
                List<string> inKnowledgeBaseVar = new List<string>();

            List<string> resultString = new List<string>();


            //need to make sure also facts are checked
            foreach (var item in result.Item2.Keys)
            {
                if (result.Item2[item].Count != 0)
                {
                    inKnowledgeBaseVar = headRecursionVar(result);
                    resultString = inKnowledgeBaseVar;
                }
                else
                {
                    lineNumbers.Add(item);
                }
            }


            //Part for Var


            /*if (result.Item2[0].Count != 0)
            {
                inKnowledgeBaseVar = headRecursionVar(result);
                resultString = inKnowledgeBaseVar;
            }*/
            int linenumbercount = lineNumbers.Count;
            if(linenumbercount != 0)
            {
                //Part for No Var.
                
                if (linenumbercount > 0)
                {
                    int[] matchresult = new int[linenumbercount];
                    int count = 0;
                    foreach (int item in lineNumbers)
                    {
                        matchresult[count] = item;
                        count++;
                    }
                    inKnowledgeBase = headRecursion(matchresult);
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
            //presents seperate tails
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
                    //Console.WriteLine(item2.term);
                }
                temporary = temporary.TrimEnd(',');
                temporary += ")";
                resultlist.Add(temporary);
            }
            return resultlist;
        }

        static private List<string> headRecursionVar(Tuple<TermTree, Dictionary<int, Dictionary<String, String>>> result)
        {
            List<string> resultstring = new List<string>();
            //List<int> lineNumbersVar = new List<int>();


            foreach (int k in result.Item2.Keys)
            {
                //lineNumbersVar.Add(k);
                if (!clauseTails.ContainsKey(k))
                {
                    //Console.Write(k + " - ");
                    foreach (String k2 in result.Item2[k].Keys)
                    {
                        resultstring.Add(k2 + "#" + result.Item2[k][k2]);
                    }
                    //Console.WriteLine(" ");
                }
                else
                {
                    int clauseTailsLength = clauseTails[k].Length;
                    Dictionary<string, string> variableBindings = new Dictionary<string, string>();
                    foreach (String k2 in result.Item2[k].Keys)
                    {
                        variableBindings.Add(result.Item2[k][k2], k2);
                    }

                    //if (clauseTailsLength > 1)
                    //{
                    List<string>[] resultstring2 = new List<string>[clauseTailsLength];
                    int count = 0;

                    //foreach (var itemu1 in lineNumbersVar)
                    //{
                    resultstring = Testmethod(k, variableBindings);

                    //}
                    int[] arityArray = new int[clauseTailsLength];
                    for (int tailobject = 0; tailobject < clauseTailsLength; tailobject++)
                    {
                        arityArray[tailobject] = clauseTails[k][tailobject].arity;
                    }
                    string tempstring;
                    foreach (string string2 in resultstring)
                    {
                        tempstring = Regex.Replace(string2, @"\s+", "");
                        resultstring2[count] = inference(tempstring);
                        count++;
                    }

                    if (clauseTailsLength == 1)
                    {
                        //check if this works properly
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
                            //just added this, presumably good
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
                                    //removes countingmechanism[zsub]* arity + zsub
                                    string test;
                                    if (comparisonDict.TryGetValue(words[0], out test)) // Returns true.
                                    {
                                        if (test != words[1])
                                        {
                                            finalcheck = false;
                                            //comparisonDict.Clear();
                                            //clearmistake
                                        }
                                    }
                                    else
                                    {
                                        comparisonDict.Add(words[0], words[1]);
                                    }
                                }
                            }

                            if (finalcheck)
                            {
                                foreach (KeyValuePair<string, string> pair in comparisonDict)
                                {
                                    resultstring.Add(pair.Key + "#" + pair.Value);
                                }
                            }

                            comparisonDict.Clear();

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
                                for (int l = clauseTailsLength - 1; l > currentslot; l--)
                                {
                                    countingmechanism[l] = 0;
                                }
                                currentslot = clauseTailsLength - 1;
                            }
                        }
                    }
                }                
            }
            return resultstring;
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
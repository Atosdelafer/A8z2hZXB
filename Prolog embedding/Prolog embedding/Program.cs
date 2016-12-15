﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Prolog_embedding
{
    class Compound {
		public string name;
		public int arity;
		public Dictionary<string,Compound>[] children; 
		public ArrayList indices = new ArrayList();

		public Compound(string name, int arity) {
			this.name = name;
			this.arity = arity;

			if (arity == -1)
				arity = 1;
			this.children = new Dictionary<string, Compound>[arity];

			for (int i = 0; i < arity; i++) {
				children [i] = new Dictionary<string, Compound> ();
			}
		}
	}

	class Program
    {
		static Compound index = new Compound ("root", -1);
		static Dictionary<int, Compound[]> clauseTails = new Dictionary<int, Compound[]> ();

        static void Main(string[] args)
        {
			//storeClauseTail ("foo(bar),doz(coo)", 0);
			
		    bool repeat = true;
            while (repeat)
            {
                string teststring = Console.ReadLine();
                Console.WriteLine(teststring);
                String[] filecontent = Parser();
				string liststring = listToCompound(teststring);
                Console.WriteLine(liststring);
                int linenumber = 0;
                foreach (string line in filecontent)
                {
                    var ClauseCheck = isValidAndDecomposesClause(line);
                    if (isValidCompound(line))
                    {                        
                        Console.WriteLine("Compound");
                        addToNode(Program.index, line, linenumber, 0);
                        linenumber++;
                    }
                    else if (ClauseCheck.Item1)
                    {                        
                        Console.WriteLine("Clause");
                        addToNode(Program.index, ClauseCheck.Item2, linenumber, 0);
                        // new clause rail function: addToClauseTree(Newtree, ClauseCheck.Item3, linenumber, 0);
                        linenumber++;
                    }
                }
                
                printTree(Program.index, "");
                
                /*
					Compound test = new Compound ("root", -1);

					addToNode (test, "foo(bar(baz), X)", 0, 0);

					int l = Program.index.indices.ToArray().Length;
					int[] indices2 = new int[l];
					for (int i = 0; i < l; i++)
						indices2 [i] = (int)Program.index.indices [i];

					int[] indices3 = {1, 2, 3, 4, 5};

					int[] indices = findIndices (Program.index, test, 0, indices3);
					Console.WriteLine (string.Join (",", indices));
                */
            }

        }

        static private String[] Parser()
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

		static private void printTree(Compound node, string tabs) {

			for (int parameter = 0; parameter < node.children.Length; parameter++) {
				Console.WriteLine (tabs + parameter);
				// Print all possible parameter values on a line

				foreach (KeyValuePair<string, Compound> child in node.children[parameter]) {

					Console.Write (tabs + "\t" + child.Value.name);
					Console.WriteLine(": " + string.Join(",", child.Value.indices.ToArray()));

					printTree (child.Value, tabs + "\t");
				}
	
			}
		}

		static private void addToNode(Compound node, string compound, int index, int parameter) {

			Tuple<string, int, ArrayList> pattern = termToPattern (compound);

			string name = pattern.Item1;
			int arity = pattern.Item2;
			ArrayList children = pattern.Item3;

			string key = name + '/' + arity;

			if (!node.children[parameter].ContainsKey(key))
				node.children[parameter].Add(key, new Compound(key, arity));
			node.children [parameter] [key].indices.Add (index);


			int p = 0;
			foreach (string child in children) {
				addToNode (node.children[parameter][key], child, index, p++);
			}
		}

		static private Tuple<string, int, ArrayList> termToPattern(string term) {
			int leftBrace = term.IndexOf ("(");
			int rightBrace = term.LastIndexOf (")");
			int arity;
			string name;
			ArrayList children = new ArrayList ();

			if (leftBrace == -1) {
				name = term;
				arity = 0;
			} else {

				name = term.Substring (0, leftBrace);

				int level = 0;
				int left = leftBrace;
				arity = 1;
				for (int i = 0; i < term.Length; i++) {
					if (term [i] == '(') {
						level++;
					} else if (term [i] == ')')
						level--;
					else if (term [i] == ',' && level == 1) {
						arity++;
						children.Add (term.Substring (left + 1, i - left - 1));
						left = i;
					}
				}
				children.Add (term.Substring (left + 1, rightBrace - left - 1));
			}

			return new Tuple<string, int, ArrayList>(name, arity, children);
		}

		static private int[] findIndices(Compound index, Compound node, int parameter, int[] indices) {
			int[] indices2;
			int l;
			for (int p = 0; p < node.children.Length; p++) {
				// Print all possible parameter values on a line

				foreach (KeyValuePair<string, Compound> child in node.children[p]) {

					if (Regex.Match (child.Value.name, "^[A-Z_]").Success) {
						Console.WriteLine (child.Value.indices);

						l = index.indices.ToArray ().Length;
						indices2 = new int[l];
						for (int i = 0; i < l; i++)
							indices2 [i] = (int)index.indices [i];
						return indices2;

					}

					if (!index.children [p].ContainsKey (child.Value.name)) {
						return new int[] {};
					}

					l = index.children[p][child.Value.name].indices.ToArray ().Length;
					indices2 = new int[l];
					for (int i = 0; i < l; i++)
						indices2 [i] = (int)index.children[p][child.Value.name].indices [i];

					indices = indices.Intersect(indices2).ToArray();

					if (!index.children[p].ContainsKey(child.Value.name)) return indices;

					string key = child.Value.name;

					int[] indicesReturned = findIndices(index.children[p][key], child.Value, p, indices);
					indices = indices.Intersect(indicesReturned).ToArray();

				}

			}
			return indices;
		}

        static private string listToCompound(string term)
        {
            int numberofcommas = 0;
            string compoundterm = term.Replace("[", ".(");
            compoundterm = compoundterm.Replace("]", "emptylist)");  

            for (int i = 0; i < compoundterm.Length; i++)
            {
                if (compoundterm[i] == ',')
                {
                    numberofcommas++;
                }
                else if (compoundterm[i] == ')' && numberofcommas != 0)
                {
                    for (int j = numberofcommas; j >0; j--)
                    {
                        compoundterm = compoundterm.Insert(i, ")");
                        numberofcommas--;
                    }                    
                }
            }

            compoundterm = compoundterm.Replace(",", ",.(");
            compoundterm = compoundterm.Replace("emptylist", ",[]");            
            return compoundterm;
        }

        static private bool isValidCompound(string term)
        {

            bool nocomma = true;
            int outerleftbrace = 0;
            int numberofbraces = 0;
            int outerrightbrace = 0;
                        

            for (int i = 0; i < term.Length; i++)
            {
                if (term[i] == '(')
                {
                    numberofbraces++;
                    if (outerleftbrace == 0)
                    {
                        outerleftbrace = i;
                    }                
                }
                else if (term[i] == ')')
                {
                    numberofbraces--;
                    if (numberofbraces == 0)
                    {
                        outerrightbrace = i;
                    }
                        
                }
                else if (term[i] == ',' && numberofbraces == 0)
                {
                    nocomma = false;
                    if (!(isValidCompound(term.Substring(0, i)) && isValidCompound(term.Substring(i+1))))
                    {
                        return false;
                    }                   
                }
            }

            if (numberofbraces != 0)
            {
                return false;
            }

            if (nocomma == true && outerleftbrace != 0)
            {
                if (!(isValidCompound(term.Substring(outerleftbrace + 1, (outerrightbrace - 1)-(outerleftbrace)))))
                {
                    return false;
                }
            }

            if (outerleftbrace != 0)
            {
                if (!(isValidTerm(term.Substring(0, outerleftbrace))))
                {
                    return false;
                }
            }
            else if (nocomma == true)
            {
                if (!(isValidTerm(term)))
                {
                    return false;
                }
            }    
            return true;            
        }

        static private bool isValidTerm(string term)
        {
            Match match = Regex.Match(term, "^(?![A-Z_ ])[A-Za-z0-9_]*$");
                        
            if (match.Success)
            {                
                string key = match.Groups[0].Value;
                return true;
                //Console.WriteLine(key);                
            }
            return false;
        }

        static private Tuple<Boolean, String, String> isValidAndDecomposesClause(string term)
        {
            bool previousCharIsColumn = false;
            bool isValidClause = false;
            string head = "{}";
            string tail = "{}";
            
            for (int i = 0; i < term.Length; i++)
            {
                if (term[i] == ':')
                {
                    previousCharIsColumn = true;                
                }
                else if (term[i] == '-')
                {
                    if (previousCharIsColumn)
                    {
                        head = term.Substring(0, i - 1);
                        tail = term.Substring(i+1);
                    }
                }
            }
            if (!(isValidCompound(head) && isValidCompound(tail)))
            {
                isValidClause = false;
            }        
            isValidClause = true;

            var tuple = new Tuple<Boolean, String, String>(isValidClause, head,tail);
            return tuple;
        }

        private static Tuple<int, int> Add_Multiply(int a, int b)
        {
            var tuple = new Tuple<int, int>(a + b, a * b);
            return tuple;
        }

		static private void storeClauseTail(string tail, int linenumber)
		{
			int level = 0;
			string buffer = "";
			List<Compound> children = new List<Compound> ();

			foreach (char c in tail) {
				if (c == '(')
					level++;
				else if (c == ')')
					level--;
				else if (c == ',' && level == 0) {
					Compound child = new Compound ("root", -1);
					addToNode (child, buffer, 0, 0);
					children.Add (child);
					buffer = "";
				} 

				if (!(c == ',' && level == 0)) {
					buffer += c;
				}
			}

			if (buffer != "") {
				Compound child = new Compound ("root", -1);
				addToNode (child, buffer, 0, 0);
				children.Add (child);
			}

			Program.clauseTails.Add (0, children.ToArray ());
		}

        static private bool isValidVariable(string term)
        {
            Match match = Regex.Match(term, "^[A-Z_][A-Za-z0-9_]*$");
                        
            if (match.Success)
            {
                string key = match.Groups[0].Value;
                return true;
                //Console.WriteLine(key);
            }
            return false;
        }
    }
}
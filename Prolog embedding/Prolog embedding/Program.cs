using System;
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

        static void Main(string[] args)
        {
		bool repeat = true;
        bool check = true;

            while (repeat)
            {
                string teststring = Console.ReadLine();

				if (teststring == null) {
					teststring = "foo(bar, boz, a(b))";
					repeat = false;
				}
                //bool check = isValidVariable(teststring);
                //bool check = isValidCompound(teststring);
                Console.WriteLine(teststring);
                String[] filecontent = Parser();
				string liststring = listToCompound(teststring);
                Console.WriteLine(liststring);
                int linenumber = 0;
                foreach (string line in filecontent)
                {
                    check = isValidCompound(line);
                    if (check)
                    {
                        Console.WriteLine("Yes");
                        addToNode(Program.index, line, linenumber, 0);
                        linenumber++;
                    }
                }
                addToNode(Program.index, "loves(testperson1,testperson2)", 30, 0);
                printTree(Program.index, "");

                /*
                check = isValidCompound(liststring);
                if (check)
                {
                    Console.WriteLine("Yes");

					addToNode (Program.index, "foo(bar)", 0, 0);
					addToNode (Program.index, "foo(baz)", 1, 0);
					addToNode (Program.index, "foo(bar(baz), boz)", 2, 0);
					addToNode (Program.index, "foo(bar(baz), baz)", 3, 0);
					addToNode (Program.index, "foo(bar(baz), baz(foo))", 4, 0);
					printTree (Program.index, "loves(testperson1,testperson2)",30,0);

					Compound test = new Compound ("root", -1);

					addToNode (test, "foo(bar(baz), X)", 0, 0);

					int l = Program.index.indices.ToArray().Length;
					int[] indices2 = new int[l];
					for (int i = 0; i < l; i++)
						indices2 [i] = (int)Program.index.indices [i];

					int[] indices3 = {1, 2, 3, 4, 5};

					int[] indices = findIndices (Program.index, test, 0, indices3);
					Console.WriteLine (string.Join (",", indices));

                }
                else
                {
                    Console.WriteLine("No");
                }
                */
            }
        }

        static private String[] Parser()
        {
            string[] lines = System.IO.File.ReadAllLines(@"testfile.txt");

            // Display the file contents by using a foreach loop.
            //System.Console.WriteLine("Contents of WriteLines2.txt = ");


            /* foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }*/

            return lines;

            /*// Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();*/
            
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
						left = i + 1;
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

        //small error still.
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

            if (!(isValidTerm(term.Substring(0, outerleftbrace))))
            {
                return false;
            }

            return true;            
        }

        static private bool isValidTerm(string term)
        {
            Match match = Regex.Match(term, "^(?![A-Z_ ])[A-Za-z0-9_]*$");
                        
            if (match.Success)
            {                
                string key = match.Groups[0].Value;
                //Console.WriteLine(key);                
            }
            return true;
        }

        static private bool isValidVariable(string term)
        {
            Match match = Regex.Match(term, "^[A-Z_][A-Za-z0-9_]*$");
                        
            if (match.Success)
            {
                string key = match.Groups[0].Value;
                //Console.WriteLine(key);
            }
            return true;
        }
    }
}

// todo: lists in this manner + recursion coumpoundcheck

//[a] = dot(a,emptylist)
//[[a,b],c] = dot(dot(a,dot(b,emptylist)),dot(c,emptylist))
// loves(man(eric),woman(julia))
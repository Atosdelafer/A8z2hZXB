using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;

namespace PrologEmbedding
{
	class TermTree {
		public string term;
		public int arity;
		public string key;
		public TermTree[] branches;

		public TermTree(string term, int arity) {
			this.initialize (term, arity);
		}

		public TermTree(string compound) {

			Tuple<string, int, ArrayList> pattern = termToPattern (compound);
			ArrayList children = pattern.Item3;

			this.initialize (pattern.Item1, children.Count);

			for (int p = 0; p < children.Count; p++) {
				this.branches[p] = compoundToTermTree(children[p].ToString(), p);
			}
		}

		private void initialize(string term, int arity) {
			this.term = term;
			this.arity = arity;
			this.key = term + "/" + arity;
			this.branches = new TermTree[arity];
		}

		static public TermTree compoundToTermTree(string compound, int parameter = 0) {

			Tuple<string, int, ArrayList> pattern = termToPattern (compound);
			string name = pattern.Item1;
			int arity = pattern.Item2;
			ArrayList children = pattern.Item3;
			TermTree tree = new TermTree (name, children.Count);

			for (int p = 0; p < children.Count; p++) {
				tree.branches[p] = compoundToTermTree(children[p].ToString(), p);
			}

			return tree;
		}

		public string toString() {
			return toString (this);
		}

		static private string toString(TermTree tree, string tabs = "") {
			string output = tabs + tree.term + "\n";

			for (int b = 0; b < tree.branches.Length; b++) {
				output += tabs + b + "\n";
				output += toString(tree.branches[b], tabs + "\t");
			}

			return output;
		}
	
		static private Tuple<string, int, ArrayList> termToPattern(string term) {
			int leftBrace = term.IndexOf("(");
			int rightBrace = term.LastIndexOf(")");
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
	}

	class TermTreeIndex {
		public string term;
		public int arity;
		public string key;
		public Dictionary<string,TermTreeIndex>[] branches;
		public List<int> indices = null;

		public TermTreeIndex() : this("root", 1) {}

		public TermTreeIndex(string term, int arity) {
			this.term = term;
			this.arity = arity;
			this.key = term + "/" + arity;
			this.branches = new Dictionary<string, TermTreeIndex>[arity];

			for (int i = 0; i < arity; i++) {
				branches [i] = new Dictionary<string, TermTreeIndex> ();
			}
			indices = new List<int>();
		}

		public void addTermTree(TermTree tree, int index) {
			addTermTree (this, tree, index, 0);
		}

		static private void addTermTree(TermTreeIndex termTreeIndex, TermTree tree, int index, int parameter) {

			if (parameter == 0) 
				termTreeIndex.indices.Add (index);

			if (!(termTreeIndex.branches [parameter].ContainsKey (tree.key)))
				termTreeIndex.branches [parameter].Add (tree.key, new TermTreeIndex (tree.term, tree.arity));

			for (int b = 0; b < tree.arity; b++) {
				addTermTree (termTreeIndex.branches [parameter] [tree.key], tree.branches [b], index, b);
			}

			if (tree.arity == 0)
				termTreeIndex.branches [parameter][tree.key].indices.Add(index);
		}

		public int[] getMatchingTrees(TermTree tree) {
			return getMatchingTrees (this, tree, 0);
		}

		static private int[] getMatchingTrees(TermTreeIndex termTreeIndex, TermTree tree, int parameter) {

			int[] indices = termTreeIndex.indices.ToArray ();

			if (Regex.Match (tree.term, "^[A-Z_]").Success) {
				return termTreeIndex.indices.ToArray ();
			} else if ((termTreeIndex.branches [parameter].ContainsKey (tree.key))) {
				for (int b = 0; b < tree.arity; b++) {
					indices = indices.Intersect (
						getMatchingTrees (termTreeIndex.branches [parameter] [tree.key], tree.branches [b], b)
						).ToArray ();
				}

				if (tree.arity == 0) 
					return termTreeIndex.branches[parameter][tree.key].indices.ToArray();

				return indices;
			} else {
				return new int[0];
			}
		}

		public Tuple<TermTree, Dictionary<int, Dictionary<String, String>>> getMatchingTrees2(TermTree tree) {
			Dictionary<int, Dictionary<String, String>> result = new Dictionary<int, Dictionary<String, String>> ();

			foreach (int i in this.indices)	result [i] = new Dictionary<String, String> ();
			int[] indices = getMatchingTrees2 (this, tree, 0, result);

			Dictionary<int, int> indicesDict = new Dictionary<int, int> ();
			foreach (int i in indices) indicesDict [i] = i;

			List<int> removeIds = new List<int> ();

			foreach (int index in result.Keys) {
				if (!indicesDict.ContainsKey (index))
					removeIds.Add (index);
			}

			foreach(int index in removeIds) result.Remove (index);


			return new Tuple<TermTree, Dictionary<int, Dictionary<string, string>>>(tree, result);
		}

		static private int[] getMatchingTrees2(TermTreeIndex termTreeIndex, TermTree tree, int parameter, 
		                                       Dictionary<int, Dictionary<String, String>> indicesAndVariables) {

			int[] indices = termTreeIndex.indices.ToArray ();

			if(Regex.Match(tree.term, "^[A-Z_]").Success) {

				// Storing variable bindings
				foreach (TermTreeIndex branch in termTreeIndex.branches [parameter].Values) {
					foreach (int index in branch.indices) {

						if (indicesAndVariables [index].ContainsKey (tree.term)) {
							if (indicesAndVariables [index] [tree.term] != branch.term) {
								List<int> indicesList = termTreeIndex.indices;
								indicesList.Remove (index);
								indices = indicesList.ToArray ();
							}
						} else {
							indicesAndVariables [index] [tree.term] = branch.term;
						}
					}
				}

				return indices;
			} else if ((termTreeIndex.branches [parameter].ContainsKey (tree.key))) {
				for (int b = 0; b < tree.arity; b++) {
					indices = indices.Intersect (
						getMatchingTrees2 (termTreeIndex.branches [parameter] [tree.key], tree.branches [b], b, indicesAndVariables)
						).ToArray ();
				}

				if (tree.arity == 0) 
					return termTreeIndex.branches[parameter][tree.key].indices.ToArray();

				return indices;
			} else {
				return new int[0];
			}
		}

		public string toString() {
			return toString (this);
		}

		static private string toString(TermTreeIndex tree, string tabs = "") {
			string output = tabs + tree.term + "/" + tree.arity;
			output += ": " + string.Join (",", tree.indices.ToArray ()) + "\n";

			for (int b = 0; b < tree.branches.Length; b++) {
				output+= tabs + b + "\n";
				foreach (KeyValuePair<string, TermTreeIndex> child in tree.branches[b]) {
					output += toString(child.Value, tabs + "\t");
				}
			}
			return output;
		}
	}
}


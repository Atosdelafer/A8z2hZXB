using System;
using System.Text.RegularExpressions;

namespace PrologEmbedding
{
	public static class ClauseValidator
	{
		static string listToCompound(string term)
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

		public static bool isValidCompound(string term)
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

		static bool isValidTerm(string term)
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

		public static Tuple<Boolean, String, String> isValidAndDecomposesClause(string term)
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

		static Tuple<int, int> Add_Multiply(int a, int b)
		{
			var tuple = new Tuple<int, int>(a + b, a * b);
			return tuple;
		}

		static bool isValidVariable(string term)
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


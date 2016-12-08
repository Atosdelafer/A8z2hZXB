using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Prolog_embedding
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                string teststring = Console.ReadLine();
                //bool check = isValidVariable(teststring);
                //bool check = isValidCompound(teststring);
                string liststring = listToCompound(teststring);
                Console.WriteLine(liststring);
                bool check = isValidCompound(liststring);
                if (check)
                {
                    Console.WriteLine("Yes");
                }
                else
                {
                    Console.WriteLine("No");
                }
            }
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
                        compoundterm.Insert(j, ")");
                    }                    
                }
            }

            compoundterm = compoundterm.Replace(",", ",.(");
            compoundterm = compoundterm.Replace("emptylist", ",[])");            
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
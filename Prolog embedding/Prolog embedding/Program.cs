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
                bool check = isValidCompound(teststring);
            }                        
        }

        static private string listtocompound(string term)
        {
            for (int i = 0; i < term.Length; i++)
            {
                if (term[i] == '[')
                {
                    term[i] == dot()
                }
            }
            string compoundterm;
            return compoundterm;
        }

        static private bool isValidCompound(string term)
        {

            bool correctcompound = true;
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
                    if (!(isValidCompound(term.Substring(0, i - 1)) && isValidCompound(term.Substring(i+1))))
                    {
                        return false;
                    }                   
                }
            }

            correctcompound = isValidTerm(term.Substring(0, outerleftbrace - 1));


            Console.WriteLine(outerleftbrace);
            Console.WriteLine(outerrightbrace);
            return correctcompound;
        }

        static private bool isValidTerm(string term)
        {
            Match match = Regex.Match(term, "^(?![A-Z_ ])[A-Za-z0-9_]*$");
                        
            if (match.Success)
            {                
                string key = match.Groups[0].Value;
                Console.WriteLine(key);                
            }
            return true;
        }

        static private bool isValidVariable(string term)
        {
            Match match = Regex.Match(term, "^[A-Z_][A-Za-z0-9_]*$");
                        
            if (match.Success)
            {
                string key = match.Groups[0].Value;
                Console.WriteLine(key);
            }
            return true;
        }
    }
}

// todo: lists in this manner + recursion coumpoundcheck

//[a] = dot(a,emptylist)
//[[a,b],c] = dot(dot(a,dot(b,emptylist)),dot(c,emptylist))
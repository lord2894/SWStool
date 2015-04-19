using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermRules;

namespace TestDllApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "TextA.txt";
            TermRules.Rules rules = new Rules(inputFile, Dictionary.IT_TERM);
            rules.ApplyRules();
        }
    }
}

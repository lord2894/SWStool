using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RulesNamespace;
using TermProcessingNamespace;

namespace TestDllApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "TextA.txt";
            Rules rules = new Rules(inputFile, DictionaryF.IT_TERM);
            rules.ApplyRules();
        }
    }
}

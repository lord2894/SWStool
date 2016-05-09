using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RulesNamespace;
using TermProcessingNamespace;
using TermsNamespace;

namespace IndexAndGlossaryNamespace
{
    public class IndexItem
    {
        public string term { get; set; }
        public List<string> supportTerms { get; set; }
        public List<int> pages { get; set; }
        public IndexItem(string term_)
        {
            term = term_;
        }
    }
    public class GlossaryItem
    {
        public string Term { get; set; }
        public string Definition { get; set; }
        public GlossaryItem(string term_, string description_)
        {
            Term = term_;
            Definition = description_;
        }
    }
    public class IndexAndGlossary
    {
        
        private Rules rules;
        private Terms mainTermsAr;
        public IndexAndGlossary(string inputfile, DictionaryF dict, int startPage, int endPage, bool ClearSupportLists)
        {
            rules = new Rules(inputfile, dict, startPage, endPage, ClearSupportLists);
            mainTermsAr = rules.ApplyRules();
        }
        public List<IndexItem> GetIndex()
        {
            List<IndexItem> index = new List<IndexItem>();
            foreach (Term term in mainTermsAr.TermsAr)
            {
                if (term.kind != KindOfTerm.DictTerm)
                {
                    List<string> separatedTerm = new List<string>();
                    separatedTerm = GetSeparatedTerm(term.TermWord, term.Pattern);
                    IndexItem curItem = index.Find(r => r.term == separatedTerm[0]);
                    if (curItem == null)
                    {
                        curItem = new IndexItem(separatedTerm[0]);
                        for (int i = 1; i < separatedTerm.Count; i++)
                            curItem.supportTerms.Add(separatedTerm[i]);
                        index.Add(curItem);
                    }
                    else
                    {
                        for (int i = 1; i < separatedTerm.Count; i++)
                        {
                            string supportTerm = curItem.supportTerms.Find(r => r == separatedTerm[i]);
                            if (supportTerm == null || supportTerm == "")
                            {
                                curItem.supportTerms.Add(separatedTerm[i]);
                            }
                        }                        
                    }
                    //IndexItem item = new IndexItem(term.TermWord, term.TermFragment);
                    //index.Add(item);
                }
            }
            index.Sort(delegate(IndexItem item1, IndexItem item2) { return item1.term.CompareTo(item2.term);});
            foreach (IndexItem item in index)
            {
                if (item.supportTerms!=null)
                    item.supportTerms.Sort(delegate(string term1, string term2) { return term1.CompareTo(term2);});
            }
            return index;
        }
        public List<GlossaryItem> GetGlossary()
        {
            List<GlossaryItem> glossary = new List<GlossaryItem>();
            foreach (Term term in mainTermsAr.TermsAr)
            {
                if (term.kind == KindOfTerm.AuthTerm)
                {
                    GlossaryItem item = new GlossaryItem(term.TermWord, term.TermFragment);
                    glossary.Add(item);
                }
            }
            return glossary;
        }        
        public List<string> GetSeparatedTerm(string term, string partsOfSpeech)
        {
            List<string> sepTerm = new List<string>();
            switch(partsOfSpeech)
            {
                case "N":
                    {
                        sepTerm.Add(term.Trim());
                        break;
                    }
                case "NN":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        break;
                    }
                case "AN":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "PN":
                    {
                        goto case "AN";
                    }
                case "AAN":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        string tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "APN":
                    {
                        goto case "AAN";
                    }
                case "PAN":
                    {
                        goto case "AAN";
                    }
                case "PPN":
                    {
                        goto case "AAN";
                    }
                case "ANN":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        string tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "PNN":
                    {
                        goto case "ANN";
                    }                
                case "NAN":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        string tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
                case "NPN":
                    {
                        goto case "NAN";
                    }
                case "NNN":
                    {
                        sepTerm = term.Trim().Split(' ').ToList();
                        string tmpStr = sepTerm[1];
                        sepTerm[2] = tmpStr + " " + sepTerm[2];
                        sepTerm.RemoveAt(1);
                        sepTerm.Reverse(0, sepTerm.Count);
                        break;
                    }
            }

            return sepTerm;
        }

    }
}

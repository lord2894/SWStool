using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TermRules
{
    public class Term
    {
        public string TermWord;
        public List<string> TermFragment;
        public List<TermTree> Pos;
        public bool setToDel;
        public int frequency;
        public Term()
        {
            TermFragment = new List<string>();
            Pos = new List<TermTree>();
        }
        ~Term() { }
    }
    public class NonDictTerm : Term
    {
        public List<List<string>> Components;
        public NonDictTerm()
        {
            Components = new List<List<string>>();
        }
        ~NonDictTerm() { }
    }
    public class CombComponent
    {
	    public string TermWord;
        public List<string> TermFragment;
	    public List<Point> Pos;
	    public int frequency;
	    public CombComponent() 
        {
            TermFragment = new List<string>();
            Pos = new List<Point>();
        }
	    ~CombComponent() {}
    }
    public class CombTerm : Term
    {
        public List<CombComponent> Components;
        public CombTerm() 
        {
            Components = new List<CombComponent>();
        }
        ~CombTerm() { }
    }
    public class SynTerm
    {
        public bool setToDel;
        public int frequency;
        public List<TermTree> Pos;
        public pair<string, string> alternatives;
        public List<string> TermFragment;
        public SynTerm()
        {
            TermFragment = new List<string>();
            alternatives = new pair<string, string>();
            Pos = new List<TermTree>();
        }
    }
    public class Terms
    {
        public List<Term> TermsAr;
        public TermTree rootTermsTree;
        public Terms()
        {
            TermsAr = new List<Term>();
            rootTermsTree = new TermTree();
        }
        ~Terms() { }
    }
    public class SynTerms
    {
            public List<SynTerm> TermsAr;
            public TermTree rootTermsTree;
            public SynTerms()
            {
                TermsAr = new List<SynTerm>();
                rootTermsTree = new TermTree();
            }
            ~SynTerms() { }
	    //TermTree* rootAltTree;
    }
    public class CombTerms
    {
        public List<CombTerm> TermsAr;
        public TermTree rootTermsTree;
        public CombTerms()
            {
                TermsAr = new List<CombTerm>();
                rootTermsTree = new TermTree();
            }
    }
    public class NonDictTerms
    {
        public List<NonDictTerm> TermsAr;
        public TermTree rootTermsTree;
        public NonDictTerms()
            {
                TermsAr = new List<NonDictTerm>();
                rootTermsTree = new TermTree();
            }
    }
    public class ComponentInElement
    {
        public int Element;
        public int Component;
        public int Version;
    }
}

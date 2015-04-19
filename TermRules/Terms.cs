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
        public string TermFragment;
        //public List<string> Patterns;
        public string Pattern;
        public string NPattern;
        public int PatCounter;
        public bool setToDel;
        public int frequency;
        public List<TermTree> Pos;        
        public Term()
        {
            //Patterns = new List<string>(); 
            Pos = new List<TermTree>();
        }
        ~Term() { }
    }
    public class NonDictComponent
    {
        public string Component;
        public List<string> Patterns;
        public int PatCounter;
        NonDictComponent()
        {
            Patterns = new List<string>();
        }
    }
    public class NonDictTerm : Term
    {
        public List<List<NonDictComponent>> Components;
        public List<string> Patterns;
        public int PatCounter;
        public NonDictTerm()
        {
            Patterns = new List<string>();
            Components = new List<List<NonDictComponent>>();
        }
        ~NonDictTerm() { }
    }
    public class CombComponent
    {
	    public string TermWord;
        public string TermFragment;
        public List<string> Patterns;
        public int PatCounter;
        public int frequency;
	    public List<Point> Pos;	    
	    public CombComponent() 
        {
            Patterns = new List<string>(); 
            Pos = new List<Point>();
        }
	    ~CombComponent() {}
    }
    public class CombTerm : Term
    {
        public List<CombComponent> Components;
        public List<string> Patterns;
        public int PatCounter;
        public CombTerm() 
        {
            Patterns = new List<string>();
            Components = new List<CombComponent>();
        }
        ~CombTerm() { }
    }
    public class SynTermAlternative
    {
        public string alternative;
        public string Pattern;
    }
    public class SynTerm
    {
        public bool setToDel;
        public int frequency;
        public List<TermTree> Pos;
        public pair<SynTermAlternative, SynTermAlternative> alternatives;
        public string TermFragment;
        public SynTerm()
        {
            alternatives = new pair<SynTermAlternative, SynTermAlternative>();
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

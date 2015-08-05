using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TermTreeNamespace;

namespace TermsNamespace
{
    public class pair<T, U>
    {
        public pair(T first, U second)
        {
            this.first = first;
            this.second = second;
        }
        public T first { get; set; }
        public U second { get; set; }
    }
    public enum KindOfTerm
            {
                Null,
                AuthTerm,
                DictTerm,
                NonDictTerm,
                CombTerm,
                SynTerm
            }
    public class Term
    {        
        //public List<Point> Positions; //необходимо, особенно для словарных терминов
        public KindOfTerm kind { get; set;}
        public string TermWord { get; set; }
        public string TermFragment { get; set; }
        public string Pattern { get; set; }
        public string NPattern { get; set; }
        public int PatCounter { get; set; }
        public bool setToDel { get; set; }
        public int frequency { get; set; }
        public List<TermTree> Pos { get; set; }
        
        public Term() 
        {
            kind = KindOfTerm.Null;
            TermWord = "";
            TermFragment = "";
            Pattern = "";
            NPattern = "";
            PatCounter = 0;
            setToDel = false;
            frequency = 0;
            Pos = new List<TermTree>(); 
        }        
    }
    public class NonDictBlock
    {
        public string Block { get; set; }
        public List<NonDictComponent> Components { get; set; }
        public NonDictBlock() 
        {
            Block = "";
            Components = new List<NonDictComponent>();
        }  
    }
    public class NonDictComponent
    {
        public string Component { get; set; }
        public string Pattern { get; set; }
        public string NPattern { get; set; }
        public NonDictComponent()
        {
            Component = "";
            Pattern = "";
            NPattern = "";
        }
    }
    public class NonDictTerm : Term
    {
        public List<NonDictBlock> Blocks { get; set; }
        public NonDictTerm()
        {
            kind = KindOfTerm.NonDictTerm;
            Blocks = new List<NonDictBlock>();
        }
        ~NonDictTerm() { }
    }
    public class CombComponent
    {
        public string TermWord { get; set; }
        public string TermFragment { get; set; }
        public string Pattern { get; set; }
        public string NPattern { get; set; }
        public int PatCounter { get; set; }
        public int frequency { get; set; }
        public List<Point> Pos { get; set; }	    
	    public CombComponent() 
        {
            TermWord = "";
            TermFragment = "";
            Pattern = "";
            NPattern = "";
            PatCounter = 0;
            frequency = 0;
            Pos = new List<Point>();
        }
	    ~CombComponent() {}
    }
    public class CombTerm : Term
    {
        public List<CombComponent> Components { get; set; }
        public CombTerm() 
        {
            kind = KindOfTerm.CombTerm;
            Components = new List<CombComponent>();
        }
        ~CombTerm() { }
    }
    public class SynTermAlternative
    {
        public string alternative { get; set; }
        public string PatternPart { get; set; }
        public string NPattern { get; set; }
        public string Pattern { get; set; }
        public SynTermAlternative()
        {
            alternative = "";
            PatternPart = "";
            NPattern = "";
            Pattern = "";
        }
    }
    public class SynTerm
    {
        public KindOfTerm kind { get; set; }
        public bool setToDel { get; set; }
        public int frequency { get; set; }
        public List<TermTree> Pos { get; set; }
        public pair<SynTermAlternative, SynTermAlternative> alternatives { get; set; }
        public string Pattern { get; set; }
        public string TermFragment { get; set; }
        public SynTerm()
        {
            kind = KindOfTerm.SynTerm;
            setToDel = false;
            frequency = 0;
            Pos = new List<TermTree>();
            alternatives = new pair<SynTermAlternative, SynTermAlternative>();
            Pattern = "";
            TermFragment = "";
        }
    }
    public class Terms
    {
        public List<Term> TermsAr { get; set;}
        public TermTree rootTermsTree { get; set;}
        public Terms()
        {
            TermsAr = new List<Term>();
            rootTermsTree = new TermTree();
        }
        ~Terms() { }
    }
    public class SynTerms
    {
            public List<SynTerm> TermsAr { get; set; }
            public TermTree rootTermsTree { get; set; }
            public SynTerms()
            {
                TermsAr = new List<SynTerm>();
                rootTermsTree = new TermTree();
            }
            ~SynTerms() { }
    }
    public class CombTerms
    {
        public List<CombTerm> TermsAr { get; set; }
        public TermTree rootTermsTree { get; set; }
        public CombTerms()
            {
                TermsAr = new List<CombTerm>();
                rootTermsTree = new TermTree();
            }
    }
    public class NonDictTerms
    {
        public List<NonDictTerm> TermsAr { get; set; }
        public TermTree rootTermsTree { get; set; }
        public NonDictTerms()
            {
                TermsAr = new List<NonDictTerm>();
                rootTermsTree = new TermTree();
            }
    }
    public class ComponentInElement
    {
        public int Element { get; set; }
        public int Block { get; set; }
        public int Component { get; set; }
        public ComponentInElement()
        {
            Element = 0;
            Block = 0;
            Component = 0;
        }
    }
}

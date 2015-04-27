using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TermRules
{
    public class Rules
    {
        public Rules(string inputfile, DictionaryF dict) {
            find = new FindFunctions();
            aux = new AuxiliaryFunctions();
            proc = new TermsProcessing(inputfile, dict);
        }
        FindFunctions find;
        AuxiliaryFunctions aux;
        TermsProcessing proc;
        public void Rule1_Mauth_to_M(Terms AuthTermsAr, Terms MainArrayTermsAr)
        {
	        for (int i = 0; i < AuthTermsAr.TermsAr.Count; i++)
	        {
		        int k = find.findINList(MainArrayTermsAr.TermsAr, AuthTermsAr.TermsAr[i].TermWord);
		        if (k == -1)
		        {
			        if (AuthTermsAr.TermsAr[i].Pos.Count>0)
			        {
				        AuthTermsAr.TermsAr[i].setToDel = true;
				        Term newEl = new Term();
                        newEl.frequency = AuthTermsAr.TermsAr[i].frequency;
                        newEl.kind = KindOfTerm.AuthTerm;
                        newEl.NPattern = AuthTermsAr.TermsAr[i].NPattern;
                        newEl.PatCounter = 0;
                        newEl.Pattern = AuthTermsAr.TermsAr[i].Pattern;
                        newEl.setToDel = false;
                        newEl.TermFragment = AuthTermsAr.TermsAr[i].TermFragment;
                        newEl.TermWord = AuthTermsAr.TermsAr[i].TermWord;
                        newEl.Pos.Add(null);
                        TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(AuthTermsAr.TermsAr[i].Pos[0].range);
                        if (e == null)
                        {
                            MainArrayTermsAr.rootTermsTree.AddRange(AuthTermsAr.TermsAr[i].Pos[0].range);
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            e = MainArrayTermsAr.rootTermsTree.FindRange(AuthTermsAr.TermsAr[i].Pos[0].range);
                            e.indexElement.Add(MainArrayTermsAr.TermsAr.Count-1);
                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                        }
                        else
                        {
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                        }                     
				        
				        
				        for (int j = 1; j < AuthTermsAr.TermsAr[i].Pos.Count; j++)
				        {
                            TermTree node = MainArrayTermsAr.rootTermsTree.FindRange(AuthTermsAr.TermsAr[i].Pos[j].range);
                            if (node == null)
                            {
                                MainArrayTermsAr.rootTermsTree.AddRange(AuthTermsAr.TermsAr[i].Pos[j].range);
                                e = MainArrayTermsAr.rootTermsTree.FindRange(AuthTermsAr.TermsAr[i].Pos[j].range);
                                if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }
                            else
                            {
                                if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }
				        }
			        }
		        }
	        }
	        aux.DelElementsWhichSetToDel(AuthTermsAr);
	        aux.getFrequency_(MainArrayTermsAr);
        }//Work
        public void Rule2_Mdict_to_M(Terms DictTermsAr, NonDictTerms NonDictTermsAr, Terms MainArrayTermsAr)
        {
	        int sizeDict = DictTermsAr.TermsAr.Count;
	        for (int i = 0; i < sizeDict; i++)
	        {
		        int k = find.findINList(MainArrayTermsAr.TermsAr, DictTermsAr.TermsAr[i].TermWord);
		        if (k == -1)
		        {
			        bool NoComp = false;
			        for (int j = 0; j < DictTermsAr.TermsAr[i].Pos.Count; j++)
			        {
				        TermTree e = NonDictTermsAr.rootTermsTree.FindRangeExtension(DictTermsAr.TermsAr[i].Pos[j].range);
				        if (e == null)
				        {
					        NoComp = true;
				        }
				        if (NoComp == true) break;
			        }
			        if (NoComp == true)
			        {
				        DictTermsAr.TermsAr[i].setToDel = true;
				        Term newEl = new Term();
				        newEl.frequency = DictTermsAr.TermsAr[i].frequency;
                        newEl.kind = KindOfTerm.DictTerm;
                        newEl.NPattern = DictTermsAr.TermsAr[i].NPattern;
                        newEl.PatCounter = 0;
                        newEl.Pattern = DictTermsAr.TermsAr[i].Pattern;
                        newEl.Pos.Add(null);
                        newEl.setToDel = false;                        
                        newEl.TermFragment = DictTermsAr.TermsAr[i].TermFragment;
                        newEl.TermWord = DictTermsAr.TermsAr[i].TermWord;
				        newEl.TermWord = DictTermsAr.TermsAr[i].TermWord;
                        TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[i].Pos[0].range);
                        if (e == null)
                        {
                            MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[i].Pos[0].range);
                            e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[i].Pos[0].range);
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                        }
                        else
                        {
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                        }		           
				        for (int j = 1; j < DictTermsAr.TermsAr[i].Pos.Count; j++)
				        {
                            TermTree node = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[i].Pos[j].range);
                            if (node == null)
                            {
                                MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[i].Pos[j].range);
                                e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[i].Pos[j].range);
                                if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }
                            else
                            {
                                if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }					       
				        }
			        }
		        }
	        }
	        /*for(int i=0; i<sizeDict; i++)
	        {
	        string str=DictTermsAr.TermsAr[i].TermWord;
	        ComponentInElement find=find.findINListComponents(NonDictTermsAr.TermsAr, str);
	        if (find.Component != -1)
	        {
	        if (find.findINList(MainArrayTermsAr.TermsAr,DictTermsAr.TermsAr[i].TermWord) == -1)
	        MainArrayTermsAr.TermsAr.Add(DictTermsAr.TermsAr[i]);
	        }
	        else if (find.findINList(NonDictTermsAr.TermsAr,str) != -1)
	        {
	        if (find.findINList(MainArrayTermsAr.TermsAr,DictTermsAr.TermsAr[i].TermWord) == -1)
	        MainArrayTermsAr.TermsAr.Add(DictTermsAr.TermsAr[i]);
	        }
	        }*/
	        aux.getFrequency_(MainArrayTermsAr);
        }
        public void Rule3_Mnondict_to_M(Terms DictTermsAr, NonDictTerms NonDictTermsAr, CombTerms CombTermsAr, Terms MainArrayTermsAr)//Переписать!!!
        {            
	        int s_ND = NonDictTermsAr.TermsAr.Count;
	        //vector<int> DictTermsToDel;
	        for (int i = 0; i < s_ND; i++)
	        {
		        int res = find.findINList(CombTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].TermWord);
		        if (res != -1)
		        {
			        bool del = false;
			        for (int j = 0; j < NonDictTermsAr.TermsAr[i].Components.Count; j++)
			        {
				        bool isDictComponents = false;
				        List<int> countDictComponents = new List<int>();
				        for (int k = 0; k < NonDictTermsAr.TermsAr[i].Components[j].Count; k++)
				        {
					        isDictComponents = false;
					        int r = find.findINList(DictTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].Components[j][k].Component);
					        if (r != -1)
					        {
						        //isDictComponents = true;
						        countDictComponents.Add(r);
					        }
					        //if (!isDictComponents) break;
				        }
				        //if (isDictComponents)
				        if (countDictComponents.Count == NonDictTermsAr.TermsAr[i].Components[j].Count)
				        {
					        del = true;
					        while (countDictComponents.Count > 0)
					        {
						        DictTermsAr.TermsAr[countDictComponents[0]].setToDel = true;
						        //DictTermsToDel.insert(DictTermsToDel.end(), countDictComponents.begin(), countDictComponents.end());
						        if (find.findINList(MainArrayTermsAr.TermsAr, DictTermsAr.TermsAr[countDictComponents[0]].TermWord) == -1)
						        {
                                    Term newEl = new Term(); ;
							        newEl.frequency = DictTermsAr.TermsAr[countDictComponents[0]].frequency;
							        newEl.TermWord = DictTermsAr.TermsAr[countDictComponents[0]].TermWord;
							        newEl.Pos.Add(null);
                                    newEl.TermFragment = DictTermsAr.TermsAr[i].TermFragment;
							        MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[0].range);
							        MainArrayTermsAr.TermsAr.Add(newEl);
							        TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[0].range);
							        e.indexElement = MainArrayTermsAr.TermsAr.Count - 1;
							        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
							        for (int k = 1; k < DictTermsAr.TermsAr[countDictComponents[0]].Pos.Count; k++)
							        {
								        MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[k].range);
								        e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[k].range);
								        e.indexElement = MainArrayTermsAr.TermsAr.Count - 1;
								        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(e);
							        }
							        countDictComponents.RemoveAt(0);							        
						        }
						        else
						        {
							        countDictComponents.RemoveAt(0);
						        }
					        }
				        }
				        else if (countDictComponents.Count == 1)
				        {
					        isDictComponents = false;
					        for (int k = 0; k < DictTermsAr.TermsAr[countDictComponents[0]].Pos.Count; k++)
					        {
						        isDictComponents = false;
						        TermTree e = NonDictTermsAr.rootTermsTree.FindRangeExtension(DictTermsAr.TermsAr[countDictComponents[0]].Pos[k].range);
						        if (e != null && e.indexElement==i)
						        {
							        isDictComponents = true;
							        //countDictComponents.Add(r);
						        }
						        if (!isDictComponents) break;
					        }
					        if (isDictComponents)
					        {
						        DictTermsAr.TermsAr[countDictComponents[0]].setToDel = true;
						        //DictTermsToDel.Add(countDictComponents[0]);
						        if (find.findINList(MainArrayTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].TermWord) == -1)
						        {
							        Term newEl = new Term();
							        newEl.frequency =NonDictTermsAr.TermsAr[i].frequency;
							        newEl.TermWord = NonDictTermsAr.TermsAr[i].TermWord;
							        newEl.Pos.Add(null);
                                    newEl.TermFragment = NonDictTermsAr.TermsAr[i].TermFragment;
							        MainArrayTermsAr.rootTermsTree.AddRange(NonDictTermsAr.TermsAr[i].Pos[0].range);
							        MainArrayTermsAr.TermsAr.Add(newEl);
							        TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(NonDictTermsAr.TermsAr[i].Pos[0].range);
							        e.indexElement = i;
							        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
							        for (int k = 1; k < NonDictTermsAr.TermsAr[i].Pos.Count; k++)
							        {
								        MainArrayTermsAr.rootTermsTree.AddRange(NonDictTermsAr.TermsAr[i].Pos[k].range);
								        e = MainArrayTermsAr.rootTermsTree.FindRange(NonDictTermsAr.TermsAr[i].Pos[k].range);
								        e.indexElement = MainArrayTermsAr.TermsAr.Count - 1;
								        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(e);
							        }
							        countDictComponents.RemoveAt(0);
						        }
					        }
				        }
			        }
			        if (del)//удаление
			        {
				        for (int k = 0; k < NonDictTermsAr.TermsAr[i].Pos.Count; k++)
				        {
					        NonDictTermsAr.rootTermsTree.DeleteRange(NonDictTermsAr.TermsAr[i].Pos[k].range);
					        NonDictTermsAr.TermsAr[i].Pos.RemoveAt(k);
					        k--;
				        }
				        NonDictTermsAr.TermsAr.RemoveAt(i);
				        i--;
				        s_ND = NonDictTermsAr.TermsAr.Count;
				        for (int k = 0; k < CombTermsAr.TermsAr[res].Pos.Count; k++)
				        {
					        CombTermsAr.rootTermsTree.DeleteRange(CombTermsAr.TermsAr[res].Pos[k].range);
					        CombTermsAr.TermsAr[res].Pos.RemoveAt(k);
					        k--;
				        }
				        CombTermsAr.TermsAr.RemoveAt(res);
				        //===========Заглушка============
				        for (int p = 0; p < NonDictTermsAr.TermsAr.Count; p++)
				        {
					        for (int j = 0; j < NonDictTermsAr.TermsAr[p].Pos.Count; j++)
						        NonDictTermsAr.TermsAr[p].Pos[j].indexElement = p;
				        }
				        for (int p = 0; p < CombTermsAr.TermsAr.Count; p++)
				        {
					        for (int j = 0; j < CombTermsAr.TermsAr[p].Pos.Count; j++)
						        CombTermsAr.TermsAr[p].Pos[j].indexElement = p;
				        }
				        //==================================
			        }
		        }

	        }
	        aux.DelElementsWhichSetToDel(DictTermsAr);
	        aux.getFrequency_(MainArrayTermsAr);
        }
        public void Rule_from_4_to_6(Terms MainTermsAr)
        {
            //TermsProcessing proc = new TermsProcessing();
	        SynTerms SynTermsAr = new SynTerms();
	        proc.GetXMLSynTerms(SynTermsAr);
	        Rule4_Msyn_to_M(MainTermsAr, SynTermsAr);
        }
        public void Rule4_Msyn_to_M(Terms MainTermsAr, SynTerms SynTermsAr)
        {
            FindFunctions find = new FindFunctions();
	        for (int i = 0; i < SynTermsAr.TermsAr.Count; i++)
	        {
		        int first_alt = find.findINList(MainTermsAr.TermsAr, SynTermsAr.TermsAr[i].alternatives.first.alternative);
		        int second_alt = find.findINList(MainTermsAr.TermsAr, SynTermsAr.TermsAr[i].alternatives.second.alternative);
		        if (first_alt != -1 && second_alt == -1)
		        {
			        SynTermsAr.TermsAr[i].setToDel = true;
			        Term newEl = new Term();
			        newEl.frequency = MainTermsAr.TermsAr[first_alt].frequency;
			        newEl.TermWord = SynTermsAr.TermsAr[i].alternatives.second.alternative;
			        newEl.setToDel = false;
                    newEl.TermFragment = SynTermsAr.TermsAr[i].TermFragment;
			        MainTermsAr.TermsAr.Add(newEl);
			        for (int j = 1; j < MainTermsAr.TermsAr[first_alt].Pos.Count; j++)
			        {
				        MainTermsAr.TermsAr[MainTermsAr.TermsAr.Count - 1].Pos.Add(MainTermsAr.TermsAr[first_alt].Pos[j]);
			        }
		        }
		        else if (first_alt == -1 && second_alt != -1)
		        {
			        SynTermsAr.TermsAr[i].setToDel = true;
			        Term newEl = new Term();
			        newEl.frequency = MainTermsAr.TermsAr[second_alt].frequency;
			        newEl.TermWord = SynTermsAr.TermsAr[i].alternatives.first.alternative;
			        newEl.setToDel = false;
                    newEl.TermFragment = SynTermsAr.TermsAr[i].TermFragment;
			        MainTermsAr.TermsAr.Add(newEl);
			        for (int j = 1; j < MainTermsAr.TermsAr[second_alt].Pos.Count; j++)
			        {
				        MainTermsAr.TermsAr[MainTermsAr.TermsAr.Count - 1].Pos.Add(MainTermsAr.TermsAr[second_alt].Pos[j]);
			        }
		        }
	        }
	        aux.DelElementsWhichSetToDel(SynTermsAr);
	        aux.getFrequency_(MainTermsAr);
        }
        public void Rule5_Mconj_to_M()
        {

        }
        public void ApplyRules()
        {
            // Инициализируем массивы терминов
            proc.GetXMLAuthTerms(proc.AuthTermsAr);
            proc.GetXMLCombTerms(proc.CombTermsAr);
            proc.GetXMLDictTerms(proc.DictTermsAr);
            proc.GetXMLNonDictTerms(proc.NonDictTermsAr);
            //Применение правила 1
            Rule1_Mauth_to_M(proc.AuthTermsAr, proc.MainTermsAr);
            //Правило 2
            Rule2_Mdict_to_M(proc.DictTermsAr, proc.NonDictTermsAr, proc.MainTermsAr);
            //Правило 3
            Rule3_Mnondict_to_M(proc.DictTermsAr, proc.NonDictTermsAr, proc.CombTermsAr, proc.MainTermsAr);
            //Правило 4
            Rule_from_4_to_6(proc.MainTermsAr);
        }
    }
}

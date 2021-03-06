﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TermProcessingNamespace;
using TermsNamespace;
using FindFunctionsNamespace;
using TermTreeNamespace;
using AuxiliaryFunctionsNamespace;

namespace RulesNamespace
{
    public class Rules
    {
        public Rules(string inputfile, DictionaryF dict, int startPage, int endPage, bool ClearSupportLists) {
            proc = new TermsProcessing(inputfile, dict, startPage, endPage, ClearSupportLists);
        }

        private TermsProcessing proc;
        private void Rule1_Mauth_to_M(Terms AuthTermsAr, Terms MainArrayTermsAr)
        {
	        for (int i = 0; i < AuthTermsAr.TermsAr.Count; i++)
	        {
                //int k = FindFunctions.findINList(MainArrayTermsAr.TermsAr, AuthTermsAr.TermsAr[i].TermWord);
                int k = MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == AuthTermsAr.TermsAr[i].TermWord && item.POSstr == AuthTermsAr.TermsAr[i].POSstr);
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
                        newEl.POSstr = AuthTermsAr.TermsAr[i].POSstr;
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
                                node = MainArrayTermsAr.rootTermsTree.FindRange(AuthTermsAr.TermsAr[i].Pos[j].range);
                                if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }
                            else
                            {
                                if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                    node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }
				        }
			        }
		        }
	        }
            AuxiliaryFunctions.DelElementsWhichSetToDel(AuthTermsAr);
            AuxiliaryFunctions.getFrequency_(MainArrayTermsAr);
        }//Work
        private void Rule2_Mdict_to_M(Terms DictTermsAr, NonDictTerms NonDictTermsAr, Terms MainArrayTermsAr)
        {
	        int sizeDict = DictTermsAr.TermsAr.Count;
	        for (int i = 0; i < sizeDict; i++)
	        {
		        //int k = FindFunctions.findINList(MainArrayTermsAr.TermsAr, DictTermsAr.TermsAr[i].TermWord);
                int k = MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == DictTermsAr.TermsAr[i].TermWord && item.POSstr == DictTermsAr.TermsAr[i].POSstr);
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
                        newEl.POSstr = DictTermsAr.TermsAr[i].POSstr;
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
                            if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                        }		           
				        for (int j = 1; j < DictTermsAr.TermsAr[i].Pos.Count; j++)
				        {
                            TermTree node = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[i].Pos[j].range);
                            if (node == null)
                            {
                                MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[i].Pos[j].range);
                                node = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[i].Pos[j].range);
                                if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                    node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }
                            else
                            {
                                if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                    node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                            }					       
				        }
			        }
		        }
	        }
	        /*for(int i=0; i<sizeDict; i++)
	        {
	        string str=DictTermsAr.TermsAr[i].TermWord;
	        ComponentInElement find=FindFunctions.findINListComponents(NonDictTermsAr.TermsAr, str);
	        if (FindFunctions.Component != -1)
	        {
	        if (FindFunctions.findINList(MainArrayTermsAr.TermsAr,DictTermsAr.TermsAr[i].TermWord) == -1)
	        MainArrayTermsAr.TermsAr.Add(DictTermsAr.TermsAr[i]);
	        }
	        else if (FindFunctions.findINList(NonDictTermsAr.TermsAr,str) != -1)
	        {
	        if (FindFunctions.findINList(MainArrayTermsAr.TermsAr,DictTermsAr.TermsAr[i].TermWord) == -1)
	        MainArrayTermsAr.TermsAr.Add(DictTermsAr.TermsAr[i]);
	        }
	        }*/
	        AuxiliaryFunctions.getFrequency_(MainArrayTermsAr);
        }
        private void Rule3_Mnondict_to_M(Terms DictTermsAr, NonDictTerms NonDictTermsAr, CombTerms CombTermsAr, Terms MainArrayTermsAr)//Переписать!!!
        {            
	        int s_ND = NonDictTermsAr.TermsAr.Count;
	        //vector<int> DictTermsToDel;
	        for (int i = 0; i < s_ND; i++)
	        {
		        //int res = FindFunctions.findINList(CombTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].TermWord);
                int res = CombTermsAr.TermsAr.FindIndex(item => item.TermWord == NonDictTermsAr.TermsAr[i].TermWord);
		        if (res != -1)
		        {
			        bool del = false;
			        for (int j = 0; j < NonDictTermsAr.TermsAr[i].Blocks.Count; j++)
			        {
				        bool isDictComponents = false;
				        List<int> countDictComponents = new List<int>();
				        for (int k = 0; k < NonDictTermsAr.TermsAr[i].Blocks[j].Components.Count; k++)
				        {
					        isDictComponents = false;
					        //int r = FindFunctions.findINList(DictTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].Blocks[j].Components[k].Component);
                            int r = DictTermsAr.TermsAr.FindIndex(item => item.TermWord == NonDictTermsAr.TermsAr[i].Blocks[j].Components[k].Component &&
                                item.POSstr == NonDictTermsAr.TermsAr[i].Blocks[j].Components[k].POSstr);
					        if (r != -1)
					        {
						        //isDictComponents = true;
						        countDictComponents.Add(r);
					        }
					        //if (!isDictComponents) break;
				        }
				        //if (isDictComponents)
				        if (countDictComponents.Count == NonDictTermsAr.TermsAr[i].Blocks[j].Components.Count)
				        {
					        del = true;
					        while (countDictComponents.Count > 0)
					        {
						        DictTermsAr.TermsAr[countDictComponents[0]].setToDel = true;
                                int index = countDictComponents[0];
						        //DictTermsToDel.insert(DictTermsToDel.end(), countDictComponents.begin(), countDictComponents.end());
						        //if (FindFunctions.findINList(MainArrayTermsAr.TermsAr, DictTermsAr.TermsAr[countDictComponents[0]].TermWord) == -1)
                                if (MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == DictTermsAr.TermsAr[index].TermWord &&
                                    item.POSstr == DictTermsAr.TermsAr[index].POSstr) == -1)
						        {
                                    Term newEl = new Term();
							        newEl.frequency = DictTermsAr.TermsAr[countDictComponents[0]].frequency;
                                    newEl.kind = KindOfTerm.DictTerm;
                                    newEl.NPattern = DictTermsAr.TermsAr[countDictComponents[0]].NPattern;
                                    newEl.PatCounter = 0;
                                    newEl.Pattern = DictTermsAr.TermsAr[countDictComponents[0]].Pattern;
                                    newEl.Pos.Add(null);
                                    newEl.setToDel = false;
                                    newEl.TermFragment = DictTermsAr.TermsAr[countDictComponents[0]].TermFragment;
                                    newEl.POSstr = DictTermsAr.TermsAr[countDictComponents[0]].POSstr;
                                    newEl.TermWord = DictTermsAr.TermsAr[countDictComponents[0]].TermWord;
							        TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[0].range);
                                    if (e == null)
                                    {
                                        MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[0].range);
                                        e = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[0].range);
                                        MainArrayTermsAr.TermsAr.Add(newEl);
                                        e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                    }
                                    else
                                    {
                                        MainArrayTermsAr.TermsAr.Add(newEl);
                                        if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                            e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                    }
							        for (int k = 1; k < DictTermsAr.TermsAr[countDictComponents[0]].Pos.Count; k++)
							        {
                                        TermTree node = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[k].range);
                                        if (node == null)
                                        {
                                            MainArrayTermsAr.rootTermsTree.AddRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[k].range);
                                            node = MainArrayTermsAr.rootTermsTree.FindRange(DictTermsAr.TermsAr[countDictComponents[0]].Pos[k].range);
                                            if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                                node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                        }
                                        else
                                        {
                                            if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                                node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                        }
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
						        if (e != null && e.indexElement.Contains(i))
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
						        //if (FindFunctions.findINList(MainArrayTermsAr.TermsAr, NonDictTermsAr.TermsAr[i].TermWord) == -1)
                                if (MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == NonDictTermsAr.TermsAr[i].TermWord &&
                                    item.POSstr == NonDictTermsAr.TermsAr[i].POSstr) == -1)
						        {
							        Term newEl = new Term();
							        newEl.frequency = NonDictTermsAr.TermsAr[i].frequency;
                                    newEl.kind = KindOfTerm.NonDictTerm;
                                    newEl.NPattern = NonDictTermsAr.TermsAr[i].NPattern;
                                    newEl.PatCounter = 0;
                                    newEl.Pattern = NonDictTermsAr.TermsAr[i].Pattern;
                                    newEl.Pos.Add(null);
                                    newEl.setToDel = false;
                                    newEl.TermFragment = NonDictTermsAr.TermsAr[i].TermFragment;
                                    newEl.TermWord = NonDictTermsAr.TermsAr[i].TermWord;
                                    newEl.POSstr = NonDictTermsAr.TermsAr[i].POSstr;
                                    TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(NonDictTermsAr.TermsAr[i].Pos[0].range);
                                    if (e == null)
                                    {
                                        MainArrayTermsAr.rootTermsTree.AddRange(NonDictTermsAr.TermsAr[i].Pos[0].range);
                                        e = MainArrayTermsAr.rootTermsTree.FindRange(NonDictTermsAr.TermsAr[i].Pos[0].range);
                                        MainArrayTermsAr.TermsAr.Add(newEl);
                                        e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                    }
                                    else
                                    {
                                        MainArrayTermsAr.TermsAr.Add(newEl);
                                        if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                            e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                        MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                    }
							        for (int k = 1; k < NonDictTermsAr.TermsAr[i].Pos.Count; k++)
							        {
                                        TermTree node = MainArrayTermsAr.rootTermsTree.FindRange(NonDictTermsAr.TermsAr[i].Pos[k].range);
                                        if (node == null)
                                        {
                                            MainArrayTermsAr.rootTermsTree.AddRange(NonDictTermsAr.TermsAr[i].Pos[k].range);
                                            node = MainArrayTermsAr.rootTermsTree.FindRange(NonDictTermsAr.TermsAr[i].Pos[k].range);
                                            if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                                node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                        }
                                        else
                                        {
                                            if (!node.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1)) 
                                                node.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                            MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Add(node);
                                        }
							        }
							        countDictComponents.RemoveAt(0);
						        }
					        }
				        }
			        }
			        if (del)//удаление
			        {
                        while (NonDictTermsAr.TermsAr[i].Pos.Count>0)
				        {
                            int l = NonDictTermsAr.TermsAr[i].Pos[0].indexElement.FindIndex(item => item == i);
                            NonDictTermsAr.TermsAr[i].Pos[0].indexElement.RemoveAt(l);
                            if (NonDictTermsAr.TermsAr[i].Pos[0].indexElement.Count == 0)
                            {
                                Range del_range = new Range(NonDictTermsAr.TermsAr[i].Pos[0].range.inf);
                                NonDictTermsAr.rootTermsTree.DeleteRange(del_range);
                            }					        
					        NonDictTermsAr.TermsAr[i].Pos.RemoveAt(0);
				        }
				        NonDictTermsAr.TermsAr.RemoveAt(i);
                        AuxiliaryFunctions.ChangeIdexesInTree(NonDictTermsAr, i);
				        i--;
				        s_ND = NonDictTermsAr.TermsAr.Count;
				        while(CombTermsAr.TermsAr[res].Pos.Count>0)
				        {
                            int l = CombTermsAr.TermsAr[res].Pos[0].indexElement.FindIndex(item => item == res);
                            CombTermsAr.TermsAr[res].Pos[0].indexElement.RemoveAt(l);
                            if (CombTermsAr.TermsAr[res].Pos[0].indexElement.Count == 0)
                            {
                                Range del_range = new Range(CombTermsAr.TermsAr[res].Pos[0].range.inf);
                                CombTermsAr.rootTermsTree.DeleteRange(del_range);
                            }					        
					        CombTermsAr.TermsAr[res].Pos.RemoveAt(0);
				        }
				        CombTermsAr.TermsAr.RemoveAt(res);
                        AuxiliaryFunctions.ChangeIdexesInTree(CombTermsAr, res);
				        //===========Заглушка============
                        //for (int p = 0; p < NonDictTermsAr.TermsAr.Count; p++)
                        //{
                        //    for (int j = 0; j < NonDictTermsAr.TermsAr[p].Pos.Count; j++)
                        //        if (!NonDictTermsAr.TermsAr[p].Pos[j].indexElement.Contains(p)) NonDictTermsAr.TermsAr[p].Pos[j].indexElement.Add(p);
                        //}
                        //for (int p = 0; p < CombTermsAr.TermsAr.Count; p++)
                        //{
                        //    for (int j = 0; j < CombTermsAr.TermsAr[p].Pos.Count; j++)
                        //        if (!CombTermsAr.TermsAr[p].Pos[j].indexElement.Contains(p)) CombTermsAr.TermsAr[p].Pos[j].indexElement.Add(p);
                        //}
				        //==================================
			        }
		        }

	        }
	        AuxiliaryFunctions.DelElementsWhichSetToDel(DictTermsAr);
	        AuxiliaryFunctions.getFrequency_(MainArrayTermsAr);
        }
        private void Rule_from_4_to_6(Terms MainTermsAr)
        {
            //TermsProcessing proc = new TermsProcessing();
	        SynTerms SynTermsAr = new SynTerms();
	        proc.GetXMLSynTerms(SynTermsAr);
	        Rule4_Msyn_to_M(MainTermsAr, SynTermsAr);
        }
        private void Rule4_Msyn_to_M(Terms MainTermsAr, SynTerms SynTermsAr)
        {
            //FindFunctions find = new FindFunctions();
	        for (int i = 0; i < SynTermsAr.TermsAr.Count; i++)
	        {
		        //int first_alt = FindFunctions.findINList(MainTermsAr.TermsAr, SynTermsAr.TermsAr[i].alternatives.first.alternative);
		        //int second_alt = FindFunctions.findINList(MainTermsAr.TermsAr, SynTermsAr.TermsAr[i].alternatives.second.alternative);
                int first_alt = MainTermsAr.TermsAr.FindIndex(item => item.TermWord == SynTermsAr.TermsAr[i].alternatives.first.alternative &&
                    item.POSstr == SynTermsAr.TermsAr[i].alternatives.first.POSstr);
                int second_alt = MainTermsAr.TermsAr.FindIndex(item => item.TermWord == SynTermsAr.TermsAr[i].alternatives.second.alternative &&
                    item.POSstr == SynTermsAr.TermsAr[i].alternatives.second.POSstr);
		        if (first_alt != -1 && second_alt == -1)
		        {
			        SynTermsAr.TermsAr[i].setToDel = true;
			        Term newEl = new Term();
			        newEl.frequency = MainTermsAr.TermsAr[first_alt].frequency;
                    newEl.kind = KindOfTerm.SynTerm;
                    newEl.NPattern = SynTermsAr.TermsAr[i].alternatives.second.NPattern;
                    newEl.PatCounter = 0;
                    newEl.Pattern = SynTermsAr.TermsAr[i].alternatives.second.Pattern;
                    //newEl.Pos.Add(null);
                    newEl.setToDel = false;
                    newEl.POSstr = SynTermsAr.TermsAr[i].alternatives.second.POSstr;
                    newEl.TermFragment = SynTermsAr.TermsAr[i].TermFragment;
                    newEl.TermWord = SynTermsAr.TermsAr[i].alternatives.second.alternative;
			        MainTermsAr.TermsAr.Add(newEl);
			        for (int j = 1; j < MainTermsAr.TermsAr[first_alt].Pos.Count; j++)
			        {
				        MainTermsAr.TermsAr[MainTermsAr.TermsAr.Count - 1].Pos.Add(MainTermsAr.TermsAr[first_alt].Pos[j]);
                        if (!MainTermsAr.TermsAr[first_alt].Pos[j].indexElement.Contains(MainTermsAr.TermsAr.Count - 1)) 
                            MainTermsAr.TermsAr[first_alt].Pos[j].indexElement.Add(MainTermsAr.TermsAr.Count - 1);
			        }
		        }
		        else if (first_alt == -1 && second_alt != -1)
		        {
			        SynTermsAr.TermsAr[i].setToDel = true;
			        Term newEl = new Term();
                    newEl.frequency = MainTermsAr.TermsAr[second_alt].frequency;
                    newEl.kind = KindOfTerm.SynTerm;
                    newEl.NPattern = SynTermsAr.TermsAr[i].alternatives.first.NPattern;
                    newEl.PatCounter = 0;
                    newEl.Pattern = SynTermsAr.TermsAr[i].alternatives.first.Pattern;
                    //newEl.Pos.Add(null);
                    newEl.setToDel = false;
                    newEl.TermFragment = SynTermsAr.TermsAr[i].TermFragment;
                    newEl.TermWord = SynTermsAr.TermsAr[i].alternatives.first.alternative;
                    newEl.POSstr = SynTermsAr.TermsAr[i].alternatives.first.POSstr;
			        MainTermsAr.TermsAr.Add(newEl);
			        for (int j = 1; j < MainTermsAr.TermsAr[second_alt].Pos.Count; j++)
			        {
				        MainTermsAr.TermsAr[MainTermsAr.TermsAr.Count - 1].Pos.Add(MainTermsAr.TermsAr[second_alt].Pos[j]);
                        if (!MainTermsAr.TermsAr[second_alt].Pos[j].indexElement.Contains(MainTermsAr.TermsAr.Count - 1)) 
                            MainTermsAr.TermsAr[second_alt].Pos[j].indexElement.Add(MainTermsAr.TermsAr.Count - 1);
			        }
		        }
	        }
	        AuxiliaryFunctions.DelElementsWhichSetToDel(SynTermsAr);
	        AuxiliaryFunctions.getFrequency_(MainTermsAr);
        }
        private void Rule6_Mcomb_to_M(Terms DictTermsAr, NonDictTerms NonDictTermsAr, CombTerms CombTermsAr, Terms MainArrayTermsAr)
        {
            int s_Comb = CombTermsAr.TermsAr.Count;
            for (int i = 0; i < s_Comb; i++)
            {
                int res_NonDict = NonDictTermsAr.TermsAr.FindIndex(item =>
                    CombTermsAr.TermsAr[i].Components.FindIndex(comp_item =>
                        comp_item.TermWord == item.TermWord && comp_item.POSstr == item.POSstr) != -1);
                if (res_NonDict != -1)
                {
                    for (int k = 0; k < CombTermsAr.TermsAr[i].Components.Count; k = 0)
                    {
                        if (MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == CombTermsAr.TermsAr[i].Components[k].TermWord &&
                                    item.POSstr == CombTermsAr.TermsAr[i].Components[k].POSstr) == -1)
                        {
                            Term newEl = new Term();
                            newEl.frequency = CombTermsAr.TermsAr[i].Components[k].frequency;
                            newEl.kind = KindOfTerm.CombTerm;
                            newEl.NPattern = CombTermsAr.TermsAr[i].Components[k].NPattern;
                            newEl.PatCounter = 0;
                            newEl.Pattern = CombTermsAr.TermsAr[i].Components[k].Pattern;
                            newEl.Pos.Add(null);
                            newEl.setToDel = false;
                            newEl.TermFragment = CombTermsAr.TermsAr[i].Components[k].TermFragment;
                            newEl.POSstr = CombTermsAr.TermsAr[i].Components[k].POSstr;
                            newEl.TermWord = CombTermsAr.TermsAr[i].Components[k].TermWord;
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            for (int j = 0; j < CombTermsAr.TermsAr[i].Pos.Count; j++)
                            {
                                TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                if (e == null)
                                {
                                    MainArrayTermsAr.rootTermsTree.AddRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                    e = MainArrayTermsAr.rootTermsTree.FindRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                    e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                    MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;

                                }
                                else
                                {
                                    if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1))
                                        e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                    MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                            }
                        }
                    }
                    for (int j = 0; j < CombTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        CombTermsAr.TermsAr[i].Pos[j].indexElement.Remove(i);
                        if (CombTermsAr.TermsAr[i].Pos[j].indexElement.Count == 0)
                            CombTermsAr.rootTermsTree.DeleteRange(CombTermsAr.TermsAr[i].Pos[j].range);
                        CombTermsAr.TermsAr[i].Pos.RemoveAt(j);
                    }
                    CombTermsAr.TermsAr.RemoveAt(i);
                    for (int j = 0; j < NonDictTermsAr.TermsAr[res_NonDict].Pos.Count; j++)
                    {
                        NonDictTermsAr.TermsAr[res_NonDict].Pos[j].indexElement.Remove(res_NonDict);
                        if (NonDictTermsAr.TermsAr[res_NonDict].Pos[j].indexElement.Count == 0)
                            NonDictTermsAr.rootTermsTree.DeleteRange(NonDictTermsAr.TermsAr[res_NonDict].Pos[j].range);
                        NonDictTermsAr.TermsAr[res_NonDict].Pos.RemoveAt(j);
                    }
                    NonDictTermsAr.TermsAr.RemoveAt(res_NonDict);
                    s_Comb--;
                    s_Comb = CombTermsAr.TermsAr.Count;
                    continue;
                }
                int res_Dict = DictTermsAr.TermsAr.FindIndex(item =>
                   CombTermsAr.TermsAr[i].Components.FindIndex(comp_item =>
                       comp_item.TermWord == item.TermWord && comp_item.POSstr == item.POSstr) != -1);
                if (res_Dict != -1)
                {
                    for (int k = 0; k < CombTermsAr.TermsAr[i].Components.Count; k = 0)
                    {
                        if (MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == CombTermsAr.TermsAr[i].Components[k].TermWord &&
                                    item.POSstr == CombTermsAr.TermsAr[i].Components[k].POSstr) == -1)
                        {
                            Term newEl = new Term();
                            newEl.frequency = CombTermsAr.TermsAr[i].Components[k].frequency;
                            newEl.kind = KindOfTerm.CombTerm;
                            newEl.NPattern = CombTermsAr.TermsAr[i].Components[k].NPattern;
                            newEl.PatCounter = 0;
                            newEl.Pattern = CombTermsAr.TermsAr[i].Components[k].Pattern;
                            newEl.Pos.Add(null);
                            newEl.setToDel = false;
                            newEl.TermFragment = CombTermsAr.TermsAr[i].Components[k].TermFragment;
                            newEl.POSstr = CombTermsAr.TermsAr[i].Components[k].POSstr;
                            newEl.TermWord = CombTermsAr.TermsAr[i].Components[k].TermWord;
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            for (int j = 0; j < CombTermsAr.TermsAr[i].Pos.Count; j++)
                            {
                                TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                if (e == null)
                                {
                                    MainArrayTermsAr.rootTermsTree.AddRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                    e = MainArrayTermsAr.rootTermsTree.FindRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                    e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                    MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;

                                }
                                else
                                {
                                    if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1))
                                        e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                    MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                            }


                        }
                    }
                    for (int j = 0; j < CombTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        CombTermsAr.TermsAr[i].Pos[j].indexElement.Remove(i);
                        if (CombTermsAr.TermsAr[i].Pos[j].indexElement.Count == 0)
                            CombTermsAr.rootTermsTree.DeleteRange(CombTermsAr.TermsAr[i].Pos[j].range);
                        CombTermsAr.TermsAr[i].Pos.RemoveAt(j);
                    }
                    CombTermsAr.TermsAr.RemoveAt(i);
                    for (int j = 0; j < NonDictTermsAr.TermsAr[res_Dict].Pos.Count; j++)
                    {
                        NonDictTermsAr.TermsAr[res_Dict].Pos[j].indexElement.Remove(res_NonDict);
                        if (NonDictTermsAr.TermsAr[res_Dict].Pos[j].indexElement.Count == 0)
                            NonDictTermsAr.rootTermsTree.DeleteRange(NonDictTermsAr.TermsAr[res_Dict].Pos[j].range);
                        NonDictTermsAr.TermsAr[res_Dict].Pos.RemoveAt(j);
                    }
                    DictTermsAr.TermsAr.RemoveAt(res_Dict);
                    s_Comb--;
                    s_Comb = CombTermsAr.TermsAr.Count;
                    continue;
                }
                int res_Main = MainArrayTermsAr.TermsAr.FindIndex(item =>
                   CombTermsAr.TermsAr[i].Components.FindIndex(comp_item =>
                       comp_item.TermWord == item.TermWord && comp_item.POSstr == item.POSstr) != -1);
                if (res_Dict != -1)
                {
                    for (int k = 0; k < CombTermsAr.TermsAr[i].Components.Count; k = 0)
                    {
                        if (MainArrayTermsAr.TermsAr.FindIndex(item => item.TermWord == CombTermsAr.TermsAr[i].Components[k].TermWord &&
                                    item.POSstr == CombTermsAr.TermsAr[i].Components[k].POSstr) == -1)
                        {
                            Term newEl = new Term();
                            newEl.frequency = CombTermsAr.TermsAr[i].Components[k].frequency;
                            newEl.kind = KindOfTerm.CombTerm;
                            newEl.NPattern = CombTermsAr.TermsAr[i].Components[k].NPattern;
                            newEl.PatCounter = 0;
                            newEl.Pattern = CombTermsAr.TermsAr[i].Components[k].Pattern;
                            newEl.Pos.Add(null);
                            newEl.setToDel = false;
                            newEl.TermFragment = CombTermsAr.TermsAr[i].Components[k].TermFragment;
                            newEl.POSstr = CombTermsAr.TermsAr[i].Components[k].POSstr;
                            newEl.TermWord = CombTermsAr.TermsAr[i].Components[k].TermWord;
                            MainArrayTermsAr.TermsAr.Add(newEl);
                            for (int j = 0; j < CombTermsAr.TermsAr[i].Pos.Count; j++)
                            {
                                TermTree e = MainArrayTermsAr.rootTermsTree.FindRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                if (e == null)
                                {
                                    MainArrayTermsAr.rootTermsTree.AddRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                    e = MainArrayTermsAr.rootTermsTree.FindRange(CombTermsAr.TermsAr[i].Pos[j].range);
                                    e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                    MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;

                                }
                                else
                                {
                                    if (!e.indexElement.Contains(MainArrayTermsAr.TermsAr.Count - 1))
                                        e.indexElement.Add(MainArrayTermsAr.TermsAr.Count - 1);
                                    MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos[MainArrayTermsAr.TermsAr[MainArrayTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                }
                            }


                        }
                    }
                    for (int j = 0; j < CombTermsAr.TermsAr[i].Pos.Count; j++)
                    {
                        CombTermsAr.TermsAr[i].Pos[j].indexElement.Remove(i);
                        if (CombTermsAr.TermsAr[i].Pos[j].indexElement.Count == 0)
                            CombTermsAr.rootTermsTree.DeleteRange(CombTermsAr.TermsAr[i].Pos[j].range);
                        CombTermsAr.TermsAr[i].Pos.RemoveAt(j);
                    }
                    CombTermsAr.TermsAr.RemoveAt(i);
                    s_Comb--;
                    s_Comb = CombTermsAr.TermsAr.Count;
                    continue;
                }
            }
        } //Протестировать
        //TODO: Добавить новое правило
        public Terms ApplyRules()
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

            return proc.MainTermsAr;
        }


    }
}

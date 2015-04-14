using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TermRules
{
    public class AuxiliaryFunctions
    {
        public AuxiliaryFunctions() { }
        public void getFrequency_(Terms MR)
        {
            int size = MR.TermsAr.Count;
            bool change = true;
            while (change)
            {
                change = false;
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    //vector<pair<unsigned, unsigned>> POS_forDelete;
                    //int count = 0; 
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        Range range = new Range(MR.TermsAr[i].Pos[j].range.inf);
                        if (MR.rootTermsTree.FindRangeExtension(range) != null)
                        {
                            //count++;
                            MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[j].range);
                            MR.TermsAr[i].Pos.RemoveAt(j);
                            //delete MR.TermsAr[i].Pos[j];
                            //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() + );
                            //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                            j--;
                        }
                    }
                    if (MR.TermsAr[i].Pos.Count == 0)
                    {
                        //for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                        //{
                        //POS_forDelete.push_back(MR.TermsAr[i].Pos[j]->inf);
                        //	MR.rootTermsTree = DeleteRange(MR.rootTermsTree, MR.TermsAr[i].Pos[j]->inf);
                        //	//delete MR.TermsAr[i].Pos[j];
                        //	MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() + j);
                        //	vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        //	j--;
                        //}
                        MR.TermsAr.RemoveAt(i);
                        //MR.TermsAr.erase(MR.TermsAr.begin() + i);
                        //vector<Term>(MR.TermsAr).swap(MR.TermsAr);
                        i--;
                    }
                    else
                    {
                        //for (int i = 0; i < POS_forDelete.Count; i++)
                        //{
                        //	TermTree* e = FindRange(MR.rootTermsTree, POS_forDelete[i]);
                        //	DeleteRange(MR.rootTermsTree, POS_forDelete[i]);
                        //}	
                        MR.TermsAr[i].frequency = MR.TermsAr[i].Pos.Count;
                    }
                }
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                        MR.TermsAr[i].Pos[j].indexElement = i;
                }
                if (size != MR.TermsAr.Count)
                {
                    change = true;
                    size = MR.TermsAr.Count;
                }
            }
        }
        public void getFrequency_(NonDictTerms MR)
        {
            int size = MR.TermsAr.Count;
            bool change = true;
            while (change)
            {
                change = false;
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    //vector<pair<unsigned, unsigned>> POS_forDelete;
                    //int count = 0; 
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        if (MR.rootTermsTree.FindRangeExtension(MR.TermsAr[i].Pos[j].range) != null)
                        {
                            //count++;
                            MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[j].range);
                            //delete MR.TermsAr[i].Pos[j];
                            //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() + j);
                            //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                            MR.TermsAr[i].Pos.RemoveAt(j);
                            j--;
                        }
                    }
                    if (MR.TermsAr[i].Pos.Count == 0)
                    {
                        //for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                        //{
                        //POS_forDelete.push_back(MR.TermsAr[i].Pos[j]->inf);
                        //	MR.rootTermsTree = DeleteRange(MR.rootTermsTree, MR.TermsAr[i].Pos[j]->inf);
                        //	//delete MR.TermsAr[i].Pos[j];
                        //	MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() + j);
                        //	vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        //	j--;
                        //}
                        //MR.TermsAr.erase(MR.TermsAr.begin() + i);
                        //vector<NonDictTerm>(MR.TermsAr).swap(MR.TermsAr);
                        MR.TermsAr.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        //for (int i = 0; i < POS_forDelete.Count; i++)
                        //{
                        //	TermTree* e = FindRange(MR.rootTermsTree, POS_forDelete[i]);
                        //	DeleteRange(MR.rootTermsTree, POS_forDelete[i]);
                        //}	
                        MR.TermsAr[i].frequency = MR.TermsAr[i].Pos.Count;
                    }
                }
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                        MR.TermsAr[i].Pos[j].indexElement = i;
                }
                if (size != MR.TermsAr.Count)
                {
                    change = true;
                    size = MR.TermsAr.Count;
                }
            }
        }
        public void getFrequency_(SynTerms MR)
        {
            int size = MR.TermsAr.Count;
            bool change = true;
            while (change)
            {
                change = false;
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    //vector<pair<unsigned, unsigned>> POS_forDelete;
                    //int count = 0; 
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        if (MR.rootTermsTree.FindRangeExtension(MR.TermsAr[i].Pos[j].range) != null)
                        {
                            //count++;
                            MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[j].range);
                            //delete MR.TermsAr[i].Pos[j];
                            //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() + j);
                            //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                            MR.TermsAr[i].Pos.RemoveAt(j);
                            j--;
                        }
                    }
                    if (MR.TermsAr[i].Pos.Count == 0)
                    {
                        //for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                        //{
                        //POS_forDelete.push_back(MR.TermsAr[i].Pos[j]->inf);
                        //	MR.rootTermsTree = DeleteRange(MR.rootTermsTree, MR.TermsAr[i].Pos[j]->inf);
                        //	//delete MR.TermsAr[i].Pos[j];
                        //	MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() + j);
                        //	vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        //	j--;
                        //}
                        //MR.TermsAr.erase(MR.TermsAr.begin() + i);
                        //vector<SynTerm>(MR.TermsAr).swap(MR.TermsAr);
                        MR.TermsAr.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        //for (int i = 0; i < POS_forDelete.Count; i++)
                        //{
                        //	TermTree* e = FindRange(MR.rootTermsTree, POS_forDelete[i]);
                        //	DeleteRange(MR.rootTermsTree, POS_forDelete[i]);
                        //}	
                        MR.TermsAr[i].frequency = MR.TermsAr[i].Pos.Count;
                    }
                }
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                        MR.TermsAr[i].Pos[j].indexElement = i;
                }
                if (size != MR.TermsAr.Count)
                {
                    change = true;
                    size = MR.TermsAr.Count;
                }
            }
        }
        public void DelElementsWhichSetToDel(Terms MR)
        {
            int sizeMR = MR.TermsAr.Count;
            for (int i = 0; i < sizeMR; i++)
            {
                if (MR.TermsAr[i].setToDel)
                {
                    while (MR.TermsAr[i].Pos.Count > 0)
                    {
                        MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[0].range);
                        //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() );
                        //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        MR.TermsAr[i].Pos.RemoveAt(0);
                    }
                    //MR.TermsAr.erase(MR.TermsAr.begin() + i);
                    //vector<Term>(MR.TermsAr).swap(MR.TermsAr);
                    MR.TermsAr.RemoveAt(i);
                    i--;
                    sizeMR = MR.TermsAr.Count;
                }
            }
            for (int i = 0; i < MR.TermsAr.Count; i++)
            {
                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    MR.TermsAr[i].Pos[j].indexElement = i;
            }
        }
        public void DelElementsWhichSetToDel(SynTerms MR)
        {
            int sizeMR = MR.TermsAr.Count;
            for (int i = 0; i < sizeMR; i++)
            {
                if (MR.TermsAr[i].setToDel)
                {
                    while (MR.TermsAr[i].Pos.Count > 0)
                    {
                        MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[0].range);
                        //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin());
                        //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        MR.TermsAr[i].Pos.RemoveAt(i);
                    }
                    //MR.TermsAr.erase(MR.TermsAr.begin() + i);
                    //vector<SynTerm>(MR.TermsAr).swap(MR.TermsAr);
                    MR.TermsAr.RemoveAt(i);
                    i--;
                    sizeMR = MR.TermsAr.Count;
                }
            }
            for (int i = 0; i < MR.TermsAr.Count; i++)
            {
                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    MR.TermsAr[i].Pos[j].indexElement = i;
            }
        }
        public Point GetRealPos(string fragment, string term, Point pos)
        {
            FindFunctions find = new FindFunctions();
            string low_term = term;
            low_term = low_term.ToLower();
            fragment = fragment.ToLower();
            //transform(low_term.begin(), low_term.end(), low_term.begin(), ::tolower);
            //transform(fragment.begin(), fragment.end(), fragment.begin(), ::tolower);
            int space = term.IndexOf(' ');
            if (space != -1)
            {
                string cur_word = term.Substring(0, space);
                string LargestFragment = find.GetLargestCommonSubstring(cur_word, fragment);
                int start = fragment.IndexOf(LargestFragment);
                if (start != -1)
                {
                    int number_spaces = find.num_spaces(low_term);
                    int end = start;
                    while (number_spaces > -1)
                    {
                        if (end + 1 < fragment.Length && fragment[end] == ' ' && fragment[end + 1] != ' ')
                        {
                            number_spaces--;
                        }
                        else if (end + 1 >= fragment.Length)
                            number_spaces--;
                        end++;
                    }
                    end--;
                    pos.X = pos.X + start - 1;
                    pos.Y = pos.X + end - 1;
                }
            }
            else
            {
                string LargestFragment = find.GetLargestCommonSubstring(low_term, fragment);
                int start = fragment.IndexOf(LargestFragment);
                if (start != -1)
                {
                    int number_spaces = 0;
                    int end = start;
                    while (number_spaces > -1)
                    {
                        if (end + 1 < fragment.Length && fragment[end] == ' ' && fragment[end + 1] != ' ')
                        {
                            number_spaces--;
                        }
                        else if (end + 1 >= fragment.Length)
                            number_spaces--;
                        end++;
                    }
                    end--;
                    pos.Y = pos.X + end - 1;
                    pos.X = pos.X + start - 1;
                }
            }
            return pos;
        }
    }
}

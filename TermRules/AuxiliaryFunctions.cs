using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace TermRules
{
    public class AuxiliaryFunctions
    {
        public List<string> ConstantPatterns;
        public AuxiliaryFunctions() 
        {
            ConstantPatterns = new List<string>();
            ConstantPatterns.Add("AP = A1 (A1) =text> A1 | Pa1 (Pa1) =text> Pa1");
            ConstantPatterns.Add("NE = \"не\" =text> \"не\"");
            //ConstantPatterns.Add("");
            //ConstantPatterns.Add("");
        }
        public void getFrequency_(Terms MR)
        {
            int size = MR.TermsAr.Count;
            bool change = true;
            while (change)
            {
                change = false;
                for (int i = 0; i < MR.TermsAr.Count; i++)
                {
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        Range range = new Range(MR.TermsAr[i].Pos[j].range.inf);
                        TermTree e = MR.rootTermsTree.FindRangeExtension(range);
                        if (e != null)
                        {
                            TermTree cur_treeEl = MR.TermsAr[i].Pos[j];
                            for (int k = 0; k < cur_treeEl.indexElement.Count; k++)
                            {
                                int ind_del_pos = MR.TermsAr[cur_treeEl.indexElement[k]].Pos.FindIndex(item => item == cur_treeEl);
                                if (ind_del_pos != -1)
                                    MR.TermsAr[cur_treeEl.indexElement[k]].Pos.RemoveAt(ind_del_pos);
                            }                                
                            MR.rootTermsTree.DeleteRange(range);
                            j--;
                        }
                    }
                    if (MR.TermsAr[i].Pos.Count == 0)
                    {
                        MR.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(MR, i);
                        i--;
                    }
                    else
                    {
                        MR.TermsAr[i].frequency = MR.TermsAr[i].Pos.Count;
                    }
                }
                //for (int i = 0; i < MR.TermsAr.Count; i++)
                //{
                //    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                //        if (!MR.TermsAr[i].Pos[j].indexElement.Contains(i)) MR.TermsAr[i].Pos[j].indexElement.Add(i);
                //}
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
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        Range range = new Range(MR.TermsAr[i].Pos[j].range.inf);
                        TermTree e = MR.rootTermsTree.FindRangeExtension(range);
                        if (e != null)
                        {
                            TermTree cur_treeEl = MR.TermsAr[i].Pos[j];
                            for (int k = 0; k < cur_treeEl.indexElement.Count; k++)
                            {
                                int ind_del_pos = MR.TermsAr[cur_treeEl.indexElement[k]].Pos.FindIndex(item => item == cur_treeEl);
                                if (ind_del_pos != -1)
                                    MR.TermsAr[cur_treeEl.indexElement[k]].Pos.RemoveAt(ind_del_pos);
                            }
                            MR.rootTermsTree.DeleteRange(range);
                            j--;
                        }
                    }
                    if (MR.TermsAr[i].Pos.Count == 0)
                    {
                        MR.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(MR, i);
                        i--;
                    }
                    else
                    {
                        MR.TermsAr[i].frequency = MR.TermsAr[i].Pos.Count;
                    }
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
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        Range range = new Range(MR.TermsAr[i].Pos[j].range.inf);
                        TermTree e = MR.rootTermsTree.FindRangeExtension(range);
                        if (e != null)
                        {
                            TermTree cur_treeEl = MR.TermsAr[i].Pos[j];
                            for (int k = 0; k < cur_treeEl.indexElement.Count; k++)
                            {
                                int ind_del_pos = MR.TermsAr[cur_treeEl.indexElement[k]].Pos.FindIndex(item => item == cur_treeEl);
                                if (ind_del_pos != -1)
                                    MR.TermsAr[cur_treeEl.indexElement[k]].Pos.RemoveAt(ind_del_pos);
                            }
                            MR.rootTermsTree.DeleteRange(range);
                            j--;
                        }
                    }
                    if (MR.TermsAr[i].Pos.Count == 0)
                    {
                        MR.TermsAr.RemoveAt(i);
                        ChangeIdexesInTree(MR, i);
                        i--;
                    }
                    else
                    {
                        MR.TermsAr[i].frequency = MR.TermsAr[i].Pos.Count;
                    }
                }
                //for (int i = 0; i < MR.TermsAr.Count; i++)
                //{
                //    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                //        if (!MR.TermsAr[i].Pos[j].indexElement.Contains(i)) MR.TermsAr[i].Pos[j].indexElement.Add(i);
                //}
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
                    for (int j=0; j<MR.TermsAr[i].Pos.Count; j++)
                    {
                        int ind_to_del = MR.TermsAr[i].Pos[j].indexElement.FindIndex(item => item == i);
                        if (ind_to_del != -1)
                        {
                            MR.TermsAr[i].Pos[j].indexElement.RemoveAt(ind_to_del);
                        }
                        if (MR.TermsAr[i].Pos[j].indexElement.Count == 0)
                        {
                            Range del_range = new Range(MR.TermsAr[i].Pos[j].range.inf);
                            MR.rootTermsTree.DeleteRange(del_range);
                        }
                    }
                    MR.TermsAr.RemoveAt(i);
                    ChangeIdexesInTree(MR, i);
                    sizeMR = MR.TermsAr.Count;
                    i--;
                }
            }
            //for (int i = 0; i < MR.TermsAr.Count; i++)
            //{
            //    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
            //        if (!MR.TermsAr[i].Pos[j].indexElement.Contains(i)) MR.TermsAr[i].Pos[j].indexElement.Add(i);
            //}
        }
        public void DelElementsWhichSetToDel(SynTerms MR)
        {
            int sizeMR = MR.TermsAr.Count;
            for (int i = 0; i < sizeMR; i++)
            {
                if (MR.TermsAr[i].setToDel)
                {
                    for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                    {
                        int ind_to_del = MR.TermsAr[i].Pos[j].indexElement.FindIndex(item => item == i);
                        if (ind_to_del != -1)
                        {
                            MR.TermsAr[i].Pos[j].indexElement.RemoveAt(ind_to_del);
                        }
                        if (MR.TermsAr[i].Pos[j].indexElement.Count == 0)
                        {
                            Range del_range = new Range(MR.TermsAr[i].Pos[j].range.inf);
                            MR.rootTermsTree.DeleteRange(del_range);
                        }
                    }
                    MR.TermsAr.RemoveAt(i);
                    ChangeIdexesInTree(MR, i);
                    sizeMR = MR.TermsAr.Count;
                    i--;
                }
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
        public string NormalizeNPattern(string NPattern)
        {
            bool change = true;
            int open_t = -1;
            int close_t = 0;
            //int prev_close_t = 0;
            string newNP = "";
            while (change)
            {
                change = false;
                open_t = NPattern.IndexOf("<", open_t + 1);
                //List<string> splitPattern = new List<string>(CombTermsAr.TermsAr[i].Components[j].NPattern.Split("<>".ToCharArray()));
                if (open_t != -1 &&
                    (NPattern[open_t - 1] >= '0' &&
                     NPattern[open_t - 1] <= '9'))
                {
                    newNP = newNP + NPattern.Substring(close_t, open_t - close_t + 1);
                    //prev_close_t = close_t;
                    close_t = NPattern.IndexOf(">", open_t + 1);
                    newNP = newNP + NPattern.Substring(open_t + 1, close_t - (open_t + 1)).Trim().ToLower().Replace(" ", "");
                    change = true;
                }
                else
                {
                    int p = open_t;
                    while (p != -1) // Костыль, можно изящнее
                    {
                        p = NPattern.IndexOf("<", p + 1);
                        if (p != -1 &&
                            (NPattern[p - 1] >= '0' &&
                             NPattern[p - 1] <= '9'))
                            break;
                    }
                    if (p != -1)
                    {
                        open_t = p;
                        newNP = newNP + NPattern.Substring(close_t, open_t - close_t + 1);
                        //prev_close_t = close_t;
                        close_t = NPattern.IndexOf(">", open_t + 1);
                        newNP = newNP + NPattern.Substring(open_t + 1, close_t - (open_t + 1)).Trim().ToLower().Replace(" ", "");
                        change = true;
                    }
                    else
                    {
                        newNP = newNP + NPattern.Substring(close_t);
                    }

                }

            }
            return newNP.Replace("=TEXT>", "=text>");
        }
        public void PrintConstantPatterns(StreamWriter sw)
        {
            foreach (string str in ConstantPatterns)
                sw.WriteLine(str);
        }
        public void ChangeIdexesInTree(Terms MR, int del_el) // Костыль, по возможности убрать
        {
            List<TermTree> Changed = new List<TermTree>();
            for (int i=0; i<MR.TermsAr.Count; i++)
            {
                for (int j=0; j<MR.TermsAr[i].Pos.Count; j++)
                {
                    if (!Changed.Contains(MR.TermsAr[i].Pos[j]))
                    {
                        for (int k = 0; k < MR.TermsAr[i].Pos[j].indexElement.Count; k++)
                        {
                            if (MR.TermsAr[i].Pos[j].indexElement[k] > del_el)
                            {
                                MR.TermsAr[i].Pos[j].indexElement[k]--;
                            }
                        }
                        Changed.Add(MR.TermsAr[i].Pos[j]);
                    }
                }
            }
        }
        public void ChangeIdexesInTree(SynTerms MR, int del_el) // Костыль, по возможности убрать
        {
            List<TermTree> Changed = new List<TermTree>();
            for (int i = 0; i < MR.TermsAr.Count; i++)
            {
                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                {
                    if (!Changed.Contains(MR.TermsAr[i].Pos[j]))
                    {
                        for (int k = 0; k < MR.TermsAr[i].Pos[j].indexElement.Count; k++)
                        {
                            if (MR.TermsAr[i].Pos[j].indexElement[k] > del_el)
                            {
                                MR.TermsAr[i].Pos[j].indexElement[k]--;
                            }
                        }
                        Changed.Add(MR.TermsAr[i].Pos[j]);
                    }
                }
            }
        }
        public void ChangeIdexesInTree(NonDictTerms MR, int del_el) // Костыль, по возможности убрать
        {
            List<TermTree> Changed = new List<TermTree>();
            for (int i = 0; i < MR.TermsAr.Count; i++)
            {
                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                {
                    if (!Changed.Contains(MR.TermsAr[i].Pos[j]))
                    {
                        for (int k = 0; k < MR.TermsAr[i].Pos[j].indexElement.Count; k++)
                        {
                            if (MR.TermsAr[i].Pos[j].indexElement[k] > del_el)
                            {
                                MR.TermsAr[i].Pos[j].indexElement[k]--;                                
                            }
                        }
                        Changed.Add(MR.TermsAr[i].Pos[j]);
                    }
                }
            }
        }
        public void ChangeIdexesInTree(CombTerms MR, int del_el) // Костыль, по возможности убрать
        {
            List<TermTree> Changed = new List<TermTree>();
            for (int i = 0; i < MR.TermsAr.Count; i++)
            {
                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
                {
                    if (!Changed.Contains(MR.TermsAr[i].Pos[j]))
                    {
                        for (int k = 0; k < MR.TermsAr[i].Pos[j].indexElement.Count; k++)
                        {
                            if (MR.TermsAr[i].Pos[j].indexElement[k] > del_el)
                            {
                                MR.TermsAr[i].Pos[j].indexElement[k]--;
                            }
                        }
                        Changed.Add(MR.TermsAr[i].Pos[j]);
                    }
                }
            }
        }
    }
}




//while (MR.TermsAr[i].Pos.Count > 0)
//                    {
//                        for (int k = 0; k < MR.TermsAr[i].Pos[0].indexElement.Count; k++)
//                            if (MR.TermsAr[i].Pos[0].indexElement[k] != i)
//                            {
//                                for (int p=0; p<MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.Count; p++)
//                                {
//                                    if (MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos[p] == MR.TermsAr[i].Pos[0])
//                                    {
//                                        MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.RemoveAt(p);
//                                        p--;
//                                    }
//                                }
//                            }
//                        MR.TermsAr[i].Pos.RemoveAt(0);
//                        MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[0].range);
//                        //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin() );
//                        //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);                        
//                    }
//                    //MR.TermsAr.erase(MR.TermsAr.begin() + i);
//                    //vector<Term>(MR.TermsAr).swap(MR.TermsAr);
//                    MR.TermsAr.RemoveAt(i);
//                    i--;
//                    sizeMR = MR.TermsAr.Count;

//int sizeMR = MR.TermsAr.Count;
//            for (int i = 0; i < sizeMR; i++)
//            {
//                if (MR.TermsAr[i].setToDel)
//                {
//                    while (MR.TermsAr[i].Pos.Count > 0)
//                    {
//                        for (int k = 0; k < MR.TermsAr[i].Pos[0].indexElement.Count; k++)
//                            if (MR.TermsAr[i].Pos[0].indexElement[k] != i)
//                            {
//                                for (int p = 0; p < MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.Count; p++)
//                                {
//                                    if (MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos[p] == MR.TermsAr[i].Pos[0])
//                                    {
//                                        MR.TermsAr[MR.TermsAr[i].Pos[0].indexElement[k]].Pos.RemoveAt(p);
//                                        p--;
//                                    }
//                                }
//                            }
//                        MR.TermsAr[i].Pos.RemoveAt(0);
//                        MR.rootTermsTree.DeleteRange(MR.TermsAr[i].Pos[0].range);
//                        //MR.TermsAr[i].Pos.erase(MR.TermsAr[i].Pos.begin());
//                        //vector<TermTree*>(MR.TermsAr[i].Pos).swap(MR.TermsAr[i].Pos);
                        
//                    }
//                    //MR.TermsAr.erase(MR.TermsAr.begin() + i);
//                    //vector<SynTerm>(MR.TermsAr).swap(MR.TermsAr);
//                    MR.TermsAr.RemoveAt(i);
//                    i--;
//                    sizeMR = MR.TermsAr.Count;
//                }
//            }
//            for (int i = 0; i < MR.TermsAr.Count; i++)
//            {
//                for (int j = 0; j < MR.TermsAr[i].Pos.Count; j++)
//                    if (!MR.TermsAr[i].Pos[j].indexElement.Contains(i)) MR.TermsAr[i].Pos[j].indexElement.Add(i);
//            }
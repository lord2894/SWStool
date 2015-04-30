using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CheckPatterns
{
    class Program
    {
        //public List<string> PatternLines;
        public class pair<T, U>
        {
            public pair() { }
            public pair(T first, U second)
            {
                this.first = first;
                this.second = second;
            }
            public T first { get; set; }
            public U second { get; set; }
            //public T first;
            //public U second;
        }
        public void checkS(List<string> PatternLines)
        {
            List<int> wrongPatterns = new List<int>();
            for (int i = 0; i < PatternLines.Count; i++)
            {
                if (PatternLines[i] != "")
                {
                    if (PatternLines[i].IndexOf("=text>") == -1)
                    {
                        //pair<string, int> new_p = new pair<string, int>();
                        //new_p.first = PatternLines[i];
                        //new_p.second = i+1;
                        //wrongPatterns.Add(new_p);
                        //wrongPatterns.Add(i + 1);
                    }
                    else
                    {
                        //string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
                        string SecondPart = PatternLines[i];
                        int count_o = 0;
                        int count_c = 0;
                        int ind = -1;
                        while (true)
                        {
                            ind = SecondPart.IndexOf("<", ind + 1);
                            if (ind == -1) break;
                            else count_o++;
                        }
                        ind = -1;
                        while (true)
                        {
                            ind = SecondPart.IndexOf(">", ind + 1);
                            if (ind == -1) break;
                            else if (SecondPart[ind - 1] != '~') count_c++;
                        }
                        if (count_o != count_c - 1)
                            wrongPatterns.Add(i + 1);
                        //if (SecondPart.ind.IndexOf("с=") != -1)
                        //    wrongPatterns.Add(SecondPart);
                    }
                }
            }
        }
        public void checkK(List<string> PatternLines)
        {
            List<int> wrongPatterns = new List<int>();
            for (int i = 0; i < PatternLines.Count; i++)
            {
                if (PatternLines[i] != "")
                {

                    //string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
                    string SecondPart = PatternLines[i];
                    int count_s = 0;
                    int ind = -1;
                    while (true)
                    {
                        ind = SecondPart.IndexOf("\"", ind + 1);
                        if (ind == -1) break;
                        else count_s++;
                    }
                    if (count_s % 2 != 0)
                        wrongPatterns.Add(i + 1);
                    //if (SecondPart.ind.IndexOf("с=") != -1)
                    //    wrongPatterns.Add(SecondPart);
                }
            }
        }
        public void checkP(List<string> PatternLines)
        {
            List<int> wrongPatterns = new List<int>();
            for (int i = 0; i < PatternLines.Count; i++)
            {
                if (PatternLines[i] != "")
                {
                    if (PatternLines[i].IndexOf("=text>") != -1)
                    {
                        string firstPart = PatternLines[i].Substring(0, PatternLines[i].IndexOf("=text>"));
                        string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
                        if (firstPart.IndexOf("N") != -1 && SecondPart.IndexOf("N") == -1)
                            wrongPatterns.Add(i + 1);
                        if (firstPart.IndexOf("N") == -1 && SecondPart.IndexOf("N") != -1)
                            wrongPatterns.Add(i + 1);
                        if (firstPart.IndexOf("A") != -1 && SecondPart.IndexOf("A") == -1)
                            wrongPatterns.Add(i + 1);
                        if (firstPart.IndexOf("A") == -1 && SecondPart.IndexOf("A") != -1)
                            wrongPatterns.Add(i + 1);
                    }
                }
            }
        }
        public void checkThroughLSPLUtilite(List<string> PatternLines, string PatternsFile)
        {
            int count = PatternLines.Count;
            int ind = 0;
            bool rewrite = false;
            while (count != 0)
            {
                if (count > 50)
                {
                    count = count - 50;
                    StreamWriter sw = new StreamWriter(PatternsFile, rewrite, Encoding.GetEncoding("Windows-1251"));
                    for (int i = 0; i < 50; i++)
                    {
                        sw.WriteLine(PatternLines[ind + i]);
                    }
                    rewrite = true;
                    sw.Close();
                    ind = ind + 50;
                }
                else
                {
                    count = 0;
                    StreamWriter sw = new StreamWriter(PatternsFile, true, Encoding.GetEncoding("Windows-1251"));
                    for (int i = 0; i < PatternLines.Count - ind; i++)
                    {
                        sw.WriteLine(PatternLines[ind + i]);
                    }
                    sw.Close();
                }
            }
        }
        public void SetName(List<string> PatternLines)
        {
            StreamWriter sw = new StreamWriter("TERM_F3.txt", false, Encoding.GetEncoding("Windows-1251"));
            string priviousWord = "";
            int cur_lat = 65;
            foreach (string current_pat in PatternLines)
            {
                string pat = current_pat.Trim();
                if (pat != "")
                {
                    int ind_s = pat.IndexOf("<");
                    int ind_k = pat.IndexOf("\"");
                    if (ind_s != -1 && ind_k != -1)
                    {
                        if (ind_s < ind_k)
                        {
                            int ind_s_c = pat.IndexOf(">", ind_s + 1);
                            int ind_s_z = pat.IndexOf(",", ind_s + 1);
                            if ((ind_s_z != -1 && ind_s_c < ind_s_z) || (ind_s_z == -1))
                            {
                                string cur_pat = pat.Substring(ind_s + 1, ind_s_c - (ind_s + 1));
                                if (cur_pat == priviousWord)
                                {
                                    cur_lat++;
                                    sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = cur_pat;
                                }
                                else
                                {
                                    cur_lat = 65;
                                    sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = cur_pat;
                                }
                            }
                            else if (ind_s_z != -1 && ind_s_z < ind_s_c)
                            {
                                string cur_pat = pat.Substring(ind_s + 1, ind_s_z - (ind_s + 1));
                                if (cur_pat == priviousWord)
                                {
                                    cur_lat++;
                                    sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = cur_pat;
                                }
                                else
                                {
                                    cur_lat = 65;
                                    sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                    priviousWord = cur_pat;
                                }
                            }
                        }
                        else
                        {
                            int ind_k_c = pat.IndexOf("\"", ind_k + 1);
                            string cur_pat = pat.Substring(ind_k + 1, ind_k_c - (ind_k + 1));

                            if (cur_pat == priviousWord)
                            {
                                cur_lat++;
                                sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                priviousWord = cur_pat;
                            }
                            else
                            {
                                cur_lat = 65;
                                sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                priviousWord = cur_pat;
                            }
                        }
                    }
                    else if (ind_s != -1)
                    {
                        int ind_s_c = pat.IndexOf(">", ind_s + 1);
                        int ind_s_z = pat.IndexOf(",", ind_s + 1);
                        if ((ind_s_z != -1 && ind_s_c < ind_s_z) || (ind_s_z == -1))
                        {
                            string cur_pat = pat.Substring(ind_s + 1, ind_s_c - (ind_s + 1));
                            if (cur_pat == priviousWord)
                            {
                                cur_lat++;
                                sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                priviousWord = cur_pat;
                            }
                            else
                            {
                                cur_lat = 65;
                                sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                priviousWord = cur_pat;
                            }
                        }
                        else if (ind_s_z != -1 && ind_s_z < ind_s_c)
                        {
                            string cur_pat = pat.Substring(ind_s + 1, ind_s_z - (ind_s + 1));
                            if (cur_pat == priviousWord)
                            {
                                cur_lat++;
                                sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                priviousWord = cur_pat;
                            }
                            else
                            {
                                cur_lat = 65;
                                sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                                priviousWord = cur_pat;
                            }
                        }
                    }
                    else if (ind_k != -1)
                    {
                        int ind_k_c = pat.IndexOf("\"", ind_k + 1);
                        string cur_pat = pat.Substring(ind_k + 1, ind_k_c - (ind_k + 1));

                        if (cur_pat == priviousWord)
                        {
                            cur_lat++;
                            sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                            priviousWord = cur_pat;
                        }
                        else
                        {
                            cur_lat = 65;
                            sw.WriteLine(cur_pat + "-" + ((char)cur_lat).ToString().ToUpper() + " = " + pat);
                            priviousWord = cur_pat;
                        }
                    }
                }
                else
                {
                    sw.WriteLine("\n");
                }
            }
            sw.Close();
        }
        static void Main(string[] args)
        {
            Program pr = new Program();
            List<string> PatternLines = new List<string>();
            //string programmPath = Application.StartupPath.ToString();
            string programmPath = "C:\\Users\\Kir\\Documents\\Visual Studio 2013\\Projects\\TermsStrategy\\Debug\\Patterns";
            string PatternsFile = programmPath + "\\TERM_F2.txt";
            string curPattern = "";
            List<int> wrongPatterns = new List<int>();
            //List<string> wrongPatterns = new List<string>();
            StreamReader fs = new StreamReader(PatternsFile, Encoding.GetEncoding("Windows-1251"));

            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                PatternLines.Add(curPattern);
            }
            fs.Close();
            pr.SetName(PatternLines);
        }
    }
}









//for (int i = 0; i < PatternLines.Count; i++)
//            {
//                if (PatternLines[i] != "")
//                {
//                    if (PatternLines[i].IndexOf("=text>") != -1)
//                    {
//                        string firstPart = PatternLines[i].Substring(0, PatternLines[i].IndexOf("=text>"));
//                        string SecondPart = PatternLines[i].Substring(PatternLines[i].IndexOf("=text>") + "=text>".Length);
//                        if (firstPart.IndexOf("N") != -1 && SecondPart.IndexOf("N") == -1)
//                            wrongPatterns.Add(i + 1);
//                        if (firstPart.IndexOf("N") == -1 && SecondPart.IndexOf("N") != -1)
//                            wrongPatterns.Add(i + 1);
//                        if (firstPart.IndexOf("A") != -1 && SecondPart.IndexOf("A") == -1)
//                            wrongPatterns.Add(i + 1);
//                        if (firstPart.IndexOf("A") == -1 && SecondPart.IndexOf("A") != -1)
//                            wrongPatterns.Add(i + 1);
//                    }                   
//                }
//            }
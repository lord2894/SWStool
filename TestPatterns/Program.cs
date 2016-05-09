using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace TestPatterns
{
    class Program
    {
        public static int findINList(List<pair<string, string>> v, string str, int alt)
        {
            if (v == null || str == null) return -1;
            for (int i = 0; i < v.Count; i++)
            {
                if ((alt == 1 && v[i].first == str) || (alt == 2 && v[i].second == str))
                    return i;
            }
            return -1;
        }
        public class pair<T, U>
        {
            public pair(T first, U second)
            {
                this.first = first;
                this.second = second;
            }
            public pair() { }
            public T first { get; set; }
            public U second { get; set; }
        }
        static void Main(string[] args)
        {
            TestAuthPatterns();
        }

        public static void TestAuthPatterns()
        {
            List<pair<string, string>> PatternsModel = new List<pair<string, string>>();
            string programmPath = System.Windows.Forms.Application.StartupPath.ToString();
            string BAT_output = programmPath + "\\AuthTerms.bat";
            string patternsName = "";
            string curPattern = "";
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\AUTH_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPattern = curPattern.Trim();
                if (curPattern.IndexOf("DefIns") == -1 && curPattern.IndexOf("DefIns") == -1)
                {
                    int len = 0;
                    if (curPattern.IndexOf("Def") != -1 && curPattern.Length > 3)
                    {
                        len = curPattern.IndexOf("Def") + "Def".Length;
                    }
                    if (len > 0)
                    {
                        int k = findINList(PatternsModel, curPattern, 1);
                        if (k == -1)
                        {
                            pair<string, string> new_p = new pair<string, string>();
                            patternsName = patternsName + " " + curPattern;
                            new_p.first = curPattern;
                            new_p.second = curPattern.Substring(len).Trim();
                            PatternsModel.Add(new_p);
                        }
                        len = 0;
                    }
                }
                while (true)
                {
                    curPattern = fs.ReadLine();
                    if (curPattern == null) break;
                    if (curPattern != "")
                    {
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPattern = curPattern.Trim();
                        if (curPattern.IndexOf("DefIns") == -1 && curPattern.IndexOf("DefXXX") == -1)
                        {
                            int len = 0;
                            if (curPattern.IndexOf("Def") != -1 && curPattern.Length > 3)
                            {
                                len = curPattern.IndexOf("Def") + "Def".Length;
                            }
                            if (len > 0)
                            {
                                int k = findINList(PatternsModel, curPattern, 1);
                                if (k == -1)
                                {
                                    pair<string, string> new_p = new pair<string, string>();
                                    patternsName = patternsName + " " + curPattern;
                                    new_p.first = curPattern;
                                    new_p.second = curPattern.Substring(len).Trim();
                                    PatternsModel.Add(new_p);
                                }
                                len = 0;
                            }
                        }
                    }
                }
                string inputFile = programmPath + "\\inputText.txt";
                string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
                string LSPL_patterns = programmPath + "\\Patterns\\AUTH_TERM.txt";
                string LSPL_output = programmPath + "\\AuthTermsOutput.xml";
                StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patternsName);
                sw.Close();
                //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                //GetAuthTerms(AuthTermsAr);

            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами авторских терминов!");
                System.Windows.Forms.Application.Exit();
            }
        }
        public static void TestNonDictPatterns()
        {
            List<pair<string, string>> PatternsModel = new List<pair<string, string>>();
            string programmPath = System.Windows.Forms.Application.StartupPath.ToString();
            string LSPL_patterns = programmPath + "\\Patterns\\NONDICT_TERM.txt";
            StreamReader fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding("Windows-1251"));
            string patternsName = "";
            string curPattern = "";
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                if (curPattern != "")
                {
                    curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    int len = 0;
                    switch (curPattern[0])
                    {
                        case 'F':
                            {
                                len = "F".Length;
                                break;
                            }
                        case 'C':
                            {
                                len = "Ca".Length;
                                break;
                            }
                        case 'N':
                            {
                                if (curPattern.IndexOf("F") != -1) len = "NPF".Length;
                                else len = "NPCa".Length;
                                break;
                            }
                    }
                    int k = findINList(PatternsModel, curPattern, 1);
                    if (k == -1 && len != 0)
                    {
                        pair<string, string> new_p = new pair<string, string>();
                        patternsName = patternsName + " " + curPattern.Trim();
                        new_p.first = curPattern;
                        new_p.second = curPattern.Substring(len).Trim();
                        PatternsModel.Add(new_p);
                    }
                }
            }
            //--------------------------------
            string inputFile = programmPath + "\\inputText.txt";
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_output = programmPath + "\\NontDictTermsOutput.xml";
            string BAT_output = programmPath  + "\\NontDictTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patternsName);
            //Close the file
            sw.Close();
            //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            //GetNonDictTerms(NonDictTermsAr);
            //---------------------------------
            return;
        }
        public static void TestSynPatterns()
        {
            List<pair<string, string>> PatternsModel = new List<pair<string, string>>();
            string programmPath = System.Windows.Forms.Application.StartupPath.ToString();
            string LSPL_patterns = programmPath + "\\Patterns\\SYN_TERM.txt";
            string patternsName = "";
            string curPattern = "";
            StreamReader fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                if (curPattern != "")
                {
                    curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    if (curPattern.IndexOf("SYN") != -1)
                    {
                        int len = curPattern.IndexOf("SYN") + "SYN".Length;
                        int k = findINList(PatternsModel, curPattern, 1);
                        if (k == -1)
                        {
                            pair<string, string> new_p = new pair<string, string>();
                            patternsName = patternsName + " " + curPattern;
                            new_p.first = curPattern;
                            new_p.second = curPattern.Substring(len).Trim();
                            PatternsModel.Add(new_p);
                        }
                        len = 0;
                    }
                }
            }
            //--------------------------------
            string inputFile = programmPath + "\\inputText.txt";
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_output = programmPath + "\\SynTermsOutput.xml";
            string BAT_output = programmPath + "\\SynTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patternsName);
            //Close the file
            sw.Close();
            //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            //GetSynTerms(SynTermsAr);
            //---------------------------------
            return;
        }
        public static void TestDictPatterns()
        {
            List<pair<string, string>> DictPatterns = new List<pair<string, string>>();
            string programmPath = System.Windows.Forms.Application.StartupPath.ToString();
            string LSPL_patterns = "";
            StreamReader fs = null;
            LSPL_patterns = programmPath + "\\Patterns\\F_TERM.txt";
            fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding(1251));
            string patternsName = "";
            string curPattern = "";
            string inputFile = programmPath + "\\inputText.txt";
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_output = programmPath + "\\DictTermsOutput.xml";
            string BAT_output = programmPath + "\\DictTerms.bat";
            string BAT_command = "\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" ";
            bool callUtilit = false;
            curPattern = fs.ReadLine();
            if (curPattern != "")
            {
                curPattern = curPattern.Trim();
                patternsName = patternsName + " " + curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                pair<string, string> cur_pat = new pair<string, string>();
                cur_pat.first = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                cur_pat.second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                DictPatterns.Add(cur_pat);
            }
            //string prevPattern = curPattern.Substring(0, curPattern.IndexOf('='));
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                //if (curPattern != "" && curPattern.Substring(0, curPattern.IndexOf('=')) != prevPattern)
                if (curPattern != "")
                {
                    string curPatternName = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    if (BAT_command.Length + patternsName.Length + curPatternName.Length < 8000)
                    {
                        patternsName = patternsName + " " + curPatternName;
                        pair<string, string> cur_pat = new pair<string, string>();
                        cur_pat.first = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        cur_pat.second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                        DictPatterns.Add(cur_pat);
                        callUtilit = false;
                    }
                    else
                    {
                        StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                        sw.WriteLine(BAT_command + patternsName);
                        sw.Close();
                        //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                        //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                       // GetDictTerms(DictTermsAr);
                        patternsName = curPatternName;
                        pair<string, string> cur_pat = new pair<string, string>();
                        cur_pat.first = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        cur_pat.second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                        DictPatterns.Add(cur_pat);
                        callUtilit = true;
                    }
                }
            }
            if (!callUtilit)
            {
                StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine(BAT_command + patternsName);
                sw.Close();
                //ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                //GetDictTerms(DictTermsAr);
            }
            /*switch (dictionary)
            //{
            //    case DictionaryF.IT_TERM:
            //        {
            //            LSPL_patterns = programmPath + "\\Patterns\\IT_TERMNP.txt";
            //            fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding("Windows-1251"));
            //            break;
            //        }
            //    case DictionaryF.F_TERM:
            //        {
            //            LSPL_patterns = programmPath + "\\Patterns\\F_TERMNP.txt";
            //            fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding("Windows-1251"));
            //            break;
            //        }
            //}
            //patternsName = "";
            //curPattern = "";            
            //while (true)
            //{
            //    curPattern = fs.ReadLine();
            //    if (curPattern == null) break;
            //    //if (curPattern != "" && curPattern.Substring(0, curPattern.IndexOf('=')) != prevPattern)
            //    if (curPattern != "")
            //    {
            //        int k = FindFunctions.findINList(DictPatterns, curPattern.Substring("NP".Length, curPattern.IndexOf('=') - "NP".Length), 1);
            //        DictPatterns[k].second = curPattern.Substring(curPattern.IndexOf('=')); 
            //    }
            //}
            //--------------------------------*/
            return;
        }
    }
}

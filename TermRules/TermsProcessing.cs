﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
using TermsNamespace;
using FindFunctionsNamespace;
using TermTreeNamespace;
using AuxiliaryFunctionsNamespace;
using System.Diagnostics;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;

namespace TermProcessingNamespace
{

    public enum DictionaryF
    {
        IT_TERM,
        F_TERM
    }
    public class TermsProcessing
    {
        //private const int MAX_COMMAND = 8000;
        public Terms MainTermsAr { get; set; }
        public Terms AuthTermsAr { get; set; }
        public Terms DictTermsAr { get; set; }
        public NonDictTerms NonDictTermsAr { get; set; }
        public CombTerms CombTermsAr { get; set; }
        public List<pair<string, string>> PatternsModel { get; set; }
        public List<pair<string, string>> DictPatterns { get; set; }
        public string tmpPath { get; set; }
        public string programmPath { get; set; }
        public string folderPath { get; set; }
        public string inputFile { get; set; }
        public string outputFile { get; set; }
        public int startPage { get; set; }
        public int endPage { get; set; }
        public List<string> Pages { get; set; }

        DictionaryF dictionary;
        public TermsProcessing(string inputfile, DictionaryF dict, int startPage, int endPage, bool ClearSupportLists)
        {
            folderPath = "TermsProcessingF";
            outputFile = "";
            inputFile = inputfile;
            string extension = Path.GetExtension(inputfile).ToLower();
            //string pattern = ".\u002E[dD][oO][cC][xX]$";
            //if (Regex.IsMatch(inputFile, pattern))
            if (extension == ".docx")
            {
                this.startPage = startPage;
                this.endPage = endPage;
                Pages = ReadTextFromDocxByPage(inputfile);
                dictionary = dict;
                tmpPath = Path.GetTempPath();
                programmPath = System.Windows.Forms.Application.StartupPath.ToString();
                MainTermsAr = new Terms();
                AuthTermsAr = new Terms();
                DictTermsAr = new Terms();
                NonDictTermsAr = new NonDictTerms();
                CombTermsAr = new CombTerms();
                Directory.CreateDirectory(tmpPath + folderPath);
                inputFile = tmpPath + folderPath + "\\InputText.txt";
                StreamWriter outfile = new StreamWriter(File.Open(inputFile, FileMode.Create), Encoding.GetEncoding("Windows-1251"));
                foreach (string page in Pages)
                {
                    outfile.Write(CleanString(page));
                }
                outfile.Close();
                PatternsModel = new List<pair<string, string>>();
                DictPatterns = new List<pair<string, string>>();
            }
            else
            {
                MessageBox.Show("ERROR: Неверный формат файла, поддерживается только *.docx");
                System.Windows.Forms.Application.Exit();
            }
        }
        static public string CleanString(string s)
        {
            if (s != null && s.Length > 0)
            {
                StringBuilder sb = new StringBuilder(s.Length);
                foreach (char c in s)
                {
                    if (c!='\x0A' && c!='\n' && Char.IsControl(c) == true)
                    {
                        if (c == '\x1E')
                            sb.Append('-');
                        else sb.Append(' ');
                        continue;
                    }
                    if (c == 'ё')
                        sb.Append('е');
                    else
                        sb.Append(c);
                }
                s = sb.ToString().Replace("/"," / ").Replace("|"," | ").Replace("\\"," \\ ");
            }
            return s;
        }

        //AuthTerms
        public void GetXMLAuthTerms(Terms AuthTermsAr)
        {
            string BAT_output = tmpPath + folderPath + "\\AuthTerms.bat";
            string TargetPatterns_output = tmpPath + folderPath + "\\targets";
            StreamWriter TargetPatternsWriter = new StreamWriter(TargetPatterns_output, false, Encoding.GetEncoding("Windows-1251"));
            string curPattern = "";
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\AUTH_TERM_NEW.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPattern = curPattern.Trim();
                if (curPattern.IndexOf("Def") != -1)
                {
                    TargetPatternsWriter.WriteLine(curPattern);
                }
                while (true)
                {
                    curPattern = fs.ReadLine();
                    if (curPattern == null) break;
                    if (curPattern != "")
                    {
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPattern = curPattern.Trim();

                        if (curPattern.IndexOf("Def") != -1 && curPattern.Length > 3)
                        {
                            TargetPatternsWriter.WriteLine(curPattern);
                        }
                    }
                }
                TargetPatternsWriter.Close();
                string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
                string LSPL_patterns = programmPath + "\\Patterns\\AUTH_TERM_NEW.txt";
                string LSPL_output = tmpPath + folderPath + "\\AuthTermsOutput.xml";
                string LSPL_output_text = tmpPath + folderPath + "\\AuthTermsOutputText.xml";
                //string LSPL_output_patterns = tmpPath + folderPath + "\\AuthTermsOutputPatterns.xml";
                StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                //bin\lspl-find -i example\text.txt -p example\patterns.txt -o example\output.xml -t example\output_text.xml -r example\output_patterns.xml -s example\targets.txt
                sw.WriteLine("\"" + LSPL_exe 
                    + "\" -i \"" + inputFile 
                    + "\" -p \"" + LSPL_patterns 
                    //+ "\" -o \"" + LSPL_output
                    + "\" -t \"" + LSPL_output_text
                    //+ "\" -r \"" + LSPL_output_patterns
                    + "\" -s \"" + TargetPatterns_output + "\" ");
                sw.Close();
                ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                GetAuthTerms(AuthTermsAr);

            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами авторских терминов!");
                System.Windows.Forms.Application.Exit();
            }
            return;
        }
        public bool GetAuthTerms(Terms AuthTermsAr)
        {
            System.Drawing.Point cur_pos = new System.Drawing.Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string lastPOS = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + folderPath + "\\AuthTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                cur_pat = xml.Value.Trim();
                                                cur_pat = cur_pat.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                cur_pos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPOS = xml.Value.Trim();
                                        }
                                    }
                                    
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    cur_fragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat.IndexOf("Def") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        //int k = FindFunctions.findINList(AuthTermsAr.TermsAr, word, lastPOS);
                                        int k = AuthTermsAr.TermsAr.FindIndex(item => item.TermWord == word && item.POSstr == lastPOS);
                                        if (k == -1)
                                        {
                                            cur_pos = AuxiliaryFunctions.GetRealPos(cur_fragment, word, cur_pos);
                                            TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            TermTree e_extension = AuthTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                            Term newEl = new Term();
                                            newEl.TermWord = word;
                                            newEl.frequency = 1;
                                            newEl.POSstr = lastPOS;
                                            newEl.setToDel = false;
                                            newEl.Pos.Add(null);
                                            newEl.TermFragment = cur_fragment;
                                            newEl.Pattern = cur_pat.Substring("Def".Length).Trim();
                                            newEl.kind = KindOfTerm.AuthTerm;
                                            if (e == null && e_extension == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                AuthTermsAr.TermsAr.Add(newEl);
                                                e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                            else
                                            {
                                                if (e != null && e_extension == null)
                                                {
                                                    AuthTermsAr.TermsAr.Add(newEl);
                                                    e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                    AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            cur_pos = AuxiliaryFunctions.GetRealPos(cur_fragment, word, cur_pos);
                                            TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            TermTree e_extension = AuthTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                            if (e == null && e_extension == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(k);
                                                AuthTermsAr.TermsAr[k].Pos.Add(e);
                                                AuthTermsAr.TermsAr[k].frequency++;
                                            }
                                            else
                                            {
                                                if (e != null && !e.indexElement.Contains(k))
                                                    e.indexElement.Add(k);
                                            }
                                        }
                                    }
                                    else if (cur_pat.IndexOf("SDef") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        //int k = FindFunctions.findINList(AuthTermsAr.TermsAr, word, lastPOS);
                                        int k = AuthTermsAr.TermsAr.FindIndex(item => item.TermWord == word && item.POSstr == lastPOS);
                                        if (k == -1)
                                        {
                                            TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            TermTree e_extension = AuthTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                            Term newEl = new Term();
                                            newEl.TermWord = word;
                                            newEl.POSstr = lastPOS;
                                            newEl.frequency = 1;
                                            newEl.setToDel = false;
                                            newEl.Pos.Add(null);
                                            newEl.TermFragment = cur_fragment;
                                            newEl.Pattern = cur_pat.Substring("SDef".Length).Trim();
                                            newEl.kind = KindOfTerm.AuthTerm;
                                            if (e == null && e_extension == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                AuthTermsAr.TermsAr.Add(newEl);
                                                e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                            else
                                            {
                                                if (e != null && e_extension == null)
                                                {
                                                    AuthTermsAr.TermsAr.Add(newEl);
                                                    e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                    AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            TermTree e_extension = AuthTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                            if (e == null && e_extension == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(k);
                                                AuthTermsAr.TermsAr[k].Pos.Add(e);
                                                AuthTermsAr.TermsAr[k].frequency++;
                                            }
                                            else
                                            {
                                                if (e!=null && !e.indexElement.Contains(k))
                                                    e.indexElement.Add(k);
                                            }
                                        }
                                    }
                                    else if (cur_pat.IndexOf("NPDef") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        
                                        //Range cur_range = new Range(cur_pos);
                                        //TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                        //if (e != null)
                                        //{
                                        //    //int k = e.indexElement;
                                        //    int k = FindFunctions.findPattern(AuthTermsAr.TermsAr, e.indexElement, cur_pat.Substring("NPDef".Length).Trim());
                                        //    AuthTermsAr.TermsAr[k].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                        //}
                                        //int f = FindFunctions.findFragmentINList(AuthTermsAr.TermsAr, cur_fragment);
                                        int f = AuthTermsAr.TermsAr.FindIndex(item => item.TermFragment == cur_fragment);
                                        if (f != -1)
                                            AuthTermsAr.TermsAr[f].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                    }
                                    else if (cur_pat.IndexOf("NPSDef") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            //int k = e.indexElement;
                                            int k = FindFunctions.findPattern(AuthTermsAr.TermsAr, e.indexElement, cur_pat.Substring("NPSDef".Length).Trim());
                                            if (k!=-1)
                                                AuthTermsAr.TermsAr[k].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            AuxiliaryFunctions.getFrequency_(AuthTermsAr);
            return true;
        }

        //CombTerms        
        public void GetXMLCombTerms(CombTerms CombTermsAr)
        {
            string BAT_output = tmpPath + folderPath + "\\CombTerms.bat";
            string TargetPatterns_output = tmpPath + folderPath + "\\targets";
            StreamWriter TargetPatternsWriter = new StreamWriter(TargetPatterns_output, false, Encoding.GetEncoding("Windows-1251"));
            string curPattern = "";
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\COMBNS_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                curPattern = curPattern.Trim();
                if (curPattern.IndexOf("CT") != -1)
                {
                    int len = curPattern.IndexOf("CT") + "CT".Length;
                    //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                    int k = PatternsModel.FindIndex(item => item.first == curPattern);
                    if (k == -1)
                    {
                        pair<string, string> new_p = new pair<string, string>();
                        TargetPatternsWriter.WriteLine(curPattern);
                        new_p.first = curPattern;
                        new_p.second = curPattern.Substring(len).Trim();
                        PatternsModel.Add(new_p);
                    }
                    len = 0;
                }
                while (true)
                {
                    curPattern = fs.ReadLine();
                    if (curPattern == null) break;
                    if (curPattern != "")
                    {
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                        curPattern = curPattern.Trim();
                        if (curPattern.IndexOf("CT") != -1)
                        {
                            int len = curPattern.IndexOf("CT") + "CT".Length;
                            //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                            int k = PatternsModel.FindIndex(item => item.first == curPattern);
                            if (k == -1)
                            {
                                pair<string, string> new_p = new pair<string, string>();
                                TargetPatternsWriter.WriteLine(curPattern);
                                new_p.first = curPattern;
                                new_p.second = curPattern.Substring(len).Trim();
                                PatternsModel.Add(new_p);
                            }
                            len = 0;
                        }
                    }
                }
                TargetPatternsWriter.Close();
                string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
                string LSPL_patterns = programmPath + "\\Patterns\\COMBNS_TERM.txt";
                //string LSPL_output = tmpPath + folderPath + "\\CombTermsOutput.xml";
                string LSPL_output_text = tmpPath + folderPath + "\\CombTermsOutputText.xml";
                StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine("\"" + LSPL_exe
                     + "\" -i \"" + inputFile
                     + "\" -p \"" + LSPL_patterns
                    //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + LSPL_output_text
                    //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + TargetPatterns_output + "\" ");
                sw.Close();
                ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //System.Diagnostics.Process.Start(startInfo).WaitForExit();
                GetCombTerms(CombTermsAr);
                string patternsName = FormCombComponentsPatterns(CombTermsAr);
                GetXMLCombComponentsTerms(CombTermsAr, patternsName);
            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами бессоюзных терминов!");
                System.Windows.Forms.Application.Exit();
            }
            return;
        }
        public string FormCombComponentsPatterns(CombTerms CombTermsAr)
        {
            string CombComponentsPatterns = tmpPath + folderPath + "\\COMP_COMB_TERM.txt";
            string patterns = "";
            StreamWriter sw = new StreamWriter(CombComponentsPatterns, false, Encoding.GetEncoding("Windows-1251"));
            //AuxiliaryFunctions.PrintConstantPatterns(sw);
            for (int i = 0; i < CombTermsAr.TermsAr.Count; i++)
            {
                for (int j = 0; j < CombTermsAr.TermsAr[i].Components.Count; j++)
                {
                    //CombTermsAr.TermsAr[i].Components[j].NPattern = AuxiliaryFunctions.NormalizeNPattern(CombTermsAr.TermsAr[i].Components[j].NPattern);
                    string patternName = "CCP" + CombTermsAr.TermsAr[i].Components[j].Pattern + "-" + CombTermsAr.TermsAr[i].Pattern;
                    if (CombTermsAr.TermsAr[i].Components[j].NPattern != "")
                    {
                        sw.WriteLine(patternName + " = " + CombTermsAr.TermsAr[i].Components[j].NPattern);
                        patterns = patterns + patternName + "\n";
                    }
                }
            }
            sw.Close();
            return patterns;

        }
        public void GetXMLCombComponentsTerms(CombTerms CombTermsAr, string patterns)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = tmpPath + folderPath + "\\COMP_COMB_TERM.txt";
            string LSPL_output_text = tmpPath + folderPath + "\\CombComponentsTermsOutputText.xml";
            string BAT_output = tmpPath + folderPath + "\\CombComponentsTerms.bat";
            string TargetPatterns_output = tmpPath + folderPath + "\\targets";
            StreamWriter TargetPatternsWriter = new StreamWriter(TargetPatterns_output, false, Encoding.GetEncoding("Windows-1251"));
            TargetPatternsWriter.Write(patterns);
            TargetPatternsWriter.Close();
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine("\"" + LSPL_exe
                     + "\" -i \"" + inputFile
                     + "\" -p \"" + LSPL_patterns
                    //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + LSPL_output_text
                    //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + TargetPatterns_output + "\" ");
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            //sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patterns);
            //Close the file
            sw.Close();
            ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            GetCombComponentsTerms(CombTermsAr);
            //---------------------------------
            return;
        }
        public bool GetCombComponentsTerms(CombTerms CombTermsAr)
        {
            System.Drawing.Point cur_pos = new System.Drawing.Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string lastPOS = "";
            List<CombComponent> CompV = new List<CombComponent>();
            using (XmlReader xml = XmlReader.Create(tmpPath + folderPath + "\\CombComponentsTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                cur_pat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                cur_pos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPOS = xml.Value.Trim();
                                        }
                                    }
                                    
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    cur_fragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat == "CompCombPat")
                                    {
                                        string word = xml.Value.Trim();
                                        //int k = FindFunctions.findINList(CompV, word);
                                        int k = CompV.FindIndex(item => item.TermWord == word);
                                        if (k == -1)
                                        {
                                            CombComponent newEl = new CombComponent();
                                            newEl.Pos.Add(cur_pos);
                                            newEl.TermWord = word;
                                            newEl.POSstr = lastPOS;
                                            newEl.TermFragment = cur_fragment;
                                            newEl.Pattern = cur_pat.Substring("CCP".Length).Trim();
                                            CompV.Add(newEl);

                                        }
                                        else
                                        {
                                            if (FindFunctions.findPOS(CompV[k].Pos, cur_pos) == -1)
                                            {
                                                CompV[k].Pos.Add(cur_pos);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            while (CompV.Count > 0)
            {
                for (int i = 0; i < CompV[0].Pos.Count; i++)
                {
                    TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(CompV[0].Pos[i]);
                    TermTree e = CombTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                    while (e != null)
                    {
                        int curTerm = FindFunctions.findPattern(CombTermsAr.TermsAr, e.indexElement, CompV[0].Pattern.Substring(0, CompV[0].Pattern.IndexOf('-')).Trim());
                        int curComp = FindFunctions.findPattern(CombTermsAr.TermsAr[curTerm].Components, CompV[0].Pattern.Substring(CompV[0].Pattern.IndexOf('-') + 1).Trim());
                        //int curComp = FindFunctions.findINList(CombTermsAr.TermsAr[curTerm].Components, CompV[0].TermWord);
                        if (curComp != -1)
                        {
                            //CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.insert(CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.end(), CompV[0].Pos.begin(), CompV[0].Pos.end());
                            CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.InsertRange(CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.Count, CompV[0].Pos);
                            CombTermsAr.TermsAr[curTerm].Components[curComp].frequency = CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.Count;
                        }
                        TermTreeNamespace.Range l = new TermTreeNamespace.Range(CompV[0].Pos[i]);
                        if (l <= e.range)
                            if (e.left != null)
                                e = e.left.FindRangeExtension(l);
                            else e = null;
                        else
                            if (e.right != null)
                                e = e.right.FindRangeExtension(l);
                            else e = null;
                    }
                }
                CompV.RemoveAt(0);
            }
            return true;
        }
        public bool GetCombTerms(CombTerms CombTermsAr)
        {
            System.Drawing.Point cur_pos = new System.Drawing.Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string lastPOS = "";
            List<CombComponent> CompV = new List<CombComponent>();
            using (XmlReader xml = XmlReader.Create(tmpPath + folderPath + "\\CombTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                cur_pat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                cur_pos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPOS = xml.Value.Trim();
                                        }
                                    }
                                    
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    cur_fragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat.IndexOf("FCT") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        int check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray<char>());
                                        if (check != -1) 
                                            continue;
                                        //int k = FindFunctions.findINList(CombTermsAr.TermsAr, word, lastPOS);
                                        int k = CombTermsAr.TermsAr.FindIndex(item => item.TermWord == word && item.POSstr == lastPOS);
                                        if (k == -1)
                                        {
                                            TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                            TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                            TermTree e_extension = CombTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                            CombTerm newEl = new CombTerm();
                                            newEl.Pos.Add(null);
                                            newEl.TermWord = word;
                                            newEl.POSstr = lastPOS;
                                            newEl.frequency = 1;
                                            newEl.TermFragment = cur_fragment;
                                            newEl.Pattern = cur_pat.Substring("FCT".Length);
                                            newEl.PatCounter = 0;
                                            newEl.kind = KindOfTerm.CombTerm;
                                            if (e == null && e_extension == null)
                                            {
                                                CombTermsAr.rootTermsTree.AddRange(cur_range);
                                                CombTermsAr.TermsAr.Add(newEl);
                                                e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(CombTermsAr.TermsAr.Count - 1);
                                                CombTermsAr.TermsAr[CombTermsAr.TermsAr.Count - 1].Pos[CombTermsAr.TermsAr[CombTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                            else
                                            {
                                                if (e != null && e_extension == null)
                                                {
                                                    CombTermsAr.TermsAr.Add(newEl);
                                                    e.indexElement.Add(CombTermsAr.TermsAr.Count - 1);
                                                    CombTermsAr.TermsAr[CombTermsAr.TermsAr.Count - 1].Pos[CombTermsAr.TermsAr[CombTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                            TermTreeNamespace.TermTree e =  CombTermsAr.rootTermsTree.FindRange(cur_range);
                                            TermTree e_extension = CombTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                            if ( e == null && e_extension == null)
                                            {
                                                CombTermsAr.rootTermsTree.AddRange(cur_range);
                                                e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(k);
                                                CombTermsAr.TermsAr[k].Pos.Add(e);
                                                CombTermsAr.TermsAr[k].frequency++;
                                            }
                                            else
                                            {
                                                if (e != null && !e.indexElement.Contains(k))
                                                {
                                                    e.indexElement.Add(k);
                                                }
                                            }
                                        }
                                    }
                                    else if (cur_pat.IndexOf("CT") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        int check = word.IndexOfAny(".[]{}()_<>.?!\";:',|\\/".ToArray<char>());
                                        if (check != -1) 
                                            continue;
                                        string cur_main_pat = cur_pat.Substring("CT".Length, cur_pat.IndexOf('-') - "CT".Length).Trim();
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            int cur_Term = FindFunctions.findPattern(CombTermsAr.TermsAr, e.indexElement, cur_main_pat);
                                            //if (FindFunctions.findINList(CombTermsAr.TermsAr[cur_Term].Components, word) == -1)
                                            if (cur_Term !=-1 && CombTermsAr.TermsAr[cur_Term].Components.FindIndex(item => item.TermWord == word) == -1)
                                            {
                                                CombComponent newEl = new CombComponent();
                                                newEl.TermWord = word;
                                                newEl.POSstr = lastPOS;
                                                newEl.TermFragment = cur_fragment;
                                                newEl.frequency = 1;
                                                newEl.Pattern = cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim();
                                                CombTermsAr.TermsAr[cur_Term].Components.Add(newEl);
                                            }
                                        }
                                    }
                                    else if (cur_pat.IndexOf("NPFCT") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            int k = FindFunctions.findPattern(CombTermsAr.TermsAr, e.indexElement, cur_pat.Substring("NPFCT".Length).Trim());
                                            if (k != -1)
                                                CombTermsAr.TermsAr[k].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                        }
                                    }
                                    else if (cur_pat.IndexOf("NPCT") == 0)
                                    {
                                        string word = xml.Value.Trim();
                                        string cur_main_pat = cur_pat.Substring("NPCT".Length, cur_pat.IndexOf('-') - "NPCT".Length).Trim();
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            int k = FindFunctions.findPattern(CombTermsAr.TermsAr, e.indexElement, cur_main_pat);
                                            if (k != -1)
                                            {
                                                int a = FindFunctions.findPattern(CombTermsAr.TermsAr[k].Components, cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim());
                                                if (a != -1)
                                                    CombTermsAr.TermsAr[k].Components[a].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            //getFrequency(CombTerms);
            return true;
        }

        //DictTerms
        public void GetXMLDictTerms(Terms DictTermsAr)
        {
            string LSPL_patterns = "";
            StreamReader fs = null;
            switch (dictionary)
            {
                case DictionaryF.IT_TERM:
                    {
                        LSPL_patterns = programmPath + "\\Patterns\\IT_TERM.txt";
                        fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding(1251));
                        break;
                    }
                case DictionaryF.F_TERM:
                    {
                        LSPL_patterns = programmPath + "\\Patterns\\F_TERM.txt";
                        fs = new StreamReader(LSPL_patterns, Encoding.GetEncoding(1251));
                        break;
                    }
            }

            string BAT_output = tmpPath + folderPath + "\\DictTerms.bat";
            string TargetPatterns_output = tmpPath + folderPath + "\\targets";
            StreamWriter TargetPatternsWriter = new StreamWriter(TargetPatterns_output, false, Encoding.GetEncoding("Windows-1251"));
            string curPattern = "";
            curPattern = fs.ReadLine();

            string patternsName = "";
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_output_text = tmpPath + folderPath + "\\DictTermsOutputText.xml";
            string BAT_command = "\"" + LSPL_exe
                     + "\" -i \"" + inputFile
                     + "\" -p \"" + LSPL_patterns
                //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + LSPL_output_text
                //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + TargetPatterns_output + "\" ";
            bool callUtilit = false;
            curPattern = fs.ReadLine();
            if (curPattern != "")
            {
                curPattern = curPattern.Trim();
                curPattern = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                TargetPatternsWriter.WriteLine(curPattern);
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
                    //if (BAT_command.Length + patternsName.Length + curPatternName.Length < MAX_COMMAND)
                    //{
                    TargetPatternsWriter.WriteLine(curPatternName);
                    pair<string, string> cur_pat = new pair<string, string>();
                    cur_pat.first = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    cur_pat.second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                    DictPatterns.Add(cur_pat);
                    //callUtilit = false;
                    //}
                    //else
                    //{
                    //    StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                    //    sw.WriteLine(BAT_command + patternsName);
                    //    sw.Close();
                    //    ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
                    //    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //    ////System.Diagnostics.Process.Start(startInfo).WaitForExit();
                    //    GetDictTerms(DictTermsAr);
                    //    patternsName = curPatternName;
                    //    pair<string, string> cur_pat = new pair<string, string>();
                    //    cur_pat.first = curPattern.Substring(0, curPattern.IndexOf('=')).Trim();
                    //    cur_pat.second = curPattern.Substring(curPattern.IndexOf("=") + 1).Trim();
                    //    DictPatterns.Add(cur_pat);
                    //    callUtilit = true;
                    //}
                }
            }
            //if (!callUtilit)
            //{
            TargetPatternsWriter.Close();
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            sw.WriteLine(BAT_command);
            sw.Close();
            ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            GetDictTerms(DictTermsAr);
            //}
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
        public bool GetDictTerms(Terms DictTermsAr)
        {
            System.Drawing.Point cur_pos = new System.Drawing.Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string lastPOS = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + folderPath + "\\DictTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                cur_pat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                cur_pos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPOS = xml.Value.Trim();
                                        }
                                    }                                   
                                    
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    cur_fragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    string word = xml.Value.Trim();
                                    //int k = FindFunctions.findINList(DictTermsAr.TermsAr, word);
                                    int k = DictTermsAr.TermsAr.FindIndex(item => item.TermWord == word);
                                    if (k == -1)
                                    {
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        Term newEl = new Term();
                                        newEl.frequency = 0;
                                        newEl.kind = KindOfTerm.DictTerm;
                                        //int element = FindFunctions.findINList(DictPatterns, cur_pat, 1);
                                        int element = DictPatterns.FindIndex(item => item.first == cur_pat);
                                        newEl.NPattern = DictPatterns[element].second;//<-------
                                        newEl.PatCounter = 1;
                                        newEl.Pattern = cur_pat;
                                        newEl.Pos.Add(null);
                                        newEl.setToDel = false;
                                        newEl.TermFragment = cur_fragment;
                                        newEl.TermWord = word;
                                        newEl.POSstr = lastPOS;
                                        TermTree e = DictTermsAr.rootTermsTree.FindRange(cur_range);
                                        TermTree e_extension = DictTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                        if (e == null && e_extension == null)
                                        {
                                            DictTermsAr.rootTermsTree.AddRange(cur_range);
                                            e = DictTermsAr.rootTermsTree.FindRange(cur_range);
                                            DictTermsAr.TermsAr.Add(newEl);
                                            e.indexElement.Add(DictTermsAr.TermsAr.Count - 1);
                                            DictTermsAr.TermsAr[DictTermsAr.TermsAr.Count - 1].Pos[DictTermsAr.TermsAr[DictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                        }
                                        else
                                        {
                                            if (e != null && e_extension == null)
                                            {
                                                DictTermsAr.TermsAr.Add(newEl);
                                                e.indexElement.Add(DictTermsAr.TermsAr.Count - 1);
                                                DictTermsAr.TermsAr[DictTermsAr.TermsAr.Count - 1].Pos[DictTermsAr.TermsAr[DictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        TermTree e = DictTermsAr.rootTermsTree.FindRange(cur_range);
                                        TermTree e_extension = DictTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                        if (e == null && e_extension == null)
                                        {
                                            DictTermsAr.rootTermsTree.AddRange(cur_range);
                                            e = DictTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement.Add(k);
                                            DictTermsAr.TermsAr[k].Pos.Add(e);
                                            DictTermsAr.TermsAr[k].frequency++;
                                        }
                                        else
                                        {
                                            if (e != null && !e.indexElement.Contains(k))
                                                e.indexElement.Add(k);
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            AuxiliaryFunctions.getFrequency_(DictTermsAr);
            return true;
        }

        //NonDictTerms
        public void GetXMLNonDictTerms(NonDictTerms NonDictTermsAr)
        {
            string BAT_output = tmpPath + folderPath + "\\NontDictTerms.bat";
            string TargetPatterns_output = tmpPath + folderPath + "\\targets";
            StreamWriter TargetPatternsWriter = new StreamWriter(TargetPatterns_output, false, Encoding.GetEncoding("Windows-1251"));
            string curPattern = "";
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\NONDICT_TERM_NEW.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            string LSPL_patterns = programmPath + "\\Patterns\\NONDICT_TERM_NEW.txt";
            //string patternsName = "";
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
                    //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                    int k = PatternsModel.FindIndex(item => item.first == curPattern);
                    if (k == -1 && len != 0)
                    {
                        pair<string, string> new_p = new pair<string, string>();
                        //patternsName = patternsName + " " + curPattern.Trim();
                        TargetPatternsWriter.WriteLine(curPattern.Trim());
                        new_p.first = curPattern;
                        new_p.second = curPattern.Substring(len).Trim();
                        PatternsModel.Add(new_p);
                    }
                }
            }
            TargetPatternsWriter.Close();
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_output_text = tmpPath + folderPath + "\\NontDictTermsOutputText.xml";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe
                     + "\" -i \"" + inputFile
                     + "\" -p \"" + LSPL_patterns
                //+ "\" -o \"" + LSPL_output
                     + "\" -t \"" + LSPL_output_text
                //+ "\" -r \"" + LSPL_output_patterns
                     + "\" -s \"" + TargetPatterns_output + "\" ");
            //Close the file
            sw.Close();
            ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            GetNonDictTerms(NonDictTermsAr);
            //---------------------------------
            return;
        }
        public bool GetNonDictTerms(NonDictTerms NonDictTermsAr)
        {
            System.Drawing.Point cur_pos = new System.Drawing.Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string lastPOS = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + folderPath + "\\NontDictTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                cur_pat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                cur_pos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPOS = xml.Value.Trim();
                                        }
                                    }
                                    
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    cur_fragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    switch (cur_pat[0])
                                    {
                                        case 'F':
                                            {
                                                string word = xml.Value.Trim();
                                                //int k = FindFunctions.findINList(NonDictTermsAr.TermsAr, word, lastPOS);
                                                int k = NonDictTermsAr.TermsAr.FindIndex(item => item.TermWord == word && item.POSstr == lastPOS);
                                                if (k == -1)
                                                {
                                                    TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                                    NonDictTerm newEl = new NonDictTerm();
                                                    newEl.Pos.Add(null);
                                                    newEl.TermWord = word;
                                                    newEl.frequency = 1;
                                                    newEl.TermFragment = cur_fragment;
                                                    newEl.Pattern = cur_pat.Substring("F".Length).Trim();
                                                    newEl.POSstr = lastPOS;
                                                    newEl.kind = KindOfTerm.NonDictTerm;
                                                    TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                    TermTree e_extension = NonDictTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                                    if (e == null && e_extension == null)
                                                    {
                                                        NonDictTermsAr.rootTermsTree.AddRange(cur_range);
                                                        e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                        NonDictTermsAr.TermsAr.Add(newEl);
                                                        e.indexElement.Add(NonDictTermsAr.TermsAr.Count - 1);
                                                        NonDictTermsAr.TermsAr[NonDictTermsAr.TermsAr.Count - 1].Pos[NonDictTermsAr.TermsAr[NonDictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                    }
                                                    else
                                                    {
                                                        if (e != null && e_extension == null)
                                                        {
                                                            NonDictTermsAr.TermsAr.Add(newEl);
                                                            e.indexElement.Add(NonDictTermsAr.TermsAr.Count - 1);
                                                            NonDictTermsAr.TermsAr[NonDictTermsAr.TermsAr.Count - 1].Pos[NonDictTermsAr.TermsAr[NonDictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                                    TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                    TermTree e_extension = NonDictTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                                    if (e == null && e_extension == null)
                                                    {
                                                        NonDictTermsAr.rootTermsTree.AddRange(cur_range);
                                                        e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                        e.indexElement.Add(k);
                                                        NonDictTermsAr.TermsAr[k].Pos.Add(e);
                                                        NonDictTermsAr.TermsAr[k].frequency++;
                                                    }
                                                    else
                                                    {
                                                        if (e!=null && !e.indexElement.Contains(k))
                                                            e.indexElement.Add(k);
                                                    }

                                                }
                                                break;
                                            }
                                        case 'C':
                                            {
                                                string word = xml.Value.Trim();
                                                TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                                TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                if (e != null)
                                                {
                                                    int cur_term = FindFunctions.findPattern(NonDictTermsAr.TermsAr, e.indexElement, cur_pat.Substring("Ca".Length, cur_pat.IndexOf('-') - "Ca".Length).Trim());
                                                    if (cur_term != -1)
                                                    {
                                                        //int block = FindFunctions.findBlock(NonDictTermsAr.TermsAr[cur_term].Blocks, cur_pat[1].ToString());
                                                        int block = NonDictTermsAr.TermsAr[cur_term].Blocks.FindIndex(item => item.Block == cur_pat[1].ToString());
                                                        if (block != -1)
                                                        {
                                                            int cur_comp = FindFunctions.findPattern(NonDictTermsAr.TermsAr[cur_term].Blocks[block].Components, cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim());
                                                            if (cur_comp == -1)
                                                            {
                                                                NonDictComponent newEl = new NonDictComponent();
                                                                newEl.Component = word;
                                                                newEl.POSstr = lastPOS;
                                                                newEl.Pattern = cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim();
                                                                NonDictTermsAr.TermsAr[cur_term].Blocks[block].Components.Add(newEl);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            NonDictBlock newBl = new NonDictBlock();
                                                            newBl.Block = cur_pat[1].ToString();
                                                            NonDictComponent newEl = new NonDictComponent();
                                                            newEl.Component = word;
                                                            newEl.POSstr = lastPOS;
                                                            newEl.Pattern = cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim();
                                                            newBl.Components.Add(newEl);
                                                            NonDictTermsAr.TermsAr[cur_term].Blocks.Add(newBl);
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        case 'N':
                                            {
                                                string word = xml.Value.Trim();
                                                TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                                TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                if (e != null)
                                                {
                                                    switch (cur_pat["NP".Length])
                                                    {
                                                        case 'F':
                                                            {
                                                                int k = FindFunctions.findPattern(NonDictTermsAr.TermsAr, e.indexElement, cur_pat.Substring("NPF".Length).Trim());
                                                                if (k != -1)
                                                                {
                                                                    NonDictTermsAr.TermsAr[k].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                                                }
                                                                break;
                                                            }
                                                        case 'C':
                                                            {
                                                                int cur_term = FindFunctions.findPattern(NonDictTermsAr.TermsAr, e.indexElement, cur_pat.Substring("NPCa".Length, cur_pat.IndexOf('-') - "NPCa".Length).Trim());
                                                                if (cur_term != -1)
                                                                {
                                                                    //int block = FindFunctions.findBlock(NonDictTermsAr.TermsAr[cur_term].Blocks, cur_pat[4].ToString());
                                                                    int block = NonDictTermsAr.TermsAr[cur_term].Blocks.FindIndex(item => item.Block == cur_pat[4].ToString());
                                                                    if (block != -1)
                                                                    {
                                                                        int cur_comp = FindFunctions.findPattern(NonDictTermsAr.TermsAr[cur_term].Blocks[block].Components, cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim());
                                                                        if (cur_comp != -1)
                                                                        {
                                                                            NonDictTermsAr.TermsAr[cur_term].Blocks[block].Components[cur_comp].NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                                                        }
                                                                    }
                                                                }
                                                                break;
                                                            }
                                                    }
                                                }
                                                break;
                                            }
                                    }
                                }

                            }
                            break;
                    }
                }
            }
            AuxiliaryFunctions.getFrequency_(NonDictTermsAr);
            return true;
        }

        //SynTerms
        public void GetXMLSynTerms(SynTerms SynTermsAr)
        {
            string BAT_output = tmpPath + folderPath + "\\SynTerms.bat";
            string TargetPatterns_output = tmpPath + folderPath + "\\targets";
            StreamWriter TargetPatternsWriter = new StreamWriter(TargetPatterns_output, false, Encoding.GetEncoding("Windows-1251"));
            string curPattern = "";
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\SYN_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            string LSPL_patterns = programmPath + "\\Patterns\\SYN_TERM.txt";
            string patternsName = "";
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
                        //int k = FindFunctions.findINList(PatternsModel, curPattern, 1);
                        int k = PatternsModel.FindIndex(item => item.first == curPattern);
                        if (k == -1)
                        {
                            pair<string, string> new_p = new pair<string, string>();
                            //patternsName = patternsName + " " + curPattern;
                            TargetPatternsWriter.WriteLine(curPattern);
                            new_p.first = curPattern;
                            new_p.second = curPattern.Substring(len).Trim();
                            PatternsModel.Add(new_p);
                        }
                        len = 0;
                    }
                }
            }
            TargetPatternsWriter.Close();
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_output_text = tmpPath + folderPath + "\\SynTermsOutputText.xml";            
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe
                    + "\" -i \"" + inputFile
                    + "\" -p \"" + LSPL_patterns
                //+ "\" -o \"" + LSPL_output
                    + "\" -t \"" + LSPL_output_text
                //+ "\" -r \"" + LSPL_output_patterns
                    + "\" -s \"" + TargetPatterns_output + "\" ");
            //Close the file
            sw.Close();
            ProcessStartInfo startInfo = new ProcessStartInfo(BAT_output);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //System.Diagnostics.Process.Start(startInfo).WaitForExit();
            GetSynTerms(SynTermsAr);
            //---------------------------------
            return;
        }
        public bool GetSynTerms(SynTerms SynTermsAr)
        {
            System.Drawing.Point cur_pos = new System.Drawing.Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string lastPOS = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + folderPath + "\\SynTermsOutputText.xml"))
            {
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "goal")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "name")
                                            {
                                                cur_pat = xml.Value.Trim();
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "match")
                                {
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "startPos")
                                            {
                                                cur_pos.X = Convert.ToInt32(xml.Value.Trim());
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value.Trim());
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "pos")
                                        {
                                            lastPOS = xml.Value.Trim();
                                        }
                                    }
                                    
                                }
                                else if (xml.Name == "fragment")
                                {
                                    lastNodeName = xml.Name;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "fragment")
                                {
                                    cur_fragment = xml.Value.Trim();
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat.IndexOf("NP") != 0)
                                    {
                                        string word = xml.Value.Trim();
                                        pair<SynTermAlternative, SynTermAlternative> alt = new pair<SynTermAlternative, SynTermAlternative>();
                                        alt.first = new SynTermAlternative();
                                        alt.second = new SynTermAlternative();
                                        int y = word.IndexOf(" - ");
                                        if (y != -1)
                                        {
                                            string PartsPatterns = cur_pat.Substring(cur_pat.IndexOf('-') + 1).Trim();
                                            alt.first.alternative = word.Substring(0, y);
                                            List<string> firstAltWords = alt.first.alternative.Split(' ').ToList<string>();
                                            int numWords = firstAltWords.Count;
                                            List<string> POSlist = lastPOS.Split(' ').ToList<string>();
                                            string firstAltPOS = "";
                                            string secondAltPOS = "";
                                            bool LDelimeter = false;
                                            for (int i = 0; i < POSlist.Count; i++)
                                            {
                                                if (POSlist[i] == "L")
                                                {
                                                    LDelimeter = true;
                                                    continue;
                                                }
                                                if (LDelimeter)
                                                    firstAltPOS += " "+POSlist[i];
                                                else
                                                    secondAltPOS += " "+POSlist[i];
                                            }
                                            alt.first.POSstr = firstAltPOS.Trim();
                                            alt.first.PatternPart = "A";
                                            alt.first.Pattern = PartsPatterns.Substring(0, PartsPatterns.IndexOf('-')).Trim();

                                            alt.second.alternative = word.Substring(y + 3, word.Length - (y + 3)).Trim();
                                            alt.second.PatternPart = "B";
                                            alt.second.Pattern = PartsPatterns.Substring(PartsPatterns.IndexOf('-') + 1).Trim();
                                            alt.second.POSstr = secondAltPOS.Trim();

                                            int k = FindFunctions.findINListPos(SynTermsAr.TermsAr, alt);
                                            if (k == -1)
                                            {
                                                SynTerm newEl = new SynTerm();
                                                //добавляем алтернативы в список
                                                newEl.alternatives.first = alt.first;
                                                newEl.alternatives.second = alt.second;
                                                //вычисляем точные координаты альтернатив пары ------> нужно ли? если да дописать
                                                newEl.frequency = 1;
                                                newEl.setToDel = false;
                                                newEl.Pos.Add(null);
                                                newEl.TermFragment = cur_fragment;
                                                newEl.Pattern = PartsPatterns;
                                                newEl.kind = KindOfTerm.SynTerm;
                                                TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                                TermTree e = SynTermsAr.rootTermsTree.FindRange(cur_range);
                                                TermTree e_extension = SynTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                                if (e == null && e_extension == null)
                                                {
                                                    SynTermsAr.rootTermsTree.AddRange(cur_range);
                                                    e = SynTermsAr.rootTermsTree.FindRange(cur_range);
                                                    SynTermsAr.TermsAr.Add(newEl);
                                                    e.indexElement.Add(SynTermsAr.TermsAr.Count - 1);
                                                    SynTermsAr.TermsAr[SynTermsAr.TermsAr.Count - 1].Pos[SynTermsAr.TermsAr[SynTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                }
                                                else
                                                {
                                                    if (e != null && e_extension == null)
                                                    {
                                                        SynTermsAr.TermsAr.Add(newEl);
                                                        e.indexElement.Add(SynTermsAr.TermsAr.Count - 1);
                                                        SynTermsAr.TermsAr[SynTermsAr.TermsAr.Count - 1].Pos[SynTermsAr.TermsAr[SynTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                                TermTree e = SynTermsAr.rootTermsTree.FindRange(cur_range);
                                                TermTree e_extension = SynTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                                                if (e == null && e_extension == null)
                                                {
                                                    SynTermsAr.rootTermsTree.AddRange(cur_range);
                                                    e = SynTermsAr.rootTermsTree.FindRange(cur_range);                                                    
                                                    e.indexElement.Add(k);
                                                    SynTermsAr.TermsAr[k].Pos.Add(e);
                                                    SynTermsAr.TermsAr[k].frequency++;
                                                }
                                                else
                                                {
                                                    if (e!=null && !e.indexElement.Contains(k))
                                                        e.indexElement.Add(k);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string word = xml.Value.Trim();
                                        TermTreeNamespace.Range cur_range = new TermTreeNamespace.Range(cur_pos);
                                        TermTree e = SynTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            int last_def = cur_pat.LastIndexOf('-');
                                            string main_pat = cur_pat.Substring(cur_pat.IndexOf('-') + 1, last_def - (cur_pat.IndexOf('-') + 1)).Trim();
                                            int main_syn = FindFunctions.findPattern(SynTermsAr.TermsAr, e.indexElement, main_pat);
                                            string pattern_part = cur_pat.Substring(last_def + 1).Trim();
                                            if (main_syn != -1)
                                            {
                                            switch (pattern_part)
                                            {
                                                case "A":
                                                    {
                                                        SynTermsAr.TermsAr[main_syn].alternatives.first.NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                                        break;
                                                    }
                                                case "B":
                                                    {
                                                        SynTermsAr.TermsAr[main_syn].alternatives.second.NPattern = AuxiliaryFunctions.NormalizeNPattern(word);
                                                        break;
                                                    }
                                            }
                                                }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            AuxiliaryFunctions.getFrequency_(SynTermsAr);
            return true;

        }

        // Постраничное извлечение текста из Docx документа -------------------------------------
        private List<string> ReadTextFromDocxByPage(string PathToFile)
        {
            List<string> pages = new List<string>();
            object path = PathToFile;
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            object miss = System.Reflection.Missing.Value;
            object readOnly = false;
            Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
            string totaltext = "";
            object obj = "\\page";
            var numberOfPages = docs.ComputeStatistics(Microsoft.Office.Interop.Word.WdStatistic.wdStatisticPages, false);
            Word.Range rng = word.Selection.Bookmarks.get_Item(ref obj).Range;
            for (int i = 1; i <= numberOfPages; i++)
            {
                pages.Add(rng.Text);
                word.Selection.GoToNext(Microsoft.Office.Interop.Word.WdGoToItem.wdGoToPage);
                rng = word.Selection.Bookmarks.get_Item(ref obj).Range;
            }
            docs.Close();
            word.Quit();
            pages = pages.GetRange(this.startPage - 1, this.endPage - this.startPage + 1);
            return pages;
        }

        //---------------------------------------------------------------------------------------
    }
}


////string cmdCommand = " -i \"" + file + "\" -p  -o  Definition SeparateDefinition";


////ofstream out_bat(outFile);
////if (!out_bat) {
////    cout << "Cannot open file.\n";
////    return;
////}

//out_bat<<\n"<<cmdCommand;
//out_bat.close();
//cmdCommand="\""+outFile+"\"";
////CreateWaitChildProcess(cmdCommand);

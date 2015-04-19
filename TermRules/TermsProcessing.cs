using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;
namespace TermRules
{
    public enum Dictionary
    {
        IT_TERM,
        F_TERM
    }
    public class TermsProcessing
    {        
        public Terms MainTermsAr;
        public Terms AuthTermsAr;
        public Terms DictTermsAr;
        public NonDictTerms NonDictTermsAr;
        public CombTerms CombTermsAr;
        public List<pair<string, string>> PatternsModel; 
        public string tmpPath;
        public string programmPath;
        public string folderPath;
        public string inputFile;
        public string outputFile;
        FindFunctions find;
        AuxiliaryFunctions aux;
        Dictionary dictionary;
        public TermsProcessing(string inputfile, Dictionary dict)
        {
            find = new FindFunctions();
            aux = new AuxiliaryFunctions();
            folderPath = "TermsProcessingF";
            outputFile = "";
            inputFile = inputfile;
            dictionary = dict;
            tmpPath = Path.GetTempPath();
            programmPath = Application.StartupPath.ToString();
            MainTermsAr = new Terms();
            AuthTermsAr = new Terms();
            DictTermsAr = new Terms();
            NonDictTermsAr = new NonDictTerms();
            CombTermsAr = new CombTerms();
            Directory.CreateDirectory(tmpPath + "\\" + folderPath);
            PatternsModel = new List<pair<string, string>>();
            FormBatfiles();
        }
        public void FormBatfiles() 
        { 
            string text = "";
            string patternsName = "";
            string curPattern = "";
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\AUTH_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            curPattern = fs.ReadLine();
            if (curPattern != null)
            {
                curPattern = curPattern.Substring(0, curPattern.IndexOf('='));
                if (curPattern.IndexOf("DefIns") == -1 && curPattern.IndexOf("DefIns") == -1)
                {
                    int len = 0;
                    if(curPattern.IndexOf("Def") !=-1 && curPattern.Length>3)
                    {
                        len = "Def".Length;
                    }
                    else if (curPattern.IndexOf("SDef") !=-1)
                    {
                        len = "SDef".Length;
                    }
                    else if (curPattern.IndexOf("NPDef") != -1)
                    {
                        len = "NPDef".Length;
                    }
                    else if (curPattern.IndexOf("NPSDef") != -1)
                    {
                        len = "NPSDef".Length;
                    }
                    if (len > 0)
                    {
                        int k = find.findINList(PatternsModel, curPattern, 1);
                        if (k == -1)
                        {
                            pair<string, string> new_p = new pair<string, string>();
                            patternsName = patternsName + " " + curPattern;
                            new_p.first = curPattern;
                            new_p.second = curPattern.Substring(len);
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
                        curPattern = curPattern.Substring(0, curPattern.IndexOf('='));
                        if (curPattern.IndexOf("DefIns") == -1 && curPattern.IndexOf("DefXXX") == -1)
                        {
                            int len = 0;
                            if (curPattern.IndexOf("Def") != -1 && curPattern.Length > 3)
                            {
                                len = "Def".Length;
                            }
                            else if (curPattern.IndexOf("SDef") != -1)
                            {
                                len = "SDef".Length;
                            }
                            else if (curPattern.IndexOf("NPDef") != -1)
                            {
                                len = "NPDef".Length;
                            }
                            else if (curPattern.IndexOf("NPSDef") != -1)
                            {
                                len = "NPSDef".Length;
                            }
                            if (len > 0)
                            {
                                int k = find.findINList(PatternsModel, curPattern, 1);
                                if (k == -1)
                                {
                                    pair<string, string> new_p = new pair<string, string>();
                                    patternsName = patternsName + " " + curPattern;
                                    new_p.first = curPattern;
                                    new_p.second = curPattern.Substring(len);
                                    PatternsModel.Add(new_p);
                                }
                                len = 0;
                            }
                        }
                    }
                }
                string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
                string LSPL_patterns = programmPath + "\\Patterns\\AUTH_TERM.txt";
                string LSPL_output = tmpPath + "\\" + folderPath + "\\AuthTermsOutput.xml";
                string BAT_output = tmpPath + "\\" + folderPath + "\\AuthTerms.bat";
                StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
                sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" Definition SeparateDefinition NPDefinition NPSeparateDefinition");
                sw.Close();
            }
            else
            {
                MessageBox.Show("Ошибка! Некорректный файл с шаблонами авторских терминов!");
                Application.Exit();
            }
        }
        //AuthTerms
        public void GetXMLAuthTerms(Terms AuthTermsAr)
        {
            
            //--------------------------------
            string BAT_output = tmpPath + "\\" + folderPath + "\\AuthTerms.bat";
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetAuthTerms(AuthTermsAr);
            //-----------------------------------------
            return;
        }
        public bool GetAuthTerms(Terms AuthTermsAr)
        {
            Point cur_pos = new Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + "\\" + folderPath + "\\AuthTermsOutput.xml"))
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
                                                cur_pat = xml.Value;
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
                                                cur_pos.X = Convert.ToInt32(xml.Value);
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value);
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
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
                                   cur_fragment = xml.Value;
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat.IndexOf("Def") == 0)
                                    {
                                        string word = xml.Value;
                                        int k = find.findINList(AuthTermsAr.TermsAr, word);
                                        if (k == -1)
                                        {
                                            cur_pos = aux.GetRealPos(cur_fragment, word, cur_pos);
                                            Range cur_range = new Range(cur_pos);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e == null)
                                            {
                                                Term newEl = new Term();
                                                newEl.TermWord = word;
                                                newEl.frequency = 1;
                                                newEl.setToDel = false;
                                                newEl.Pos.Add(null);
                                                newEl.TermFragment = cur_fragment;
                                                newEl.Pattern = cur_pat.Substring("Def".Length);
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                AuthTermsAr.TermsAr.Add(newEl);
                                                e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                            else
                                            {
                                                Term newEl = new Term();
                                                newEl.TermWord = word;
                                                newEl.frequency = 1;
                                                newEl.setToDel = false;
                                                newEl.Pos.Add(null);
                                                newEl.TermFragment = cur_fragment;
                                                newEl.Pattern = cur_pat.Substring("Def".Length);
                                                AuthTermsAr.TermsAr.Add(newEl);
                                                e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                        }
                                        else
                                        {
                                            cur_pos = aux.GetRealPos(cur_fragment, word, cur_pos);
                                            Range cur_range = new Range(cur_pos);
                                            if (AuthTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(k);
                                                AuthTermsAr.TermsAr[k].Pos.Add(e);
                                                AuthTermsAr.TermsAr[k].frequency++;
                                            }
                                        }
                                    }
                                    else if (cur_pat.IndexOf("SDef") == 0)
                                    {
                                        string word = xml.Value;
                                        int k = find.findINList(AuthTermsAr.TermsAr, word);
                                        if (k == -1)
                                        {
                                            Range cur_range = new Range(cur_pos);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e == null)
                                            {
                                                Term newEl = new Term();
                                                newEl.TermWord = word;
                                                newEl.frequency = 1;
                                                newEl.setToDel = false;
                                                newEl.Pos.Add(null);
                                                newEl.TermFragment = cur_fragment;
                                                newEl.Pattern = cur_pat.Substring("SDef".Length);
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                AuthTermsAr.TermsAr.Add(newEl);
                                                e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }
                                            else
                                            {
                                                Term newEl = new Term();
                                                newEl.TermWord = word;
                                                newEl.frequency = 1;
                                                newEl.setToDel = false;
                                                newEl.Pos.Add(null);
                                                newEl.TermFragment = cur_fragment;
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                AuthTermsAr.TermsAr.Add(newEl);
                                                e.indexElement.Add(AuthTermsAr.TermsAr.Count - 1);
                                                AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                            }                                            
                                        }
                                        else
                                        {
                                            Range cur_range = new Range(cur_pos);
                                            if (AuthTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement.Add(k);
                                                AuthTermsAr.TermsAr[k].Pos.Add(e);
                                                AuthTermsAr.TermsAr[k].frequency++;
                                            }
                                        }
                                    }
                                    else if (cur_pat.IndexOf("NPDef") != -1)
                                    {
                                        string word = xml.Value;
                                        word = word.Replace('[', '<');
                                        word = word.Replace(']', '>');
                                        Range cur_range = new Range(cur_pos);
                                        TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            //int k = e.indexElement;
                                            int k = find.findPattern(AuthTermsAr.TermsAr, e.indexElement, cur_pat.Substring(2));
                                            AuthTermsAr.TermsAr[k].NPattern = word;
                                        }
                                    }
                                 }
                                break;
                            }
                    }
                }
            }
            aux.getFrequency_(AuthTermsAr);
            return true;
        }

        //CombTerms
        
        public void GetXMLCombTerms(CombTerms CombTermsAr)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = programmPath + "\\Patterns\\COMBNS_TERM.txt";
            string LSPL_output = tmpPath + "\\" + folderPath + "\\CombTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\CombTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" FULLCombTerm CombTerm NPFULLCombTerm NPCombTerm");
            //Close the file
            sw.Close();
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetCombTerms(CombTermsAr);
            FormCombComponentsPatterns(CombTermsAr);
            GetXMLCombComponentsTerms(CombTermsAr);
            //---------------------------------
            return;
        }
        public void FormCombComponentsPatterns(CombTerms CombTermsAr)
        {
            //string CombComponentsPatterns = tmpPath + "\\" + folderPath + "\\COMP_COMB_TERM.txt";
            //StreamWriter sw = new StreamWriter(CombComponentsPatterns, false, Encoding.GetEncoding("Windows-1251"));
            //for (int i = 0 ; i < CombTermsAr.TermsAr.Count ; i++)
            //{
            //    for (int j = 0; j < CombTermsAr.TermsAr[i].Components.Count; j++)
            //    {
            //        sw.WriteLine("CompCombPat = "+CombTermsAr.TermsAr[i].Components[j].Pattern);
            //    }
            //}
            //sw.Close();
        }
        public void GetXMLCombComponentsTerms(CombTerms CombTermsAr)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = tmpPath + "\\" + folderPath + "\\COMP_COMB_TERM.txt";
            string LSPL_output = tmpPath + "\\" + folderPath + "\\CombComponentsTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\CombComponentsTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" CompCombPat");
            //Close the file
            sw.Close();
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetCombComponentsTerms(CombTermsAr);
            //---------------------------------
            return;
        }
        public bool GetCombComponentsTerms(CombTerms CombTermsAr)
        {
            Point cur_pos = new Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            List<CombComponent> CompV = new List<CombComponent>();
            using (XmlReader xml = XmlReader.Create(tmpPath + "\\" + folderPath + "\\CombComponentsTermsOutput.xml"))
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
                                                cur_pat = xml.Value;
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
                                                cur_pos.X = Convert.ToInt32(xml.Value);
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value);
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
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
                                        cur_fragment = xml.Value;
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat == "CompCombPat")
                                    {
                                        string word = xml.Value;
                                        int k = find.findINList(CompV, word);
                                        if (k == -1)
                                        {
                                            CombComponent newEl = new CombComponent();
                                            newEl.Pos.Add(cur_pos);
                                            newEl.TermWord = word;
                                            newEl.TermFragment = cur_fragment;
                                            CompV.Add(newEl);

                                        }
                                        else
                                        {
                                            if (find.findPOS(CompV[k].Pos, cur_pos) == -1)
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
                    Range cur_range = new Range(CompV[0].Pos[i]);
                    TermTree e = CombTermsAr.rootTermsTree.FindRangeExtension(cur_range);
                    while (e != null)
                    {
                        int curTerm = e.indexElement;
                        int curComp = find.findINList(CombTermsAr.TermsAr[curTerm].Components, CompV[0].TermWord);
                        if (curComp != -1)
                        {
                            //CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.insert(CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.end(), CompV[0].Pos.begin(), CompV[0].Pos.end());
                            CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.InsertRange(CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.Count, CompV[0].Pos);
                            CombTermsAr.TermsAr[curTerm].Components[curComp].frequency = CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.Count;
                        }
                        Range l = new Range(CompV[0].Pos[i]);
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
            Point cur_pos = new Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string previousPattern = "";
            bool curTermFind = false;
            int curTerm = -1;
            int curComponent = 0;
            List<CombComponent> CompV = new List<CombComponent>();
            using (XmlReader xml = XmlReader.Create(tmpPath + "\\" + folderPath + "\\CombTermsOutput.xml"))
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
                                                cur_pat = xml.Value;
                                                if (curTermFind)
                                                {
                                                    curComponent = 0;
                                                    previousPattern = "";
                                                    curTerm = -1;
                                                    curTermFind = false;
                                                }
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
                                                cur_pos.X = Convert.ToInt32(xml.Value);
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value);
                                            }
                                        }
                                    }
                                    if (cur_pat == "NPCombTerm")
                                    {
                                        Range cur_range = new Range(cur_pos);
                                        if (!curTermFind)
                                        {
                                            TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e != null)
                                            {
                                                curTermFind = true;
                                                curTerm = e.indexElement;
                                            }
                                        }
                                        else
                                        {
                                            TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e != null)
                                            {
                                                curTerm = e.indexElement;
                                                previousPattern = "";
                                                curComponent = 0;
                                            }
                                            else
                                                curTermFind = false;
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
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
                                        cur_fragment = xml.Value;
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat == "FULLCombTerm")
                                    {
                                        string word = xml.Value;
                                        int k = find.findINList(CombTermsAr.TermsAr, word);
                                        if (k == -1)
                                        {
                                            CombTerm newEl = new CombTerm();
                                            newEl.Pos.Add(null);
                                            newEl.TermWord = word;
                                            newEl.frequency = 1;
                                            newEl.TermFragment = cur_fragment;
                                            newEl.PatCounter = 0;
                                            Range cur_range = new Range(cur_pos);
                                            CombTermsAr.rootTermsTree.AddRange(cur_range);
                                            CombTermsAr.TermsAr.Add(newEl);
                                            TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement = CombTermsAr.TermsAr.Count - 1;
                                            CombTermsAr.TermsAr[CombTermsAr.TermsAr.Count - 1].Pos[CombTermsAr.TermsAr[CombTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                        }
                                        else
                                        {
                                            Range cur_range = new Range(cur_pos);
                                            if (CombTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                CombTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement = k;
                                                CombTermsAr.TermsAr[k].Pos.Add(e);
                                                CombTermsAr.TermsAr[k].frequency++;
                                            }
                                        }
                                    }
                                    else if (cur_pat == "CombTerm")
                                    {
                                        string word = xml.Value;
                                        Range cur_range = new Range(cur_pos);
                                        TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            int cur_Term = e.indexElement;
                                            if (find.findINList(CombTermsAr.TermsAr[cur_Term].Components, word) == -1)
                                            {
                                                CombComponent newEl = new CombComponent();
                                                newEl.TermWord = word;
                                                newEl.TermFragment = cur_fragment;
                                                newEl.frequency = 1;
                                                CombTermsAr.TermsAr[cur_Term].Components.Add(newEl);
                                            }
                                        }
                                    }
                                    else if (cur_pat == "NPFULLCombTerm")
                                    {
                                        string word = xml.Value;
                                        word = word.Replace('[', '<');
                                        word = word.Replace(']', '>');
                                        Range cur_range = new Range(cur_pos);
                                        TermTree e = CombTermsAr.rootTermsTree.FindRange(cur_range);
                                        if (e != null)
                                        {
                                            int k = e.indexElement;
                                            CombTermsAr.TermsAr[k].Pattern = word;
                                        }
                                    }
                                    else if (cur_pat == "NPCombTerm")
                                    {
                                        string word = xml.Value;
                                        word = word.Replace('[', '<');
                                        word = word.Replace(']', '>');
                                        if (curTermFind)
                                        {
                                            if (curTerm != -1 && previousPattern != word)
                                            {
                                                CombTermsAr.TermsAr[curTerm].Components[curComponent].Pattern = word;
                                                previousPattern = word;
                                                curComponent++;
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
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\IT_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            string text = "";
            string patternsName = "";
            string curPattern = "";
            curPattern = fs.ReadLine();
            patternsName = patternsName + " " + curPattern.Substring(0, curPattern.IndexOf('='));
            string prevPattern = curPattern.Substring(0, curPattern.IndexOf('='));
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                if (curPattern != "" && curPattern.Substring(0, curPattern.IndexOf('=')) != prevPattern)
                {
                    patternsName = patternsName + " " + curPattern.Substring(0, curPattern.IndexOf('='));
                    prevPattern = curPattern.Substring(0, curPattern.IndexOf('='));
                }
            }
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = "";
            switch (dictionary)
            {
                case Dictionary.IT_TERM:
                    {
                        LSPL_patterns = programmPath + "\\Patterns\\IT_TERM.txt";
                        break;
                    }
                case Dictionary.F_TERM:
                    {
                        LSPL_patterns = programmPath + "\\Patterns\\F_TERM.txt";
                        break;
                    }
            }
            string LSPL_output = tmpPath + "\\" + folderPath + "\\DictTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\DictTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patternsName);
            //Close the file
            sw.Close();
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetDictTerms(DictTermsAr);
            //---------------------------------
            return;
        }
        public bool GetDictTerms(Terms DictTermsAr)
        {            
            Point cur_pos = new Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + "\\" + folderPath + "\\DictTermsOutput.xml"))
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
                                                cur_pat = xml.Value;
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
                                                cur_pos.X = Convert.ToInt32(xml.Value);
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value);
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
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
                                        cur_fragment = xml.Value;
                                }
                                if (lastNodeName == "result")
                                {
                                    string word = xml.Value;
                                    int k = find.findINList(DictTermsAr.TermsAr, word);
                                    if (k == -1)
                                    {
                                        Term newEl = new Term();
                                        newEl.Pos.Add(null);
                                        newEl.TermWord = word;
                                        newEl.frequency = 1;
                                        newEl.setToDel = false;
                                        newEl.TermFragment = cur_fragment;
                                        Range cur_range = new Range(cur_pos);
                                        DictTermsAr.rootTermsTree.AddRange(cur_range);
                                        DictTermsAr.TermsAr.Add(newEl);
                                        TermTree e = DictTermsAr.rootTermsTree.FindRange(cur_range);
                                        e.indexElement = DictTermsAr.TermsAr.Count - 1;
                                        DictTermsAr.TermsAr[DictTermsAr.TermsAr.Count - 1].Pos[DictTermsAr.TermsAr[DictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                    }
                                    else
                                    {
                                        Range cur_range = new Range(cur_pos);
                                        if (DictTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                        {
                                            DictTermsAr.rootTermsTree.AddRange(cur_range);
                                            TermTree e = DictTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement = k;
                                            DictTermsAr.TermsAr[k].Pos.Add(e);
                                            DictTermsAr.TermsAr[k].frequency++;
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            aux.getFrequency_(DictTermsAr);
            return true;
        }

        //NonDictTerms
        public void GetXMLNonDictTerms(NonDictTerms NonDictTermsAr)
        {
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\NONDICT_TERM.txt", Encoding.GetEncoding("Windows-1251"));
            string text = "";
            string patternsName = "";
            string curPattern = "";
            curPattern = fs.ReadLine();
            patternsName = patternsName + " " + curPattern.Substring(0, curPattern.IndexOf('='));
            string prevPattern = curPattern.Substring(0, curPattern.IndexOf('='));
            while (true)
            {
                curPattern = fs.ReadLine();
                if (curPattern == null) break;
                if (curPattern != "" && curPattern.Substring(0, curPattern.IndexOf('=')) != prevPattern)
                {
                    patternsName = patternsName + " " + curPattern.Substring(0, curPattern.IndexOf('='));
                    prevPattern = curPattern.Substring(0, curPattern.IndexOf('='));
                }
            }
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = programmPath + "\\Patterns\\MSP_PAT.txt";
            string LSPL_output = tmpPath + "\\" + folderPath + "\\NontDictTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\NontDictTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" " + patternsName);
            //Close the file
            sw.Close();
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetNonDictTerms(NonDictTermsAr);
            //---------------------------------
            return;
        }
        public bool GetNonDictTerms(NonDictTerms NonDictTermsAr)
        {
            Point cur_pos = new Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            string module = "";
            bool Cpat = false;
            List<NonDictComponent> new_v = new List<NonDictComponent>();
            bool curTermFind = false;
            int curTerm = -1;
            using (XmlReader xml = XmlReader.Create(tmpPath + "\\" + folderPath + "\\NontDictTermsOutput.xml"))
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
                                                cur_pat = xml.Value;
                                                if (cur_pat[0] == 'C' || cur_pat[0] == 'F')
                                                {
                                                    if (curTermFind)
                                                    {
                                                        if (curTerm != -1)
                                                        {
                                                            if (find.findEqualVariants(NonDictTermsAr.TermsAr[curTerm], new_v) == -1)
                                                                NonDictTermsAr.TermsAr[curTerm].Components.Add(new_v);                                                                  
                                                        }
                                                        curTerm = -1;
                                                        curTermFind = false;
                                                        new_v = new List<NonDictComponent>();
                                                    }                                                    
                                                }                                                
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
                                                cur_pos.X = Convert.ToInt32(xml.Value);
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value);
                                            }
                                        }
                                    }
                                    if (cur_pat[0] == 'C')
                                    {
                                        Range cur_range = new Range(cur_pos);
                                        if (!curTermFind)
                                        {                                            
                                            TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e != null)
                                            {
                                                curTermFind = true;
                                                curTerm = e.indexElement;
                                            }
                                        }
                                        else
                                        {
                                            if (curTerm != -1)
                                            {
                                                if (find.findEqualVariants(NonDictTermsAr.TermsAr[curTerm], new_v) == -1)
                                                    NonDictTermsAr.TermsAr[curTerm].Components.Add(new_v);                                                
                                            }                                            
                                            TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e != null)
                                            {
                                                curTerm = e.indexElement;
                                                curTerm = -1;
                                                new_v = new List<NonDictComponent>();
                                            }
                                            else
                                                curTermFind = false;
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
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
                                        cur_fragment = xml.Value;
                                }
                                if (lastNodeName == "result")
                                {
                                    if (cur_pat[0] == 'F')
                                    {
                                        module = "";
                                        string word = xml.Value;
                                        int k = find.findINList(NonDictTermsAr.TermsAr, word);
                                        if (k == -1)
                                        {
                                            NonDictTerm newEl = new NonDictTerm();
                                            newEl.Pos.Add(null);
                                            newEl.TermWord = word;
                                            newEl.frequency = 1;
                                            newEl.TermFragment = cur_fragment;                                            
                                            Range cur_range = new Range(cur_pos);
                                            NonDictTermsAr.rootTermsTree.AddRange(cur_range);
                                            NonDictTermsAr.TermsAr.Add(newEl);
                                            TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement = NonDictTermsAr.TermsAr.Count - 1;
                                            NonDictTermsAr.TermsAr[NonDictTermsAr.TermsAr.Count - 1].Pos[NonDictTermsAr.TermsAr[NonDictTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                        }
                                        else
                                        {
                                            Range cur_range = new Range(cur_pos);
                                            if (NonDictTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                NonDictTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement = k;
                                                NonDictTermsAr.TermsAr[k].Pos.Add(e);
                                                NonDictTermsAr.TermsAr[k].frequency++;
                                            }
                                        }
                                    }
                                    else if (cur_pat[0] == 'C')
                                    {                                        
                                        string word = xml.Value;
                                        if (curTermFind)
                                        {
                                            if (find.findINListStr(new_v, word) == -1)
                                            {
                                                NonDictComponent new_El = new NonDictComponent();
                                                new_El.Component = word;
                                                new_v.Add(new_El);
                                            }
                                        }
                                    }
                                }

                            }
                            break;
                    }
                }
            }
            aux.getFrequency_(NonDictTermsAr);
            return true;
        }

        //SynTerms
        public void GetXMLSynTerms(SynTerms SynTermsAr)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = programmPath + "\\Patterns\\SYN_TERM.txt";
            string LSPL_output = tmpPath + "\\" + folderPath + "\\SynTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\SynTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" SYN");
            //Close the file
            sw.Close();
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetSynTerms(SynTermsAr);
            //---------------------------------
            return;
        }
        public bool GetSynTerms(SynTerms SynTermsAr)
        {
            Point cur_pos = new Point();
            string cur_pat = "";
            string cur_fragment = "";
            string lastNodeName = "";
            using (XmlReader xml = XmlReader.Create(tmpPath + "\\" + folderPath + "\\SynTermsOutput.xml"))
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
                                                cur_pat = xml.Value;
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
                                                cur_pos.X = Convert.ToInt32(xml.Value);
                                            }
                                            if (xml.Name == "endPos")
                                            {
                                                cur_pos.Y = Convert.ToInt32(xml.Value);
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "result")
                                {
                                    lastNodeName = xml.Name;
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
                                        cur_fragment = xml.Value;
                                }
                                if (lastNodeName == "result")
                                {
                                    string word = xml.Value;
                                    pair<string, string> alt = new pair<string, string>("", "");
                                    int y = word.IndexOf(" - ");
                                    if (y != -1)
                                    {
                                        alt.first = word.Substring(0, y);
                                        alt.second = word.Substring(y + 3, word.Length - (y+3));
                                        int k = find.findINList(SynTermsAr.TermsAr, alt);
                                        if (k == -1)
                                        {
                                            SynTerm newEl = new SynTerm();
                                            //добавляем алтернативы в список
                                            newEl.alternatives.first.alternative = alt.first;
                                            newEl.alternatives.second.alternative = alt.second;
                                            //вычисляем точные координаты альтернатив пары ------> нужно ли? если да дописать
                                            newEl.frequency = 1;
                                            newEl.setToDel = false;
                                            newEl.Pos.Add(null);
                                            newEl.TermFragment = cur_fragment;
                                            Range cur_range = new Range(cur_pos);
                                            SynTermsAr.rootTermsTree.AddRange(cur_range);
                                            SynTermsAr.TermsAr.Add(newEl);
                                            TermTree e = SynTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement = SynTermsAr.TermsAr.Count - 1;
                                            SynTermsAr.TermsAr[SynTermsAr.TermsAr.Count - 1].Pos[SynTermsAr.TermsAr[SynTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                        }
                                        else
                                        {
                                            Range cur_range = new Range(cur_pos);
                                            if (SynTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                SynTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = SynTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement = k;
                                                SynTermsAr.TermsAr[k].Pos.Add(e);
                                                SynTermsAr.TermsAr[k].frequency++;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            aux.getFrequency_(SynTermsAr);
            return true;

        }
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

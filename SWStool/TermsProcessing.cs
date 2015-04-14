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
    public class TermsProcessing
    {
        public Terms MainTermsAr;
        public Terms AuthTermsAr;
        public Terms DictTermsAr;
        public NonDictTerms NonDictTermsAr;
        public CombTerms CombTermsAr;
        public string tmpPath;
        public string programmPath;
        public string folderPath;
        public string inputFile;
        public string outputFile;
        FindFunctions find;
        AuxiliaryFunctions aux;
        public TermsProcessing(string inputfile)
        {
            find = new FindFunctions();
            aux = new AuxiliaryFunctions();
            folderPath = "TermsProcessingF";
            outputFile = "";
            inputFile = inputfile;
            tmpPath = Path.GetTempPath();
            programmPath = Application.StartupPath.ToString();
            MainTermsAr = new Terms();
            AuthTermsAr = new Terms();
            DictTermsAr = new Terms();
            NonDictTermsAr = new NonDictTerms();
            CombTermsAr = new CombTerms();
            Directory.CreateDirectory(tmpPath + "\\" + folderPath);
        }
        //AuthTerms
        public void GetXMLAuthTerms(Terms AuthTermsAr)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = programmPath + "\\Patterns\\AUTH_TERM.txt";
            string LSPL_output = tmpPath + "\\" + folderPath + "\\AuthTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\AuthTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" Definition SeparateDefinition");
            //Close the file
            sw.Close();
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
                                    if (cur_pat == "Definition")
                                    {
                                        string word = xml.Value;
                                        int k = find.findINList(AuthTermsAr.TermsAr, word);
                                        if (k == -1)
                                        {
                                            //Point pos;
                                            //pos.first = stoi(xml_match->Attribute("startPos"));
                                            //pos.second = stoi(xml_match->Attribute("endPos"));
                                            cur_pos = aux.GetRealPos(cur_fragment, word, cur_pos);
                                            Term newEl = new Term();
                                            newEl.TermWord = word;
                                            newEl.frequency = 1;
                                            newEl.setToDel = false;
                                            newEl.Pos.Add(null);
                                            newEl.TermFragment.Add(cur_fragment);
                                            Range cur_range = new Range(cur_pos);
                                            AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                            AuthTermsAr.TermsAr.Add(newEl);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement = AuthTermsAr.TermsAr.Count - 1;
                                            AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                        }
                                        else
                                        {
                                            //pair<unsigned, unsigned> pos;
                                            //pos.first = stoi(xml_match->Attribute("startPos"));
                                            //pos.second = stoi(xml_match->Attribute("endPos"));
                                            cur_pos = aux.GetRealPos(cur_fragment, word, cur_pos);
                                            Range cur_range = new Range(cur_pos);
                                            if (AuthTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement = k;
                                                AuthTermsAr.TermsAr[k].Pos.Add(e);
                                                AuthTermsAr.TermsAr[k].frequency++;
                                            }
                                        }
                                    }
                                    else if (cur_pat == "SeparateDefinition")
                                    {

                                        string word = xml.Value;
                                        int k = find.findINList(AuthTermsAr.TermsAr, word);
                                        if (k == -1)
                                        {
                                            //pair<unsigned, unsigned> pos;
                                            //pos.first = stoi(xml_match->Attribute("startPos"));
                                            //pos.second = stoi(xml_match->Attribute("endPos"));
                                            //Решить задачу координат для разрывных конструкций
                                            //pos = GetRealPos(xml_fragment->GetText(), word, pos);
                                            Term newEl = new Term();
                                            newEl.TermWord = word;
                                            newEl.frequency = 1;
                                            newEl.setToDel = false;
                                            newEl.Pos.Add(null);
                                            newEl.TermFragment.Add(cur_fragment);
                                            Range cur_range = new Range(cur_pos);
                                            AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                            AuthTermsAr.TermsAr.Add(newEl);
                                            TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                            e.indexElement = AuthTermsAr.TermsAr.Count - 1;
                                            AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos[AuthTermsAr.TermsAr[AuthTermsAr.TermsAr.Count - 1].Pos.Count - 1] = e;
                                        }
                                        else
                                        {
                                            //pair<unsigned, unsigned> pos;
                                            //pos.first = stoi(xml_match->Attribute("startPos"));
                                            // pos.second = stoi(xml_match->Attribute("endPos"));
                                            //Решить задачу координат для разрывных конструкций
                                            //pos = GetRealPos(xml_fragment->GetText(), word, pos);
                                            Range cur_range = new Range(cur_pos);
                                            if (AuthTermsAr.rootTermsTree.FindRange(cur_range) == null)
                                            {
                                                AuthTermsAr.rootTermsTree.AddRange(cur_range);
                                                TermTree e = AuthTermsAr.rootTermsTree.FindRange(cur_range);
                                                e.indexElement = k;
                                                AuthTermsAr.TermsAr[k].Pos.Add(e);
                                                AuthTermsAr.TermsAr[k].frequency++;
                                            }
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
        public void GetXMLCombComponentsTerms(CombTerms CombTermsAr)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = programmPath + "\\Patterns\\COMP_COMB_PAT.txt";
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
        public void GetXMLCombTerms(CombTerms CombTermsAr)
        {
            //--------------------------------
            string LSPL_exe = programmPath + "\\bin\\lspl-find.exe";
            string LSPL_patterns = programmPath + "\\Patterns\\NS_COMB_PAT.txt";
            string LSPL_output = tmpPath + "\\" + folderPath + "\\CombTermsOutput.xml";
            string BAT_output = tmpPath + "\\" + folderPath + "\\CombTerms.bat";
            StreamWriter sw = new StreamWriter(BAT_output, false, Encoding.GetEncoding("cp866"));
            //Write a line of text
            //sw.WriteLine("cd \"" + programmPath + "\"");
            //Write a second line of text
            sw.WriteLine("\"" + LSPL_exe + "\" -i \"" + inputFile + "\" -p \"" + LSPL_patterns + "\" -o \"" + LSPL_output + "\" FULLCombTerm CombTerm");
            //Close the file
            sw.Close();
            System.Diagnostics.Process.Start(BAT_output).WaitForExit();
            GetCombTerms(CombTermsAr);            
            GetXMLCombComponentsTerms(CombTermsAr);
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
                                            newEl.TermFragment.Add(cur_fragment);
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
                            CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.InsertRange(CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.Count + 1, CompV[0].Pos);
                            CombTermsAr.TermsAr[curTerm].Components[curComp].frequency = CombTermsAr.TermsAr[curTerm].Components[curComp].Pos.Count;
                        }
                        Range l = new Range(CompV[0].Pos[i]);
                        if (l <= e.range)
                            e = e.left.FindRangeExtension(l);
                        else
                            e = e.right.FindRangeExtension(l);
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
                                            newEl.TermFragment.Add(cur_fragment);
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
                                            int curTerm = e.indexElement;
                                            if (find.findINList(CombTermsAr.TermsAr[curTerm].Components, word) == -1)
                                            {
                                                CombComponent newEl = new CombComponent();
                                                newEl.TermWord = word;
                                                newEl.TermFragment.Add(cur_fragment);
                                                newEl.frequency = 1;
                                                CombTermsAr.TermsAr[curTerm].Components.Add(newEl);
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
            string LSPL_patterns = programmPath + "\\Patterns\\IT_TERM.txt";
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
                                        newEl.TermFragment.Add(cur_fragment);
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
            StreamReader fs = new StreamReader(programmPath + "\\Patterns\\MSP_PAT.txt", Encoding.GetEncoding("Windows-1251"));
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
            List<string> new_v = new List<string>();
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
                                                if (cur_pat[0] == 'C')
                                                    Cpat = true;
                                                if (cur_pat[0] == 'F')
                                                {
                                                    if (curTerm != -1)
                                                    {                                                        
                                                        if (find.findEqualVariants(NonDictTermsAr.TermsAr[curTerm], new_v) == -1)
                                                            NonDictTermsAr.TermsAr[curTerm].Components.Add(new_v);
                                                        curTerm = -1;
                                                        curTermFind = false;
                                                        new_v.Clear();
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
                                            newEl.TermFragment.Add(cur_fragment);
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
                                        TermTree e = null;
                                        if (!curTermFind)
                                        {
                                            Range cur_range = new Range(cur_pos);
                                            e = NonDictTermsAr.rootTermsTree.FindRange(cur_range);
                                            if (e != null)
                                            {
                                                curTermFind = true;
                                                curTerm = e.indexElement;
                                            }
                                        }
                                        string word = xml.Value;
                                        if (find.findINListStr(new_v, word) == -1)
                                        {
                                            new_v.Add(word);
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
            string LSPL_patterns = programmPath + "\\Patterns\\SYN_PAT.txt";
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
                                            newEl.alternatives.first = alt.first;
                                            newEl.alternatives.second = alt.second;
                                            //вычисляем точные координаты альтернатив пары ------> нужно ли? если да дописать
                                            newEl.frequency = 1;
                                            newEl.setToDel = false;
                                            newEl.Pos.Add(null);
                                            newEl.TermFragment.Add(cur_fragment);
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

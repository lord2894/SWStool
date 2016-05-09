using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Collections.ObjectModel;
//using System.Windows.Forms;
using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Novacode;
using RulesNamespace;
using TermProcessingNamespace;
using TermsNamespace;
using IndexAndGlossaryNamespace;

namespace SWStool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            tmpPath = System.IO.Path.GetTempPath();
            Directory.CreateDirectory(tmpPath + "\\" + folderPath);
            ProgrammTmpPath = tmpPath + "\\" + folderPath;
            //StartButton.IsEnabled = false;
        }

        public string tmpPath = "";
        public string folderPath = "SWStoolTmp";
        public string ProgrammTmpPath = "";
        // Открытие документа в DocumentView-----------------------------------------------------
        public string textFileName = "";
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".docx";
            dlg.Filter = "Word documents (.docx)|*.docx";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a DocumentView
            if (result == true)
            {
                if (dlg.FileName.Length > 0)
                {
                    textFileName = dlg.FileName;
                    //SelectedFileTextBox.Text = dlg.FileName;
                    //string newXPSDocumentName = String.Concat(System.IO.Path.GetDirectoryName(dlg.FileName), "\\",
                    //               System.IO.Path.GetFileNameWithoutExtension(dlg.FileName), ".xps");
                    string newXPSDocumentName = String.Concat(ProgrammTmpPath, "\\", System.IO.Path.GetFileNameWithoutExtension(dlg.FileName), ".xps");
                    // Set DocumentViewer.Document to XPS document
                    DocViewer.Document = ConvertWordDocToXPSDoc(dlg.FileName, newXPSDocumentName).GetFixedDocumentSequence();
                }
            }
            StartButton.IsEnabled = true;
        }
        private XpsDocument ConvertWordDocToXPSDoc(string wordDocName, string xpsDocName)
        {
            // Create a WordApplication and add Document to it
            Microsoft.Office.Interop.Word.Application
                wordApplication = new Microsoft.Office.Interop.Word.Application();
            wordApplication.Documents.Add(wordDocName);


            Document doc = wordApplication.ActiveDocument;
            // You must ensure you have Microsoft.Office.Interop.Word.Dll version 12.
            // Version 11 or previous versions do not have WdSaveFormat.wdFormatXPS option
            try
            {
                doc.SaveAs(xpsDocName, WdSaveFormat.wdFormatXPS);
                wordApplication.Quit();

                XpsDocument xpsDoc = new XpsDocument(xpsDocName, System.IO.FileAccess.Read);
                return xpsDoc;
            }
            catch (Exception exp)
            {
                string str = exp.Message;
            }
            return null;
        }
        //---------------------------------------------------------------------------------------

        // Постраничное извлечение текста из Docx документа -------------------------------------
       

        //---------------------------------------------------------------------------------------
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
        //public class GlossaryItem
        //{
        //    public string Term { get; set; }
        //    public string Definition { get; set; }
        //}

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {         
                      
            //Rules rules = new Rules(textFileName, DictionaryF.IT_TERM,0, 5, true);      
            //Rules rules = new Rules("C:\\Users\\Kir\\Desktop\\test.docx", DictionaryF.IT_TERM, 1, 5, true);
            //Terms mainTermsAr = rules.ApplyRules();Glava3N_3.docx
            IndexAndGlossary getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Пособие_Лисп_финал.docx", DictionaryF.F_TERM, 2, 108, true);
            //IndexAndGlossary getter = new IndexAndGlossary("C:\\Users\\Kir\\Desktop\\Test\\Glava3N_3.docx", DictionaryF.F_TERM, 1, 15, true);
            List<GlossaryItem> glossary = getter.GetGlossary();
            ObservableCollection<GlossaryItem> coll = new ObservableCollection<GlossaryItem>(glossary);
            GlossaryGrid.ItemsSource = coll;
            GlossaryGrid.Items.Refresh();
            List<IndexItem> index = getter.GetIndex();
            TreeViewItem curLetterBlock = new TreeViewItem();
	        curLetterBlock.Header = index[0].term[0].ToString().ToUpper();
	        //curLetterBlock.ItemsSource = new string[] { "Monitor", "CPU", "Mouse" };
            foreach(IndexItem item in index)
            {
                if (item.term[0].ToString().ToUpper().CompareTo(curLetterBlock.Header)!=0)
                {
                    IdexTreeView.Items.Add(curLetterBlock);
                    curLetterBlock = new TreeViewItem();
                    curLetterBlock.Header = item.term[0].ToString().ToUpper();
                }
                TreeViewItem newTerm = new TreeViewItem();
                newTerm.Header = item.term;
                newTerm.ItemsSource = item.supportTerms;
                curLetterBlock.Items.Add(newTerm);
            }
            IdexTreeView.Items.Add(curLetterBlock);
            IdexTreeView.Items.Refresh();
        }
    }
}



//StringBuilder text = new StringBuilder();  
//ReadTextFromDocxByPage(@"C:\Users\Kir\Desktop\Документы\Отчет Иванов.docx");
//string inputFile = ProgrammTmpPath + "\\TextA.txt";
//            try
//            {
//                using (StreamWriter sw = new StreamWriter(inputFile, false, System.Text.Encoding.GetEncoding("Windows-1251")))
//                {
//                    sw.WriteLine(text);
//                    sw.Close();
//                }
//            }
//            catch (Exception exp)
//            {
//                using (StreamWriter sw = new StreamWriter(ProgrammTmpPath + "\\ExeptionLog.txt", false, System.Text.Encoding.GetEncoding("Windows-1251")))
//                {
//                    sw.WriteLine(exp.Message);
//                    sw.Close();
//                }
//            }
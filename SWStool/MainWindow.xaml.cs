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
using Novacode;
using System.IO;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using System.Windows.Xps.Packaging;
using System.Collections.ObjectModel;

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

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //// Create OpenFileDialog
            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //// Set filter for file extension and default file extension
            //dlg.DefaultExt = ".docx";
            //dlg.Filter = "Microsoft Word (.docx)|*.docx";

            //// Display OpenFileDialog by calling ShowDialog method
            //Nullable<bool> result = dlg.ShowDialog();

            //// Get the selected file name and display in a TextBox
            //if (result == true)
            //{
            //    // Open document
            //    string filename = dlg.FileName;
            //    //var doc = DocX.Load(filename);
            //    //DocViewer.Document = null;
            //    //ListBoxT.Items.IndexOf(ListBoxT.SelectedItems[i]);
            //    var document = DocumentModel.Load("Document.docx", LoadOptions.DocxDefault);

            //    // Convert DOCX to PDF.
            //    document.Save("Document.pdf");

            //    // View document in WPF's DocumentViewer.
            //    documentViewer.Document = document.ConvertToXpsDocument(SaveOptions.XpsDefault).GetFixedDocumentSequence();

            //}

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".doc";
            dlg.Filter = "Word documents (.docx)|*.docx";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                if (dlg.FileName.Length > 0)
                {
                    //SelectedFileTextBox.Text = dlg.FileName;
                    string newXPSDocumentName = String.Concat(System.IO.Path.GetDirectoryName(dlg.FileName), "\\",
                                   System.IO.Path.GetFileNameWithoutExtension(dlg.FileName), ".xps");

                    // Set DocumentViewer.Document to XPS document
                    DocViewer.Document =
                        ConvertWordDocToXPSDoc(dlg.FileName, newXPSDocumentName).GetFixedDocumentSequence();
                }
            }
            ObservableCollection<GlossaryItem> coll = new ObservableCollection<GlossaryItem>();
            coll.Add(new GlossaryItem() { Term = "Порт", Definition = "Порт – это некая схема сопряжения." });
            coll.Add(new GlossaryItem() { Term = "Перефирийное устройство", Definition = "Устройство, совершающее по отношению к микропроцессору операции ввода-вывода, можно назвать периферийным." });
            GlossaryGrid.ItemsSource = coll;
            GlossaryGrid.Items.Refresh();
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
        public class GlossaryItem
        {
            public string Term { get; set; }
            public string Definition { get; set; }
        }
    }
}

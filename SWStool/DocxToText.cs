using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;
using System.IO;

namespace SWStool
{
    public class DocxToText
    {
        private const string ContentTypeNamespace =
            @"http://schemas.openxmlformats.org/package/2006/content-types";

        private const string WordprocessingMlNamespace =
            @"http://schemas.openxmlformats.org/wordprocessingml/2006/main";

        private const string DocumentXmlXPath =
            "/t:Types/t:Override[@ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"]";

        private const string BodyXPath = "/w:document/w:body";

        private string docxFile = "";
        private string docxFileLocation = "";


        public DocxToText(string fileName)
        {
            docxFile = fileName;
        }

        #region ExtractText()
        /// <summary>
        /// Extracts text from the Docx file.
        /// </summary>
        /// <returns>Extracted text.</returns>
        //public string ExtractText()
        public void ExtractText()
        {
            if (string.IsNullOrEmpty(docxFile))
                throw new Exception("Input file not specified.");

            // Usually it is "/word/document.xml"

            docxFileLocation = FindDocumentXmlLocation();

            if (string.IsNullOrEmpty(docxFileLocation))
                throw new Exception("It is not a valid Docx file.");
            ReadDocumentXml();

            return; 
        }
        #endregion

        #region FindDocumentXmlLocation()
        /// <summary>
        /// Gets location of the "document.xml" zip entry.
        /// </summary>
        /// <returns>Location of the "document.xml".</returns>
        private string FindDocumentXmlLocation()
        {
            ZipFile zip = new ZipFile(docxFile);
            foreach (ZipEntry entry in zip)
            {
                // Find "[Content_Types].xml" zip entry

                if (string.Compare(entry.Name, "[Content_Types].xml", true) == 0)
                {
                    Stream contentTypes = zip.GetInputStream(entry);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(contentTypes);
                    contentTypes.Close();

                    //Create an XmlNamespaceManager for resolving namespaces

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("t", ContentTypeNamespace);

                    // Find location of "document.xml"

                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(DocumentXmlXPath, nsmgr);

                    if (node != null)
                    {
                        string location = ((XmlElement)node).GetAttribute("PartName");
                        return location.TrimStart(new char[] { '/' });
                    }
                    break;
                }
            }
            zip.Close();
            return null;
        }
        #endregion

        #region ReadDocumentXml()
        /// <summary>
        /// Reads "document.xml" zip entry.
        /// </summary>
        /// <returns>Text containing in the document.</returns>
        //private string ReadDocumentXml()
        private void ReadDocumentXml()
        {
            StringBuilder sb = new StringBuilder();

            ZipFile zip = new ZipFile(docxFile);
            foreach (ZipEntry entry in zip)
            {
                if (string.Compare(entry.Name, docxFileLocation, true) == 0)
                {
                    Stream documentXml = zip.GetInputStream(entry);

                    ReadXml(documentXml);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(documentXml);
                    documentXml.Close();

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                    nsmgr.AddNamespace("w", WordprocessingMlNamespace);

                    XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(BodyXPath, nsmgr);

                    if (node == null)
                        //return string.Empty;
                        return;

                    sb.Append(ReadNode(node));

                    break;
                }
            }
            zip.Close();
            //return sb.ToString();
            return;
        }
        #endregion

        #region ReadNode()
        /// <summary>
        /// Reads content of the node and its nested childs.
        /// </summary>
        /// <param name="node">XmlNode.</param>
        /// <returns>Text containing in the node.</returns>
        private void ReadXml(Stream xmlStream)
        {
            string pageText = null;
            List<string> pages = new List<string>();
            string lastNodeName = "";
            string space = null;
            using (XmlReader xml = XmlReader.Create(xmlStream))
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xml.NameTable);
                nsmgr.AddNamespace("w", WordprocessingMlNamespace);
                while (xml.Read())
                {
                    switch (xml.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                if (xml.Name == "w:t")
                                {
                                    lastNodeName = xml.Name;
                                    if (xml.HasAttributes)
                                    {
                                        while (xml.MoveToNextAttribute())
                                        {
                                            if (xml.Name == "xml:space")
                                            {
                                                space = xml.Value;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (xml.Name == "w:br")
                                {
                                    lastNodeName = xml.Name;
                                    while (xml.MoveToNextAttribute())
                                    {
                                        if (xml.Name == "w:type")
                                        {
                                            if (xml.Value == "page")
                                                if (!string.IsNullOrEmpty(pageText))
                                                {
                                                    pages.Add(pageText);
                                                    pageText = null;
                                                    break;
                                                }
                                        }
                                    }
                                }
                                else if (xml.Name == "w:lastRenderedPageBreak")
                                {
                                    lastNodeName = xml.Name;
                                    if (!string.IsNullOrEmpty(pageText))
                                    {
                                        pages.Add(pageText);                                    
                                        pageText = null;
                                    }
                                }
                                else if (xml.Name == "w:tab")
                                {
                                    lastNodeName = xml.Name;
                                }
                                else if (xml.Name == "w:p")
                                {
                                    pageText += Environment.NewLine;
                                    pageText += Environment.NewLine;
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            {
                                if (lastNodeName == "w:t")
                                {
                                    pageText += xml.Value.Trim();
                                    if (!string.IsNullOrEmpty(space) && space == "preserve")
                                    {
                                        pageText += ' ';
                                        space = null;
                                    }                                    
                                }
                                else if (lastNodeName == "w:tab")
                                {
                                    pageText += "\t";
                                }
                                break;
                            }
                    }
                }
                if (!string.IsNullOrEmpty(pageText))
                    pages.Add(pageText);
                pageText = null;
            }
        }
        private string ReadNode(XmlNode node)
        {
            if (node == null || node.NodeType != XmlNodeType.Element)
                return string.Empty;
            List<string> pages = new List<string>();
            string pageText = "";
            StringBuilder sb = new StringBuilder();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element) continue;
                switch (child.LocalName)
                {
                    case "t":                           // Text
                        //sb.Append(child.InnerText.TrimEnd());
                        pageText += child.InnerText.TrimEnd();
                        string space = ((XmlElement)child).GetAttribute("xml:space");
                        if (!string.IsNullOrEmpty(space) && space == "preserve")
                            //sb.Append(' ');
                            pageText += ' ';

                        break;

                    case "cr":                          // Carriage return
                    case "br":                          // Page break
                        sb.Append(Environment.NewLine);
                        break;

                    case "tab":                         // Tab
                        //sb.Append("\t");
                        pageText += "\t";
                        break;

                    case "p":                           // Paragraph
                        //sb.Append(ReadNode(child));
                        //sb.Append(Environment.NewLine);
                        //sb.Append(Environment.NewLine);
                        //pageText += ReadNode(child)
                        break;

                    default:
                        sb.Append(ReadNode(child));
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}


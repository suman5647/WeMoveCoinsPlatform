using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WMC.Web.Tests.WMC.Data.Reader
{
    [TestClass]
    public class CSVFileReaderTest
    {

        [TestMethod]
        public void Test4()
        {
            int sourceType = 4;
            string file1 = @"C:\Users\dell\Desktop\wmc_2\UN_Sanctions_List.xml";
            // Stream fs = new FileStream(file1, FileMode.Open);
            using (IWMCDataTeader reader = BaseWMCDataTeader.Create(sourceType, file1))
            {
                while (reader.Read())
                {
                    var csvRow = reader.CurrentRow;
                    // var rowValue = string.Format("LN:{14} | FN:{15} | MN:{16} | FN:{17}", row);
                }
            }
        }


        [TestMethod]
        public void Test2()
        {
            int sourceType = 1;
            string file1 = @"C:\Users\dell\Desktop\wmc_2\EU_Fonancial_Sanctions_Files.csv";
            // Stream fs = new FileStream(file1, FileMode.Open);
            using (IWMCDataTeader reader = BaseWMCDataTeader.Create(sourceType, file1))
            {
                while (reader.Read())
                {
                    var csvRow = reader.CurrentRow;
                    // var rowValue = string.Format("LN:{14} | FN:{15} | MN:{16} | FN:{17}", row);
                }
            }
        }
        [TestMethod]
        public void Test1()
        {
            string file1 = @"C:\Users\dell\Desktop\wmc_2\EU_Fonancial_Sanctions_Files.csv";
            using (IWMCDataTeader reader = new EUFinancialSanctionsFileReader(file1))
            {
                while (reader.Read())
                {
                    var csvRow = reader.CurrentRow;
                    // var rowValue = string.Format("LN:{14} | FN:{15} | MN:{16} | FN:{17}", row);
                }
            }

            file1 = @"C:\Users\dell\Desktop\wmc_2\HMT_-_UK_Financial_Sanctions.csv";
            using (IWMCDataTeader reader = new HMTUKFinancialSanctionsFileReader(file1))
            {
                while (reader.Read())
                {
                    var csvRow = reader.CurrentRow;
                    // var rowValue = string.Format("N6:{0} | N1:{1} | N2:{2} | N3:{3} | N4:{4} | N5:{5} | TT:{6} | CR?:{9}", row);
                }
            }
        }
    }

    public interface IWMCDataTeader : IDisposable
    {
        bool Read();
        bool IsValid();
        /// <summary>
        /// N1,N2,N3,N4,N5,N6,DOB,COR,Summary,FromSource
        /// </summary>
        object[] CurrentRow { get; }
    }

    public abstract class BaseWMCDataTeader : IWMCDataTeader
    {
        public abstract object[] CurrentRow { get; }

        public abstract bool IsValid();

        public abstract bool Read();

        public static IWMCDataTeader Create(int sourceType, string file)
        {
            switch (sourceType)
            {
                case 1:
                    return new EUFinancialSanctionsFileReader(file);
                case 2:
                    return new HMTUKFinancialSanctionsFileReader(file);
                case 3:
                    return new OFACSanctionsFileReader(file);
                case 4:
                    return new UNSanctionsFileReader(file);
                default:
                    return default(IWMCDataTeader);
            }
        }

        public abstract void Dispose();
    }

    public abstract class BaseCSVFileReader : BaseWMCDataTeader, IDisposable
    {
        protected readonly TextFieldParser csvParser;
        protected string[] Current;
        public readonly string[] Columns;

        public BaseCSVFileReader(string filePath, string delimitter = ";", int headderCol = 0, int skipLines = 0)
        {
            this.csvParser = new TextFieldParser(filePath);
            // csvParser.CommentTokens = new string[] { "#" };
            csvParser.SetDelimiters(new string[] { delimitter });
            csvParser.HasFieldsEnclosedInQuotes = true;
            int skipColsIndex = 0;
            if (headderCol >= 0)
            {
                skipColsIndex = 0;
                while (headderCol >= skipColsIndex)
                {
                    if (headderCol == skipColsIndex)
                    {
                        this.Columns = csvParser.ReadFields();
                        break;
                    }
                    else
                    {
                        csvParser.ReadFields();
                    }

                    skipColsIndex++;
                }
            }

            skipColsIndex = 0;
            while (skipLines < skipColsIndex)
            {
                csvParser.ReadFields();
                skipColsIndex++;
            }
        }

        public override bool IsValid()
        {
            return true;
        }

        public override bool Read()
        {
            while (!csvParser.EndOfData)
            {
                this.Current = csvParser.ReadFields();
                return true;
                //if (IsValid())
                //{
                //    yield return this.Current;
                //}
            }

            return false;
        }

        public override object[] CurrentRow
        {
            get
            {
                return this.Current;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                csvParser.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                disposedValue = true;
            }
        }

        ~BaseCSVFileReader()
        {
            Dispose(false);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class EUFinancialSanctionsFileReader : BaseCSVFileReader, IWMCDataTeader
    {
        public EUFinancialSanctionsFileReader(string filePath)
            : base(filePath, ";", 0, 0)
        {
        }

        public override bool IsValid()
        {
            // if fullname has value, considered correct and valid
            return !string.IsNullOrEmpty(this.Current[17]);
        }

        public override object[] CurrentRow
        {
            get
            {
                return new object[] { this.Current[17], this.Current[14], this.Current[15], this.Current[16], null, null, this.Current[40], null, string.Join(",", this.Current), 1 };
            }
        }
    }
    public class HMTUKFinancialSanctionsFileReader : BaseCSVFileReader, IWMCDataTeader
    {
        public HMTUKFinancialSanctionsFileReader(string filePath)
            : base(filePath, ",", 1, 1)
        {
        }

        public override bool IsValid()
        {
            return true;
        }
    }
    public class OFACSanctionsFileReader : BaseCSVFileReader, IWMCDataTeader
    {
        public OFACSanctionsFileReader(string filePath)
            : base(filePath, ",", -1, -1)
        {
        }

        public override bool IsValid()
        {
            // if fullname has value, considered correct and valid
            if (!string.IsNullOrEmpty(this.Current[2]) && this.Current[2] == "individual")
            {
                return true;
            }

            return false;
        }
    }

    public abstract class BaseXMLFileReader : BaseWMCDataTeader, IDisposable
    {
        bool hasNext = false;
        IEnumerator xnodeEnmerator;
        XmlNodeList xnodes;
        XmlNode xcurrentNode;
        protected string[] Current = null;
        protected List<string> Columns = null;
        protected readonly List<string> columnPaths = null;

        public BaseXMLFileReader(string filePath, string xpath = "", List<string> columnPaths = null)
        {
            if (columnPaths != null && columnPaths.Count > 0)
            {
                Columns = columnPaths;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            xnodes = doc.SelectNodes(xpath);
            xnodeEnmerator = xnodes.GetEnumerator();
            this.MoveNext();
        }

        public override bool IsValid()
        {
            return true;
        }

        protected void MoveNext()
        {
            if (!hasNext) xcurrentNode = null;

            hasNext = xnodeEnmerator.MoveNext();
            if (hasNext)
            {
                xcurrentNode = xnodeEnmerator.Current as XmlNode;
            }
            else
            {
                xcurrentNode = default;
            }
        }

        protected List<string> ReadFields()
        {
            if (!hasNext) return default;

            var childNodes = xcurrentNode.ChildNodes.OfType<XmlNode>().OrderBy(x => x.Name);
            if (Columns == null)
            {
                Columns = new List<string>();
                foreach (XmlNode curNode in childNodes)
                {
                    Columns.Add(curNode.Name);
                }
            }

            List<string> current = new List<string>();
            foreach (string column in Columns)
            {
                XmlNodeList curNode = xcurrentNode.SelectNodes(column);
                if (curNode == null)
                {
                    current.Add(null);
                }
                else
                {
                    if (curNode.Count > 1)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (XmlNode xnode in curNode)
                        {
                            sb.Append(xnode.InnerText);
                            sb.Append(",");
                        }

                        sb.Remove(sb.Length - 2, 1); // last charecter ','
                        current.Add(sb.ToString());
                    }
                    else if (curNode.Count > 0)
                    {
                        current.Add(curNode[0].InnerText);
                    }
                    else
                    {
                        current.Add(null);
                    }
                }
            }

            return current;
        }

        public override bool Read()
        {
            if (xcurrentNode != null)
            {
                this.Current = ReadFields().ToArray();
                this.MoveNext();
                return true;
                //if (IsValid())
                //{
                //    yield return this.Current;
                //}
            }

            return hasNext;
        }

        public override object[] CurrentRow
        {
            get
            {
                return this.Current;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // this.xnodes.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                disposedValue = true;
            }
        }

        ~BaseXMLFileReader()
        {
            Dispose(false);
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class UNSanctionsFileReader : BaseXMLFileReader, IWMCDataTeader
    {
        public UNSanctionsFileReader(string filePath)
            : base(filePath, "//CONSOLIDATED_LIST/INDIVIDUALS/INDIVIDUAL",
                  new List<string>() {
                    "DATAID", // 0
                    "VERSIONNUM", // 1
                    "FIRST_NAME", // 2
                    "SECOND_NAME", // 3
                    "THIRD_NAME", // 4
                    "FOURTH_NAME", // 5
                    "UN_LIST_TYPE", // 6
                    "REFERENCE_NUMBER", // 7
                    "LISTED_ON",  // 8 // date
                    "SUBMITTED_BY", // 9
                    "GENDER", // 10
                    "NAME_ORIGINAL_SCRIPT", // 11
                    "COMMENTS1", // 12
                    "TITLE/VALUE", // 13
                    "DESIGNATION/VALUE", // 14
                    "DESIGNATION/VALUE", // 15
                    "NATIONALITY/VALUE", // 16
                    "LIST_TYPE/VALUE", // 17
                    "LAST_DAY_UPDATED/VALUE", // 18
                    "INDIVIDUAL_ALIAS/QUALITY", // 19
                    "INDIVIDUAL_ALIAS/ALIAS_NAME", // 20
                    "INDIVIDUAL_ALIAS/DATE_OF_BIRTH", // 21
                    "INDIVIDUAL_ALIAS/CITY_OF_BIRTH", // 22
                    "INDIVIDUAL_ALIAS/COUNTRY_OF_BIRTH", // 23
                    "INDIVIDUAL_ALIAS/NOTE", // 24
                    "INDIVIDUAL_ADDRESS/STREET", // 25
                    "INDIVIDUAL_ADDRESS/CITY", // 26
                    "INDIVIDUAL_ADDRESS/ZIP_CODE", // 27
                    "INDIVIDUAL_ADDRESS/STATE_PROVINCE", // 28
                    "INDIVIDUAL_ADDRESS/COUNTRY", // 29
                    "INDIVIDUAL_ADDRESS/NOTE", // 30
                    "INDIVIDUAL_DATE_OF_BIRTH/TYPE_OF_DATE", // 31
                    "INDIVIDUAL_DATE_OF_BIRTH/NOTE", // 32
                    "INDIVIDUAL_DATE_OF_BIRTH/FROM_YEAR", // 33
                    "INDIVIDUAL_DATE_OF_BIRTH/TO_YEAR", // 34
                    "INDIVIDUAL_DATE_OF_BIRTH/YEAR", // 35
                    "INDIVIDUAL_DATE_OF_BIRTH/DATE", // 36
                    "INDIVIDUAL_PLACE_OF_BIRTH/CITY", // 37
                    "INDIVIDUAL_PLACE_OF_BIRTH/STATE_PROVINCE", // 38
                    "INDIVIDUAL_PLACE_OF_BIRTH/NOTE", // 39
                    "INDIVIDUAL_PLACE_OF_BIRTH/COUNTRY", // 40
                    "INDIVIDUAL_DOCUMENT/TYPE_OF_DOCUMENT", // 41
                    "INDIVIDUAL_DOCUMENT/TYPE_OF_DOCUMENT2", // 42
                    "INDIVIDUAL_DOCUMENT/NUMBER", // 43
                    "INDIVIDUAL_DOCUMENT/ISSUING_COUNTRY", // 44
                    "INDIVIDUAL_DOCUMENT/DATE_OF_ISSUE", // 45
                    "INDIVIDUAL_DOCUMENT/CITY_OF_ISSUE", // 46
                    "INDIVIDUAL_DOCUMENT/COUNTRY_OF_ISSUE", // 47
                    "INDIVIDUAL_DOCUMENT/NOTE", // 48
                    "SORT_KEY", // 49
                    "SORT_KEY_LAST_MOD", // 50
                  })
        {
        }

        public override bool IsValid()
        {
            // if fullname has value, considered correct and valid
            return true; // !string.IsNullOrEmpty(this.Current[17]);
        }

        /// <summary>
        /// N1,N2,N3,N4,N5,N6,DOB,COR,Summary,FromSource
        /// </summary>
        public override object[] CurrentRow
        {
            get
            {
                return new object[] { this.Current[2], this.Current[3], this.Current[4], this.Current[5], null, null,
                    this.Current[36], this.Current[23], string.Join(",", this.Current), 4 };
            }
        }
    }
}

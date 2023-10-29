using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace WMC.Logic
{
    public abstract class BaseXMLFileReader : SanctionList.SanctionListDataReader, IDisposable
    {
        private bool hasNext = false;
        private IEnumerator xnodeEnmerator;
        private XmlNodeList xnodes;
        private XmlNode xcurrentNode;
        protected string[] Current = null;
        protected List<string> Columns = null;
        protected readonly List<string> columnPaths = null;

        public BaseXMLFileReader(Stream file, string xpath = "", List<string> columnPaths = null)
        {
            if (columnPaths != null && columnPaths.Count > 0)
            {
                Columns = columnPaths;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(file);
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

        public override string RowSummary => string.Join(",", this.Current);

        public DateTime? ToDateTime(string dateString)
        {
            if (dateString.StartsWith(@"00/00/"))
            {
                dateString = @"1/1/" + dateString.Substring(6, dateString.Length - 6);
            }
            else if (dateString.StartsWith(@"00/"))
            {
                dateString = @"1/" + dateString.Substring(3, dateString.Length - 3);
            }

            Console.WriteLine(dateString);

            string format = "d/M/yyyy";
            DateTime dateTime;
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }

            return null;
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
}

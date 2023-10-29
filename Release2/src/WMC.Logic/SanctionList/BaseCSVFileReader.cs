using Microsoft.VisualBasic.FileIO;
using System;
using System.Globalization;
using System.IO;

namespace WMC.Logic
{
    public abstract class BaseCSVFileHandler : SanctionList.SanctionListDataReader, IDisposable
    {
        protected readonly TextFieldParser csvParser;
        protected string[] Current;
        public readonly string[] Columns;

        public BaseCSVFileHandler(Stream file, string delimitter = ";", int headderCol = 0, int skipLines = 0)
        {
            this.csvParser = new TextFieldParser(file);
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
            if (!csvParser.EndOfData)
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

                csvParser.Dispose();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                disposedValue = true;
            }
        }

        ~BaseCSVFileHandler()
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

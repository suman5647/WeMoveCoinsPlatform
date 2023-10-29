using System;
using System.Globalization;
using System.IO;

namespace WMC.Logic.SanctionList
{
    public class EUFinancialSanctionsFileReader : BaseCSVFileHandler, IWMCDataReader, IWMCDateParser
    {
        public EUFinancialSanctionsFileReader(Stream file)
            : base(file, ";", 0, 0)
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
                return new object[] { this.Current[17], this.Current[15], this.Current[16], this.Current[14], 
                    null, null, this.Current[39], null, this.RowSummary, 1 };
            }
        }

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
    }
}

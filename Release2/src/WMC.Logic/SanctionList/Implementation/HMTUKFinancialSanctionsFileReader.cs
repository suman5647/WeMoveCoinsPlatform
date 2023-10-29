using System;
using System.Globalization;
using System.IO;

namespace WMC.Logic.SanctionList
{
    public class HMTUKFinancialSanctionsFileReader : BaseCSVFileHandler, IWMCDataReader, IWMCDateParser
    {
        public HMTUKFinancialSanctionsFileReader(Stream file)
            : base(file, ",", 2, 1)
        {
        }

        public override bool IsValid()
        {
            if (!string.IsNullOrEmpty(this.Current[23]) && this.Current[23] == "Individual")
            {
                return true;
            }

            return false;
        }
        public override object[] CurrentRow
        {
            get
            {
                return new object[] { this.Current[0], this.Current[1], this.Current[2], this.Current[3], this.Current[4], 
                    this.Current[5], this.Current[7], this.Current[21], this.RowSummary, 2 };
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

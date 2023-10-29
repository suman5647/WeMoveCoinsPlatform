using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace WMC.Logic.SanctionList
{
    public class OFACSanctionsFileReader : BaseCSVFileHandler, IWMCDataReader, IWMCDateParser
    {
        public OFACSanctionsFileReader(Stream file)
            : base(file, ",", 0, 0)
        {
        }

        public override bool IsValid()
        {
            // if fullname has value, considered correct and valid
            if (this.Current.Length >= 2)
            {
                if (!string.IsNullOrEmpty(this.Current[2]) && this.Current[2] == "individual")
                {
                    return true;
                }
            }

            return false;
        }
        public override object[] CurrentRow
        {
            get
            {
                if (this.Current.Length >= 2)
                {
                    string[] names = SplitNames(this.Current[1]);
                    return new object[] { names[0].Length>0?names[0]:null, names[1].Length > 0 ? names[1] : null, names[2].Length > 0 ? names[2] : null, names[3].Length > 0 ? names[3] : null, names[4].Length > 0 ? names[4] : null, names[5].Length > 0 ? names[5]:null, this.Current.Length >=11 ? this.Current[11] : null, null, this.RowSummary, 3 };
                }
                return null;
            }
        }

        public string[] SplitNames(string nameString)
        {
            string[] Names = nameString.Split(',');
            for(int i=Names.Length+1; i<=6; i++)
            {
                Names = new List<string>(Names) {""}.ToArray();
            }
            return Names;
        }

        public DateTime? ToDateTime(string dateString)
        {
            if (dateString.StartsWith("DOB"))
            {
                var match = Regex.Match(dateString, @"([0-3][0-9]\s(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s\d{4})|(19\d{2})|(200\d)|(201[0-3])").ToString();
                if (match.Length == 11)
                {
                    match = match.Replace(@" ", "/");
                    DateTime dateTime1;
                    DateTime.TryParseExact(match, "dd/MMM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime1);
                    return dateTime1;
                }
                else if (match.Length == 4)
                {
                    DateTime dateTime;
                    match = @"1/1/" + match;
                    DateTime.TryParseExact(match, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
                    return dateTime;
                }
            }
            return null;
        }
    }
}

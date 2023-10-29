using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WMC.Logic.SanctionList
{
    public class UNSanctionsFileReader : BaseXMLFileReader, IWMCDataReader, IWMCDateParser
    {
        public UNSanctionsFileReader(Stream file)
            : base(file, "//CONSOLIDATED_LIST/INDIVIDUALS/INDIVIDUAL",
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
                    this.Current[36], this.Current[23], this.RowSummary, 4 };
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

            string format = "yyyy-MM-dd";
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

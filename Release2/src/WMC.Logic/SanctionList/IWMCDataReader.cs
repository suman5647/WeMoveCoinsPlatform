using System;

namespace WMC.Logic
{
    public interface IWMCDataReader : IDisposable
    {
        bool Read();
        bool IsValid();
        /// <summary>
        /// N1,N2,N3,N4,N5,N6,DOB,COR,Summary,FromSource
        /// </summary>
        object[] CurrentRow { get; }
        string RowSummary { get; }

        string[] SplitNames(string nameString);
    }

    public interface IWMCDateParser : IDisposable
    {
        DateTime? ToDateTime(string dateString);
    }
}

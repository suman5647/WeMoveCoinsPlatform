using System.IO;

namespace WMC.Logic.SanctionList
{
    public abstract class SanctionListDataReader : IWMCDataReader
    {
        public abstract object[] CurrentRow { get; }

        public virtual string RowSummary => null;

        public abstract bool IsValid();

        public abstract bool Read();

        public static IWMCDataReader Create(int sourceType, Stream file)
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
                    return default(IWMCDataReader);
            }
        }

        public abstract void Dispose();

        public string[] SplitNames(string nameString)
        {
            throw new System.NotImplementedException();
        }
    }
}

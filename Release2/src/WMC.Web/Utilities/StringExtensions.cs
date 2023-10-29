namespace WMC.Utilities
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string text)
        {
            double test;
            return double.TryParse(text, out test);
        }
    }
}

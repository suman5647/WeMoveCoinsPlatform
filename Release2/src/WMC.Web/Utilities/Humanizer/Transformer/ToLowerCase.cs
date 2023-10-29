using System.Globalization;

namespace WMC.Web.Utilities.Humanizer.Transformer
{
    class ToLowerCase : IStringTransformer
    {
        public string Transform(string input)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToLower(input);
        }
    }
}
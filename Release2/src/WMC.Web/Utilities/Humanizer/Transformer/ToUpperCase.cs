namespace WMC.Web.Utilities.Humanizer.Transformer
{
    class ToUpperCase : IStringTransformer
    {
        public string Transform(string input)
        {
            return input.ToUpper();
        }
    }
}
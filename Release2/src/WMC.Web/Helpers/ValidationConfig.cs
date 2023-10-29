namespace WMC.Helpers
{
    public class ValidationConfig
    {
        public ValidationTypes ValidationType { get; set; }

        public object ValueToCompare { get; set; }

        public string Expression { get; set; }
    }

}
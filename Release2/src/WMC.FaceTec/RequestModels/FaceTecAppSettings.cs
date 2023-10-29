namespace WMC.FaceTec
{
    public class FaceTecProdKey
    {
        public string domains { get; set; }
        public string expiryDate { get; set; }
        public string key { get; set; }
    }

    public class FaceTecAppSettings
    {
        public string KYC { get; set; }
        public string FaceTecDeviceKey { get; set; }
        public string FaceTecHost { get; set; }
        public int FaceTecPort { get; set; }
        public FaceTecProdKey FaceTecProdKey { get; set; }
    }
}
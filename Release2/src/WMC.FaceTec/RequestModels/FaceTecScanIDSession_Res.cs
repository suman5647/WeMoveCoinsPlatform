using System.Collections.Generic;
using Newtonsoft.Json;
using WMC.Logic;

namespace WMC.FaceTec
{
    public class IdScan
    {
    }

    public class IdScanMetrics
    {
        [JsonProperty("frontImages")]
        public List<string> FrontImages { get; set; }

        [JsonProperty("backImages")]
        public List<string> BackImages { get; set; }

        [JsonProperty("idScan")]
        public IdScan IdScan { get; set; }
    }

    public enum ZoomIDType : long
    {
        /**
          *ID card type
          */
        ZoomIDTypeIDCard = 0,
        /*
         *Passport type
         */
        ZoomIDTypePassport = 1,
        /**
          *ID type was not selected
          */
        ZoomIDTypeNotSelected = 2
    }

    public enum ZoomIDSessionStatus
    {
        /**
          *The ID Scan was successful.
          */
        ZoomIDScanStatusSuccess = 0,
        /**
          *The ID Scan was not successful
          */
        ZoomIDScanStatusUnsuccess = 1,
        /**
          *User cancelled ID Scan
          */
        ZoomIDScanStatusUserCancelled = 2,
        /**
          *Timeout during ID Scan
          */
        ZoomIDScanStatusTimedOut = 3,
        /**
          *Context Switch during ID Scan
          */
        ZoomIDScanStatusContextSwitch = 4,
        /**
          *Error setting up the ID Scan Camera
          */
        CameraError = 5,
        /**
          *Camera Permissions were not enabled
          */
        CameraNotEnabled = 6,
        /**
          *ID Scan was skipped.
          */
        Skipped = 7
    }

    public enum FaceTecIDScanStatus
    {
        /**
        The ID Scan was successful.
       */
        Success = 0,
        /**
         The ID Scan was not successful
        */
        Unsuccess = 1,
        /**
         User cancelled ID Scan
        */
        UserCancelled = 2,
        /**
         Timeout during ID Scan
        */
        TimedOut = 3,
        /**
         Context Switch during ID Scan
        */
        ContextSwitch = 4,
        /**
         Error setting up the ID Scan Camera
        */
        CameraError = 5,
        /**
         Camera Permissions were not enabled
        */
        CameraNotEnabled = 6,
        /**
         ID Scan was skipped.
        */
        Skipped = 7
    }

    public class FaceTecScanIDSession : ICloneCacheObject<FaceTecScanIDSession>
    {
        [JsonProperty("status")]
        public ZoomIDSessionStatus Status { get; set; }

        [JsonProperty("idType")]
        public ZoomIDType IdType { get; set; }

        [JsonProperty("idScanMetrics")]
        public IdScanMetrics IdScanMetrics { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        public FaceTecScanIDSession Clone()
        {
            return new FaceTecScanIDSession { Status = Status, IdType = IdType, SessionId = SessionId };
        }
    }

    public enum FaceTecIDType
    {
        /**
         ID card type
        */
        IDCard = 0,
        /**
         Passport type
        */
        Passport = 1,
        /**
         ID type was not selected
        */
        NotSelected = 2
    }

    public class FaceTecScanIDSessionResult : ICloneCacheObject<FaceTecScanIDSessionResult>
    {

        [JsonProperty("status")]
        public FaceTecIDScanStatus Status { get; set; }

        [JsonProperty("idType")]
        public FaceTecIDType IdType { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        public FaceTecScanIDSessionResult Clone()
        {
            return new FaceTecScanIDSessionResult { Status = Status, IdType = IdType, SessionId = SessionId };
        }
    }
}

using MongoDB.Bson;
using Newtonsoft.Json;
using WMC.Logic;

namespace WMC.FaceTec
{
    public class FaceScanSecurityChecksReq
    {
        [JsonProperty("replayCheckSucceeded")]
        public bool ReplayCheckSucceeded { get; set; }

        [JsonProperty("sessionTokenCheckSucceeded")]
        public bool SessionTokenCheckSucceeded { get; set; }

        [JsonProperty("auditTrailVerificationCheckSucceeded")]
        public bool AuditTrailVerificationCheckSucceeded { get; set; }

        [JsonProperty("faceScanLivenessCheckSucceeded")]
        public bool FaceScanLivenessCheckSucceeded { get; set; }
    }

    public class CallDataReq
    {
        [JsonProperty("tid")]
        public string Tid { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("date")]
        public BsonDateTime Date { get; set; }

        [JsonProperty("epochSecond")]
        public int EpochSecond { get; set; }

        [JsonProperty("requestMethod")]
        public string RequestMethod { get; set; }
    }

    public class AdditionalSessionDataReq
    {
        [JsonProperty("isAdditionalDataPartiallyIncomplete")]
        public bool IsAdditionalDataPartiallyIncomplete { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("deviceModel")]
        public string DeviceModel { get; set; }

        [JsonProperty("deviceSDKVersion")]
        public string DeviceSDKVersion { get; set; }

        [JsonProperty("sessionID")]
        public string SessionID { get; set; }

        [JsonProperty("userAgent")]
        public string UserAgent { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }
    }

    public class ServerInfoReq
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }
    }

    public class SessionDataReq
    {
        [JsonProperty("faceMap")]
        public string FaceMap { get; set; }

        [JsonProperty("auditTrailImage")]
        public string AuditTrailImage { get; set; }

        [JsonProperty("lowQualityAuditTrailImage")]
        public string LowQualityAuditTrailImage { get; set; }
    }

    public class FaceTecLivenessReq : ICloneCacheObject<FaceTecLivenessReq>
    {
        [JsonProperty("_id")]
        public ObjectId Id { get; set; }

        [JsonProperty("faceScanSecurityChecks")]
        public FaceScanSecurityChecksReq FaceScanSecurityChecks { get; set; }

        [JsonProperty("ageEstimateGroupEnumInt")]
        public int AgeEstimateGroupEnumInt { get; set; }

        [JsonProperty("externalDatabaseRefID")]
        public string ExternalDatabaseRefID { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("callData")]
        public CallDataReq CallData { get; set; }

        [JsonProperty("additionalSessionData")]
        public AdditionalSessionDataReq AdditionalSessionData { get; set; }

        [JsonProperty("error")]
        public bool error { get; set; }

        [JsonProperty("serverInfo")]
        public ServerInfoReq ServerInfo { get; set; }

        [JsonProperty("data")]
        public SessionDataReq Data { get; set; }

        public FaceTecLivenessReq Clone()
        {
            return new FaceTecLivenessReq { Id = Id, AgeEstimateGroupEnumInt = AgeEstimateGroupEnumInt, AdditionalSessionData = AdditionalSessionData, Success = Success, FaceScanSecurityChecks = FaceScanSecurityChecks, CallData = CallData };
        }
    }
}
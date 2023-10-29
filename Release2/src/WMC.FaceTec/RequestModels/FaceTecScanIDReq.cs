using MongoDB.Bson;
using Newtonsoft.Json;
using WMC.Logic;

namespace WMC.FaceTec
{

    public class ScanIDDataReq
    {
        [JsonProperty("idScan")]
        public string IdScan { get; set; }

        [JsonProperty("idScanFrontImage")]
        public string IdScanFrontImage { get; set; }

        [JsonProperty("idScanBackImage")]
        public string IdScanBackImage { get; set; }
    }

    public class FaceTecScanIDReq : ICloneCacheObject<FaceTecScanIDReq>
    {
        [JsonProperty("_id")]
        public ObjectId Id { get; set; }

        [JsonProperty("idScanAgeEstimateGroupEnumInt")]
        public int IdScanAgeEstimateGroupEnumInt { get; set; }

        [JsonProperty("externalDatabaseRefID")]
        public string ExternalDatabaseRefID { get; set; }

        [JsonProperty("matchLevel")]
        public int MatchLevel { get; set; }

        [JsonProperty("fullIDStatusEnumInt")]
        public int FullIDStatusEnumInt { get; set; }

        [JsonProperty("digitalIDSpoofStatusEnumInt")]
        public int DigitalIDSpoofStatusEnumInt { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("callData")]
        public CallDataReq CallData { get; set; }

        [JsonProperty("additionalSessionData")]
        public AdditionalSessionDataReq AdditionalSessionData { get; set; }

        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("serverInfo")]
        public ServerInfoReq ServerInfo { get; set; }

        [JsonProperty("data")]
        public ScanIDDataReq Data { get; set; }

        [JsonProperty("enrollmentSession")]
        public ObjectId EnrollmentSession { get; set; }

        public FaceTecScanIDReq Clone()
        {
            return new FaceTecScanIDReq { Id = Id, IdScanAgeEstimateGroupEnumInt = IdScanAgeEstimateGroupEnumInt, AdditionalSessionData = AdditionalSessionData, Success = Success, MatchLevel = MatchLevel, CallData = CallData, EnrollmentSession = EnrollmentSession };
        }
    }

}
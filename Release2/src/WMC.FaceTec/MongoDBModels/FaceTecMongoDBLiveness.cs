using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WMC.Common;
using WMC.Logic;

namespace WMC.FaceTec
{
    public interface IFaceScanSecurityChecks
    {
        bool ReplayCheckSucceeded { get; set; }
        bool SessionTokenCheckSucceeded { get; set; }
        bool AuditTrailVerificationCheckSucceeded { get; set; }
        bool FaceScanLivenessCheckSucceeded { get; set; }
    }
    public interface ICallData
    {
        string Tid { get; set; }
        string Path { get; set; }
        DateTimeOffset Date { get; set; }
        int EpochSecond { get; set; }
        string RequestMethod { get; set; }
    }
    public interface IAdditionalSessionData
    {
        bool IsAdditionalDataPartiallyIncomplete { get; set; }
        string Platform { get; set; }
        string DeviceModel { get; set; }
        string DeviceSDKVersion { get; set; }
        //string SessionId { get; set; }
        string UserAgent { get; set; }
        string IpAddress { get; set; }
    }
    public interface IServerInfo
    {
        string Version { get; set; }
        string Type { get; set; }
        string Mode { get; set; }
    }
    public interface ISessionData
    {
        string FaceMap { get; set; }
        string AuditTrailImage { get; set; }
        string LowQualityAuditTrailImage { get; set; }
    }
    public interface IFaceTecMongoDBLiveness : IDto
    {
        // string Id { get; set; }
        // FaceScanSecurityChecksModel FaceScanSecurityChecks { get; set; }
        int AgeEstimateGroupEnumInt { get; set; }
        string ExternalDatabaseRefID { get; set; }
        bool Success { get; set; }
        // CallDataModel CallData { get; set; }
        //AdditionalSessionDataModel AdditionalSessionData { get; set; }
        bool Error { get; set; }
        // [JsonIgnore]
        // ServerInfoModel ServerInfo { get; set; }
        // SessionDataModel Data { get; set; }
        // IFaceTecMongoDBLiveness Clone();
    }

    public class FaceScanSecurityChecksModel : IFaceScanSecurityChecks, IDto
    {
        public virtual bool ReplayCheckSucceeded { get; set; }
        public virtual bool SessionTokenCheckSucceeded { get; set; }
        public virtual bool AuditTrailVerificationCheckSucceeded { get; set; }
        public virtual bool FaceScanLivenessCheckSucceeded { get; set; }
        public IDto Transfer()
        {
            return default;
        }
    }

    [BsonIgnoreExtraElements]
    public class FaceScanSecurityChecks : IFaceScanSecurityChecks
    {
        [BsonElement("replayCheckSucceeded")]
        public bool ReplayCheckSucceeded { get; set; }
        [BsonElement("sessionTokenCheckSucceeded")]
        public bool SessionTokenCheckSucceeded { get; set; }
        [BsonElement("auditTrailVerificationCheckSucceeded")]
        public bool AuditTrailVerificationCheckSucceeded { get; set; }
        [BsonElement("faceScanLivenessCheckSucceeded")]
        public bool FaceScanLivenessCheckSucceeded { get; set; }
        public IDto Transfer()
        {
            return new FaceScanSecurityChecksModel()
            {
                AuditTrailVerificationCheckSucceeded = AuditTrailVerificationCheckSucceeded,
                FaceScanLivenessCheckSucceeded = FaceScanLivenessCheckSucceeded,
                ReplayCheckSucceeded = ReplayCheckSucceeded,
                SessionTokenCheckSucceeded = SessionTokenCheckSucceeded
            };
        }
    }
    public class CallDataModel : ICallData, IDto
    {
        [JsonIgnore]
        public virtual string Tid { get; set; }
        [JsonIgnore]
        public virtual string Path { get; set; }
        public virtual DateTimeOffset Date { get; set; }
        [JsonIgnore]
        public virtual int EpochSecond { get; set; }
        [JsonIgnore]
        public virtual string RequestMethod { get; set; }

        public virtual IDto Transfer()
        {
            return default;
        }
    }

    [BsonIgnoreExtraElements]
    public class CallData : CallDataModel, ICallData
    {
        [BsonElement("tid")]
        public new string Tid { get; set; }

        [BsonElement("path")]
        public new string Path { get; set; }

        [BsonElement("date")]
        public new BsonDateTime Date { get; set; }

        [BsonElement("epochSecond")]
        public new int EpochSecond { get; set; }

        [BsonElement("requestMethod")]
        public new string RequestMethod { get; set; }

        public new IDto Transfer()
        {
            return new CallDataModel()
            {
                Date = new DateTimeOffset(this.Date.ToUniversalTime()),
                //EpochSecond = EpochSecond,
                //Path = Path,
                //RequestMethod = RequestMethod,
                //Tid = Tid
            };
        }
    }
    public class AdditionalSessionDataModel : IAdditionalSessionData, IDto
    {
        [JsonIgnore]
        public virtual bool IsAdditionalDataPartiallyIncomplete { get; set; }
        [JsonIgnore]
        public virtual string Platform { get; set; }
        [JsonIgnore]
        public virtual string DeviceModel { get; set; }
        [JsonIgnore]
        public virtual string DeviceSDKVersion { get; set; }
        public virtual string SessionID { get; set; }
        [JsonIgnore]
        public virtual string UserAgent { get; set; }
        [JsonIgnore]
        public virtual string IpAddress { get; set; }
        public virtual IDto Transfer()
        {
            return default;
        }
    }

    [BsonIgnoreExtraElements]
    public class AdditionalSessionData : IAdditionalSessionData
    {
        [BsonElement("isAdditionalDataPartiallyIncomplete")]
        public bool IsAdditionalDataPartiallyIncomplete { get; set; }

        [BsonElement("platform")]
        public string Platform { get; set; }

        [BsonElement("deviceModel")]
        public string DeviceModel { get; set; }

        [BsonElement("deviceSDKVersion")]
        public string DeviceSDKVersion { get; set; }

        [BsonElement("sessionID")]
        public string SessionID { get; set; }

        [BsonElement("userAgent")]
        public string UserAgent { get; set; }

        [BsonElement("ipAddress")]
        public string IpAddress { get; set; }

        public IDto Transfer()
        {
            return new AdditionalSessionDataModel()
            {
                //IsAdditionalDataPartiallyIncomplete = IsAdditionalDataPartiallyIncomplete,
                //Platform = Platform,
                //DeviceModel = DeviceModel,
                //DeviceSDKVersion = DeviceSDKVersion,
                SessionID = SessionID,
                //UserAgent = UserAgent,
                //IpAddress = IpAddress
            };
        }
    }
    public class ServerInfoModel : IServerInfo, IDto
    {
        public virtual string Version { get; set; }
        public virtual string Type { get; set; }
        public virtual string Mode { get; set; }

        public virtual IDto Transfer()
        {
            return default;
        }
    }

    [BsonIgnoreExtraElements]
    public class ServerInfo : IServerInfo
    {
        [BsonElement("version")]
        public string Version { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("mode")]
        public string Mode { get; set; }

        public IDto Transfer()
        {
            return new ServerInfoModel()
            {
                Version = Version,
                Type = Type,
                Mode = Mode
            };
        }
    }

    public class SessionDataModel : ISessionData, IDto
    {
        [JsonIgnore]
        public virtual string FaceMap { get; set; }
        public virtual string AuditTrailImage { get; set; }
        [JsonIgnore]
        public virtual string LowQualityAuditTrailImage { get; set; }
        public virtual IDto Transfer()
        {
            return default;
        }
    }

    [BsonIgnoreExtraElements]
    public class SessionData : ISessionData
    {
        [BsonElement("faceMap")]
        public string FaceMap { get; set; }

        [BsonElement("auditTrailImage")]
        public string AuditTrailImage { get; set; }

        [BsonElement("lowQualityAuditTrailImage")]
        public string LowQualityAuditTrailImage { get; set; }

        public IDto Transfer()
        {
            return new SessionDataModel()
            {
                //FaceMap = FaceMap,
                AuditTrailImage = AuditTrailImage,
                //LowQualityAuditTrailImage = LowQualityAuditTrailImage
            };
        }
    }


    [BsonIgnoreExtraElements]
    public class FaceTecMongoDBLiveness : IFaceTecMongoDBLiveness
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("faceScanSecurityChecks")]
        public FaceScanSecurityChecks FaceScanSecurityChecks { get; set; }

        [BsonElement("ageEstimateGroupEnumInt")]
        public int AgeEstimateGroupEnumInt { get; set; }

        [BsonElement("externalDatabaseRefID")]
        public string ExternalDatabaseRefID { get; set; }

        [BsonElement("success")]
        public bool Success { get; set; }

        [BsonElement("callData")]
        public CallData CallData { get; set; }

        [BsonElement("additionalSessionData")]
        public AdditionalSessionData AdditionalSessionData { get; set; }

        [BsonElement("error")]
        public bool Error { get; set; }

        [BsonElement("serverInfo")]
        public ServerInfo ServerInfo { get; set; }

        [BsonElement("data")]
        public SessionData Data { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public IDto Transfer()
        {
            return new FaceTecMongoDBLivenessModel
            {
                Id = Id.ToString(),
                AgeEstimateGroupEnumInt = AgeEstimateGroupEnumInt,
                AdditionalSessionData = AdditionalSessionData.Transfer() as AdditionalSessionDataModel,
                Success = Success,
                FaceScanSecurityChecks = FaceScanSecurityChecks.Transfer() as FaceScanSecurityChecksModel,
                Error = this.Error,
                ExternalDatabaseRefID = this.ExternalDatabaseRefID.ToString(),
                CallData = CallData.Transfer() as CallDataModel,
                Data = Data.Transfer() as SessionDataModel,
                //ServerInfo = ServerInfo.Transfer() as ServerInfoModel
            };
        }
    }

    public class FaceTecMongoDBLivenessModel : IFaceTecMongoDBLiveness, ICloneCacheObject<FaceTecMongoDBLivenessModel>
    {
        [JsonProperty("Id")]
        public string Id { get; set; }
        public FaceScanSecurityChecksModel FaceScanSecurityChecks { get; set; }
        public int AgeEstimateGroupEnumInt { get; set; }
        public string ExternalDatabaseRefID { get; set; }
        public bool Success { get; set; }
        public CallDataModel CallData { get; set; }
        public AdditionalSessionDataModel AdditionalSessionData { get; set; }
        public bool Error { get; set; }
        [JsonIgnore]
        public ServerInfoModel ServerInfo { get; set; }
        public SessionDataModel Data { get; set; }

        public FaceTecMongoDBLivenessModel Clone()
        {
            return new FaceTecMongoDBLivenessModel
            {
                Id = Id,
                AgeEstimateGroupEnumInt = AgeEstimateGroupEnumInt,
                AdditionalSessionData = AdditionalSessionData,
                Success = Success,
                FaceScanSecurityChecks = FaceScanSecurityChecks,
                CallData = CallData,
                //Data = Data
            };
        }

        public IDto Transfer()
        {
            return new FaceTecMongoDBLivenessModel
            {
                Id = Id,
                AgeEstimateGroupEnumInt = AgeEstimateGroupEnumInt,
                AdditionalSessionData = AdditionalSessionData,
                Success = Success,
                FaceScanSecurityChecks = FaceScanSecurityChecks,
                CallData = CallData,
                Data = Data
            };
        }
    }

}
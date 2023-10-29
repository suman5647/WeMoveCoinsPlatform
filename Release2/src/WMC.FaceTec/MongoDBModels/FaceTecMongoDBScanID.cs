using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using WMC.Common;
using WMC.Logic;

namespace WMC.FaceTec
{
    public interface IScanIDData
    {
        string IdScan { get; set; }
        string IdScanFrontImage { get; set; }
        string IdScanBackImage { get; set; }
    }


    public interface IFaceTecMongoDBScanID : IDto
    {
        int IdScanAgeEstimateGroupEnumInt { get; set; }
        string ExternalDatabaseRefID { get; set; }
        int MatchLevel { get; set; }
        int FullIDStatusEnumInt { get; set; }
        int DigitalIDSpoofStatusEnumInt { get; set; }
        bool Success { get; set; }
        bool Error { get; set; }
    }

    public static class DboObjectHelper
    {
        public static FaceTecMongoDBScanIDModel ToScanID(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FaceTecMongoDBScanIDModel>(data);
        }
        public static FaceTecMongoDBLivenessModel ToLiveness(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FaceTecMongoDBLivenessModel>(data);
        }
    }
    public class ScanIDDataModel : IScanIDData, IDto
    {
        [JsonIgnore]
        public virtual string IdScan { get; set; }
        public virtual string IdScanFrontImage { get; set; }
        public virtual string IdScanBackImage { get; set; }

        public virtual IDto Transfer()
        {
            return default;
        }
    }

    [BsonIgnoreExtraElements]
    public class ScanIDData : IScanIDData
    {
        [BsonElement("idScan")]
        public string IdScan { get; set; }

        [BsonElement("idScanFrontImage")]
        public string IdScanFrontImage { get; set; }

        [BsonElement("idScanBackImage")]
        public string IdScanBackImage { get; set; }

        public IDto Transfer()
        {
            return new ScanIDDataModel()
            {
                //IdScan = IdScan,
                IdScanBackImage = IdScanBackImage,
                IdScanFrontImage = IdScanFrontImage
            };
        }
    }

    [BsonIgnoreExtraElements]
    public class FaceTecMongoDBScanID : IFaceTecMongoDBScanID
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("idScanAgeEstimateGroupEnumInt")]
        public int IdScanAgeEstimateGroupEnumInt { get; set; }

        [BsonElement("externalDatabaseRefID")]
        public string ExternalDatabaseRefID { get; set; }

        [BsonElement("matchLevel")]
        public int MatchLevel { get; set; }

        [BsonElement("fullIDStatusEnumInt")]
        public int FullIDStatusEnumInt { get; set; }

        [BsonElement("digitalIDSpoofStatusEnumInt")]
        public int DigitalIDSpoofStatusEnumInt { get; set; }

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
        public ScanIDData Data { get; set; }

        [BsonElement("enrollmentSession")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string EnrollmentSession { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public IDto Transfer()
        {
            return new FaceTecMongoDBScanIDModel
            {
                Id = Id.ToString(),
                IdScanAgeEstimateGroupEnumInt = IdScanAgeEstimateGroupEnumInt,
                AdditionalSessionData = AdditionalSessionData.Transfer() as AdditionalSessionDataModel,
                Success = Success,
                DigitalIDSpoofStatusEnumInt = DigitalIDSpoofStatusEnumInt,
                EnrollmentSession = this.EnrollmentSession.ToString(),
                Error = Error,
                FullIDStatusEnumInt = FullIDStatusEnumInt,
                MatchLevel = MatchLevel,
                //ServerInfo = ServerInfo,
                ExternalDatabaseRefID = this.ExternalDatabaseRefID.ToString(),
                CallData = CallData.Transfer() as CallDataModel,
                Data = Data.Transfer() as ScanIDDataModel,
            };
        }
    }

    public class FaceTecMongoDBScanIDModel : IFaceTecMongoDBScanID, ICloneCacheObject<FaceTecMongoDBScanIDModel>
    {
        public virtual string Id { get; set; }
        public virtual int IdScanAgeEstimateGroupEnumInt { get; set; }
        public virtual string ExternalDatabaseRefID { get; set; }
        public virtual int MatchLevel { get; set; }
        public virtual int FullIDStatusEnumInt { get; set; }
        public virtual int DigitalIDSpoofStatusEnumInt { get; set; }
        public virtual bool Success { get; set; }
        public virtual CallDataModel CallData { get; set; }
        public virtual AdditionalSessionDataModel AdditionalSessionData { get; set; }
        public virtual bool Error { get; set; }
        public virtual ServerInfoModel ServerInfo { get; set; }
        public virtual ScanIDDataModel Data { get; set; }
        public virtual string EnrollmentSession { get; set; }

        public FaceTecMongoDBScanIDModel Clone()
        {
            return new FaceTecMongoDBScanIDModel
            {
                Id = Id,
                IdScanAgeEstimateGroupEnumInt = IdScanAgeEstimateGroupEnumInt,
                AdditionalSessionData = AdditionalSessionData,
                Success = Success,
                MatchLevel = MatchLevel,
                DigitalIDSpoofStatusEnumInt = DigitalIDSpoofStatusEnumInt,
                CallData = CallData,
                Error = Error
                //  Data = Data
            };
        }

        public virtual IDto Transfer()
        {
            return new FaceTecMongoDBScanIDModel
            {
                Id = Id,
                IdScanAgeEstimateGroupEnumInt = IdScanAgeEstimateGroupEnumInt,
                AdditionalSessionData = AdditionalSessionData,
                Success = Success,
                MatchLevel = MatchLevel,
                DigitalIDSpoofStatusEnumInt = DigitalIDSpoofStatusEnumInt,
                CallData = CallData,
                Data = Data
            };
        }
    }
}
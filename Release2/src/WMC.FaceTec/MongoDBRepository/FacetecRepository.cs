using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using WMC.Logic;

namespace WMC.FaceTec
{
    public class FacetecRepository : IFacetecRepository
    {
        readonly ISettingsManager settingsManager;
        readonly IAuditLog auditLog;

        private static volatile object syncRoot = new object();
        private static IFacetecRepository _Instance;
        public static IFacetecRepository Instance(ISettingsManager settingsManager, IAuditLog auditLog)
        {
            if (_Instance == null)
            {
                lock (syncRoot)
                {

                    if (_Instance == null)
                    {
                        _Instance = new FacetecRepository(settingsManager, auditLog);
                    }
                }
            }

            return _Instance;
        }

        IMongoDatabase db;
        private FacetecRepository(ISettingsManager settingsManager, IAuditLog auditLog)
        {
            this.settingsManager = settingsManager;
            this.auditLog = auditLog;

            MongoDBSettings mongoDBSettings = settingsManager.Get("MongoDBDetails").GetJsonData<MongoDBSettings>();
            var MongoDatabaseName = mongoDBSettings.MongoDatabaseName;
            var MongoUsername = mongoDBSettings.MongoUsername;
            var MongoPassword = mongoDBSettings.MongoPassword;
            var MongoPort = mongoDBSettings.MongoPort;
            var MongoHost = mongoDBSettings.MongoHost;
            var MongoURL = mongoDBSettings.MongoDBURL;


            // Creating credentials  
            var credential = MongoCredential.CreateCredential(MongoDatabaseName, MongoUsername, MongoPassword);

            var settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(MongoHost, Convert.ToInt32(MongoPort))
            };

            try
            {
                var client = new MongoClient(MongoURL);
                db = client.GetDatabase(MongoDatabaseName);
                auditLog.RecordLog("Connected to mongoDB successfully for FaceTec", (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Info);
            }
            catch (Exception ex)
            {
                auditLog.RecordLog("Failed to connect mongoDB for FaceTec" + ex, (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public FaceTecMongoDBLivenessModel GetFaceTecDocWithSessionId(string sessionId)
        {
            FaceTecMongoDBLiveness docs = null;
            try
            {
                var document = db.GetCollection<FaceTecMongoDBLiveness>("Session");

                var filter = Builders<FaceTecMongoDBLiveness>.Filter.Regex(x => x.AdditionalSessionData.SessionID, new BsonRegularExpression(sessionId));

                docs = document.Find(filter).FirstOrDefault();

                auditLog.RecordLog("Filter on Session successfully " + sessionId, (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Info);

                return docs.Transfer() as FaceTecMongoDBLivenessModel;
            }
            catch (Exception ex)
            {
                auditLog.RecordLog("Filter on Session failed " + sessionId + ex, (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Error);

                return new FaceTecMongoDBLivenessModel { };
            }
        }

        public FaceTecMongoDBScanIDModel GetFaceTecScanDocWithSessionId(string sessionId)
        {
            FaceTecMongoDBScanID docs = null;
            try
            {
                var document = db.GetCollection<FaceTecMongoDBScanID>("Session");

                var filter = Builders<FaceTecMongoDBScanID>.Filter.Regex(x => x.AdditionalSessionData.SessionID, new BsonRegularExpression(sessionId));

                docs = document.Find(filter).FirstOrDefault();

                auditLog.RecordLog("Filter on ScanIDSession Success " + sessionId, (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Info);

                return docs.Transfer() as FaceTecMongoDBScanIDModel;
            }
            catch (Exception ex)
            {
                auditLog.RecordLog("Filter on ScanIDSession failed" + sessionId + ex, (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Error);

                return new FaceTecMongoDBScanIDModel { };
            }
        }
    }
}
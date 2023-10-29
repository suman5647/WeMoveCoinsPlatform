using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using WMC.FaceTec;
using WMC.Logic;
using WMC.Web.Controllers;

namespace WMC.Web.Tests.Logic
{
    [TestClass]
    public class TestMongoDB
    {
        //MongoContext _dbContext;
        //public TestMongoDB()
        //{
        //    _dbContext = new MongoContext();
        //}
        //MongoClient _client;
        //public IMongoDatabase _database;
        //[TestMethod]
        //public void MongoContext()        //constructor   
        //{

        //    // Reading credentials from Web.config file   
        //    var MongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"]; //CarDatabase  
        //    var MongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; //demouser  
        //    var MongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; //Pass@123  
        //    var MongoPort = ConfigurationManager.AppSettings["MongoPort"];  //27017  
        //    var MongoHost = ConfigurationManager.AppSettings["MongoHost"];  //localhost  

        //    // Creating credentials  
        //    var credential = MongoCredential.CreateCredential(MongoDatabaseName, MongoUsername, MongoPassword);

        //    // Creating MongoClientSettings  
        //    var settings = new MongoClientSettings
        //    {
        //        Credential = credential,
        //        Server = new MongoServerAddress(MongoHost, Convert.ToInt32(MongoPort))
        //    };
        //    _client = new MongoClient(settings);
        //    _database = _client.GetDatabase(MongoDatabaseName);


        //    var doc = _database.GetCollection<Models.FaceTecSessionMongoDB>("Session");

        //    var doc2 = Builders<Models.FaceTecSessionMongoDB>.Filter.Eq("sessionID", "6f028fe5-fc7d-4520-9de3-bcc832ad7f24");


        //    var doc3 = doc.Find(doc2).FirstOrDefault();

        //    var doc_1 = _database.GetCollection<Models.FaceTecScanIDSessionMongoDB>("Session");

        //    var doc_2 = Builders<Models.FaceTecScanIDSessionMongoDB>.Filter.Eq("ExternalDatabaseRefID", "e5c23c60-a032-422f-a4e3-ac3698c02476");

        //    var doc_3 = doc_1.Find(doc_2).FirstOrDefault();

        //    //var doc_22 = _database.GetCollection<Models.FaceTecSessionMongoDB>("FaceTecSessionMongoDB").Find<>;

        //    ////var doc_23 = Builders<Models.FaceTecSessionMongoDB>.;
        [TestMethod]
        public void Test()
        {
            //var MongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"]; //CarDatabase  
            //var MongoUsername = ConfigurationManager.AppSettings["MongoUsername"]; //demouser  
            //var MongoPassword = ConfigurationManager.AppSettings["MongoPassword"]; //Pass@123  
            //var MongoPort = ConfigurationManager.AppSettings["MongoPort"];  //27017  
            //var MongoHost = ConfigurationManager.AppSettings["MongoHost"];  //localhost  

            //// Creating credentials  
            //var credential = MongoCredential.CreateCredential(MongoDatabaseName, MongoUsername, MongoPassword);

            //var settings = new MongoClientSettings
            //{
            //    Credential = credential,
            //    Server = new MongoServerAddress(MongoHost, Convert.ToInt32(MongoPort))
            //};
            //var client = new MongoClient(settings);


            //IMongoDatabase db = client.GetDatabase(MongoDatabaseName);

            //var cars = db.GetCollection<FaceTecSessionMongoDB>("Session");

            //var filter = Builders<FaceTecSessionMongoDB>.Filter.Eq(x => x.AdditionalSessionData.SessionID, );

            //var docs = cars.Find(filter).FirstOrDefault();


            //var cars1 = db.GetCollection<FaceTecScanIDSessionMongoDB>("Session");

            //var filter1 = Builders<FaceTecScanIDSessionMongoDB>.Filter.Eq(x => x.AdditionalSessionData.SessionID, "0c56cdc7-c46f-42d5-ae74-fa8cb6a42706");

            //var docs1 = cars1.Find(filter1).FirstOrDefault();
            var s = KycDataProvider.Instance;
            var d1 = s.GetFaceTecDocWithSessionId("c2785c52-e83b-4044-9ed3-c145e7b1d9fc");
            //var mongoDBUnitOfWork = KycDataProvider.Instance;
            var facetecLivenessSession = s.GetFaceTecScanDocWithSessionId("b9a1710a-0c73-4708-9672-75bd3a89195a");
            ////var ft = BsonSerializer.Deserialize<FaceTecLivenessReq>(d1);
            ////var result = JsonConvert.DeserializeObject<FaceTecLivenessReq>(d1.);
            //var d2 = s.GetFaceTecScanDocWithSessionId("b9a1710a-0c73-4708-9672-75bd3a89195a");
            //AuditLog.log("faceTecSessionDBLivenessLite " + JsonConvert.SerializeObject(d1?.Clone()) + "ScanId:" + JsonConvert.SerializeObject(d2.Clone()), 17, 2);




            //MongoDBSettings mongoDBSettings = settingsManager.Get("MongoDBDetails").GetJsonData<MongoDBSettings>();
            //var MongoDatabaseName = "facetec-sdk-data";
            //var MongoUsername = mongoDBSettings.MongoUsername;
            //var MongoPassword = mongoDBSettings.MongoPassword;
            //var MongoPort = mongoDBSettings.MongoPort;
            //var MongoHost = mongoDBSettings.MongoHost;
            //var MongoURL = "mongodb://monniTest:FaceTec05Monni@localhost:27017/";


            //var client = new MongoClient(MongoURL);
            //IMongoDatabase db = client.GetDatabase(MongoDatabaseName);

            //FaceTecMongoDBScanID docs = null;

            //string sessionId = "b9a1710a-0c73-4708-9672-75bd3a89195a";

            //var document = db.GetCollection<FaceTecMongoDBScanID>("Session");

            //var filter = Builders<FaceTecMongoDBScanID>.Filter.Regex(x => x.AdditionalSessionData.SessionID, new BsonRegularExpression(sessionId));
            //                                                   //Eq(x => x.ExternalDatabaseRefID, "50b74555-26d6-4583-b7a5-8ac705f12d38"); 
            //docs = document.Find(filter).FirstOrDefault();

            ////liveness
            //FaceTecMongoDBLiveness docs1 = null;

            //string sessionId1 = "b9a1710a-0c73-4708-9672-75bd3a89195a";

            //var document1 = db.GetCollection<FaceTecMongoDBLiveness>("Session");

            //var filter1 = Builders<FaceTecMongoDBLiveness>.Filter.Regex(x => x.AdditionalSessionData.SessionID, new MongoDB.Bson.BsonRegularExpression(sessionId1));

            //docs1 = document1.Find(filter1).FirstOrDefault();
            //auditLog.RecordLog("Filter on ScanIDSession Success " + sessionId, (int)Data.Enums.AuditLogStatus.FaceTec, (int)Data.Enums.AuditTrailLevel.Info);

            //var userSessionModel = new UserSessionModel();
            //userSessionModel.KycRequirement = "KYC";
            //userSessionModel.PhoneNumber = "+917093635254";
            //userSessionModel.OrderAmount = 500;
            //userSessionModel.OrderId = 205;
            //var n = new Controllers.PurchaseController();
            //n.ViewBag.UserSessionModel = userSessionModel;
            //FaceTecSession f = new FaceTecSession();
            //f.isSuccess = true;
            //f.zoomIDScanResult.IdType = (int)FaceTecIDType.IDCard;
            //f.zoomIDScanResult.SessionId = "11874bbe-78a1-4e26-8449-e7d26f7a13d3";
            //f.zoomIDScanResult.Status = (int)FaceTecIDScanStatus.Success;
            //f.zoomSessionResult.SessionId = "fe862edb-95d9-4b44-bb57-5abe88a788bd";
            //f.zoomSessionResult.Status = 0;
            //n.FaceTecKYCFileUpload(f);

        }
    }

    public class TestFaceTecMongoDBScanID
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
        public ObjectId EnrollmentSession { get; set; }

    }
}
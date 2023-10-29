using System;
using System.Web.Http;
using RestSharp;
using WMC.FaceTec;
using WMC.Logic;

namespace WMC.Web.Controllers
{
    /// <summary>
    /// TODO: add security, public key from header, add authorization
    /// Also add action filer to set throtling
    /// </summary>
    [RoutePrefix("kycs")]
    public class KycDataController : ApiController, IKycDataProvider
    {
        [HttpGet()]
        [Route("sessions/{sessionId}/livelinessdoc")]
        public FaceTecMongoDBLivenessModel GetFaceTecDocWithSessionId(string sessionId)
        {
            var mongoDBUnitOfWork = FacetecRepository.Instance(SettingsManager.GetDefault(), new AuditLog());
            return mongoDBUnitOfWork.GetFaceTecDocWithSessionId(sessionId);
        }

        [HttpGet()]
        [Route("sessions/{sessionId}/scandoc")]
        public FaceTecMongoDBScanIDModel GetFaceTecScanDocWithSessionId(string sessionId)
        {
            var mongoDBUnitOfWork = FacetecRepository.Instance(SettingsManager.GetDefault(), new AuditLog());
            return mongoDBUnitOfWork.GetFaceTecScanDocWithSessionId(sessionId);
        }
    }

    public interface IKycDataProvider
    {
        FaceTecMongoDBLivenessModel GetFaceTecDocWithSessionId(string sessionId);
        FaceTecMongoDBScanIDModel GetFaceTecScanDocWithSessionId(string sessionId);
    }

    public class KycDataProvider : IKycDataProvider
    {
        private readonly string baseUrl;
        private readonly IKycDataProvider proxy = null;
        private static volatile object syncRoot = new object();
        private static IKycDataProvider _Instance;
        public static IKycDataProvider Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (syncRoot)
                    {

                        if (_Instance == null)
                        {
                            _Instance = new KycDataProvider();
                        }
                    }
                }

                return _Instance;
            }
        }

        private KycDataProvider()
        {
            string baseUrl = SettingsManager.GetDefault().Get("KycDataProviderUrl")?.Value;
            // TODO: get baseurl from DB?
            // string baseUrl = "/"; // https://test.app.monni.com
            this.baseUrl = baseUrl;
            if (string.IsNullOrEmpty(baseUrl) || baseUrl.Length < 5)
            {
                proxy = new KycDataController();
            }
        }

        public FaceTecMongoDBLivenessModel GetFaceTecDocWithSessionId(string sessionId)
        {
            try
            {
                if (proxy != null)
                {
                    return proxy.GetFaceTecDocWithSessionId(sessionId);
                }
                else
                {
                    var restClient = new RestClient(baseUrl + $"/kycs/sessions/{sessionId}/livelinessdoc");
                    var request = new RestRequest();
                    var response = restClient.Get(request);
                    var res = DboObjectHelper.ToLiveness(response.Content);
                    return res.Transfer() as FaceTecMongoDBLivenessModel;
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in GetFaceTecLivenessDocsWithSessionId():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                return null;
            }
        }

        public FaceTecMongoDBScanIDModel GetFaceTecScanDocWithSessionId(string sessionId)
        {
            try
            {
                if (proxy != null)
                {
                    return proxy.GetFaceTecScanDocWithSessionId(sessionId);
                }
                else
                {
                    var restClient = new RestClient(baseUrl + $"/kycs/sessions/{sessionId}/scandoc");
                    var request = new RestRequest();
                    var response = restClient.Get(request);
                    var res = DboObjectHelper.ToScanID(response.Content);
                    return res.Transfer() as FaceTecMongoDBScanIDModel;
                }
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in GetFaceTecScanDocWithSessionId():\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.ApplicationError, (int)Data.Enums.AuditTrailLevel.Error);
                return null;
            }
        }
    }
}
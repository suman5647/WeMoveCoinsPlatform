using System;

namespace WMC.FaceTec
{
    public interface IFacetecRepository
    {
        FaceTecMongoDBLivenessModel GetFaceTecDocWithSessionId(string sessionId);
        FaceTecMongoDBScanIDModel GetFaceTecScanDocWithSessionId(string sessionId);
    }
}
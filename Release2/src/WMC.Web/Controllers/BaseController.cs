using System;
using System.Web.Mvc;
using WMC.Data;
using WMC.Data.Interfaces;

namespace WMC.Web.Controllers
{
    public class BaseController : Controller
    {
        public IDataUnitOfWork DataUnitOfWork { get; set; }

        protected override bool DisableAsyncSupport
        {
            get
            {
                return true;
            }
        }

        protected override void ExecuteCore()
        {
            DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            base.ExecuteCore();
        }

        protected string ReferrerUrl
        {
            get
            {
                Uri referrer = Request.UrlReferrer;
                if (referrer != null)
                {
                    string original = referrer.OriginalString; //.ToLower();
                                                               // "Visitor Came From " + original;
                    return original;
                }

                return null;
            }
        }
    }
}
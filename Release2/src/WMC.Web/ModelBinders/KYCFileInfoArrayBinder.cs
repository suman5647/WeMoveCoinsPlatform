using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using WMC.Helpers;

namespace WMC.Web.ModelBinders
{
    public class KYCFileInfoBinder : IModelBinder
    {
        static int _FileSize = 5 * 1024 * 1024;

        static KYCFileInfoBinder()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxFileUploadSize"], out _FileSize))
            {
                _FileSize = 5 * 1024 * 1024;
            }

        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var kycFileInfo = new KYCFileInfo { Type = controllerContext.HttpContext.Request.Form["fileType"] };
            var files = controllerContext.HttpContext.Request.Files;

            for (int i = 0; i < files.Count; i++)
            {
                var fileSize = files[i].ContentLength;

                // Settings.  
                var isValid = fileSize <= _FileSize;
                if (!isValid)
                {
                    var errorMesaage = Helpers.ResourceExtensions.Resource(controllerContext.HttpContext, "WMCResources", "Maxfilesize");
                    kycFileInfo.Error = errorMesaage;
                }
                kycFileInfo.Files.Add(files[i]);
            }
            return kycFileInfo;
        }
    }
    public class KYCFileInfo
    {
        public List<HttpPostedFileBase> Files { get; set; } = new List<HttpPostedFileBase>();
        public string Type { get; set; }
        public string Error { get; set; }
    }
}
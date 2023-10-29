using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMC.Helpers;
using WMC.Utilities;

namespace Web.ActionResults
{
	public class BetterJsonResult : JsonResult
	{
		public IList<Tuple<string, string>> ErrorMessageDetails { get; private set; }
		public IList<string> ErrorMessages { get; private set; }
		public int StatusCode { get; set; }
		public string ErrorCode { get; set; }

		public BetterJsonResult()
		{
			ErrorMessages = new List<string>();
			ErrorMessageDetails = new List<Tuple<string, string>>();
		}

		public void AddError(string errorMessage)
		{
			ErrorMessages.Add(errorMessage);
            StatusCode = 400;
        }

		public void AddErrorKey(string messageId, string lang)
		{
			AddError(messageId, StringResourceService.GetDefault().Get(messageId, lang)?.Value);
		}

		public void AddError(string messageId, string errorMessage)
		{
			ErrorMessageDetails.Add(new Tuple<string, string>( messageId, errorMessage));
			StatusCode = 400;
		}
		public override void ExecuteResult(ControllerContext context)
		{
			DoUninterestingBaseClassStuff(context);
			SerializeData(context.HttpContext.Response);
		}

		private void DoUninterestingBaseClassStuff(ControllerContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
				"GET".Equals(context.HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase))
				throw new InvalidOperationException(
					"GET access is not allowed.  Change the JsonRequestBehavior if you need GET access.");

			var response = context.HttpContext.Response;
			response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

			if (ContentEncoding != null)
				response.ContentEncoding = ContentEncoding;
		}

		protected virtual void SerializeData(HttpResponseBase response)
		{
			if (ErrorMessages.Any())
			{
				Data = new
				{
                    ErrorCode = ErrorCode,
                    ErrorMessage = string.Join("\n", ErrorMessages),
					ErrorMessages = ErrorMessages.ToArray()
				};
                response.StatusCode = StatusCode;
            }
			else if (ErrorMessageDetails.Any())
			{
				Data = new
				{
					ErrorCode = ErrorCode,
					Errors = ErrorMessageDetails
				};
				response.StatusCode = StatusCode;
			}
			if (Data == null) return;
			response.Write(Data.ToJson());
		}
	}

	public class BetterJsonResult<T> : BetterJsonResult
	{
		public new T Data
		{
			get { return (T) base.Data; }
			set { base.Data = value; }
		}
	}
}

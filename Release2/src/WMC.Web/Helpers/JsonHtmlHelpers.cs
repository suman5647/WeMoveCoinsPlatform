using System.Web;
using System.Web.Mvc;
using WMC.Utilities;

namespace WMC.Helpers
{
	public static class JsonHtmlHelpers
	{
		public static IHtmlString JsonFor<T>(this HtmlHelper helper, T obj)
		{
			return helper.Raw(obj.ToJson());
		}
	}
}
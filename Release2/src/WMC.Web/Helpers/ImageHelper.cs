using System.Collections.Generic;
using System.Web.Mvc;

namespace WMC.Helpers
{
    public static class ImageHelper
    {
        public static MvcHtmlString ImageLink(this HtmlHelper htmlHelper, string imgSrc, string alt, string actionName, string controllerName, object routeValues, object htmlAttributes, object imgHtmlAttributes)
        {
            UrlHelper urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
            var imgTag = new TagBuilder("img");
            imgTag.MergeAttribute("src", imgSrc);
            imgTag.MergeAttributes((IDictionary<string, string>)imgHtmlAttributes, true);
            string url = urlHelper.Action(actionName, controllerName, routeValues);

            var imglink = new TagBuilder("a");
            imglink.MergeAttribute("href", url);
            imglink.InnerHtml = imgTag.ToString();
            imglink.MergeAttributes((IDictionary<string, string>)htmlAttributes, true);

            return MvcHtmlString.Create(imglink.ToString());
        }
    }
}

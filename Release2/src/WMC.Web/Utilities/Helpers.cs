using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace WMC.Utilities
{
    public class Helpers
    {
        private Helpers()
        {
        }

        private const int MAX_VALUE_INFO_COUNT = 200;
        public static string Title
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();

                // use assembly name as default title
                string title__1 = asm.GetName().Name;

                // find title from assembly title attribue
                object[] attributes = asm.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        title__1 = titleAttribute.Title;
                    }
                }
                // append assembly version to totle
                return title__1 + ". Version: " + Convert.ToString(asm.GetName().Version);
            }
        }

        public static string GetPostBackCall(Control ctrl, char action, string target)
        {
            return ctrl.Page.ClientScript.GetPostBackEventReference(ctrl, action + target) + ";return false;";
        }

        private static string GetImageUrl(string parentDir, string image)
        {
            return "ImageHandler.ashx?img=" + HttpContext.Current.Server.UrlEncode(Path.Combine(parentDir, image));
        }
    }
}
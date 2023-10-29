using System;
using System.Collections.Generic;
using System.Text;

namespace WMC.Logic
{
    public static class ExceptionEx
    {
        public static string ToMessageAndCompleteStacktrace(this Exception exception)
        {
            Exception e = exception;
            StringBuilder s = new StringBuilder();
            while (e != null)
            {
                s.AppendLine("Exception type: " + e.GetType().FullName);
                s.AppendLine("Message: " + e.Message);
                s.AppendLine("Stacktrace:");
                s.AppendLine(e.StackTrace);
                s.AppendLine();
                e = e.InnerException;
            }
            return s.ToString();
        }
    }

    public class NullChecker
    {
        public static string GetResult(params Func<dynamic>[] t)
        {
            var nullResult = new List<int>();
            var exceptionResult = new List<int>();
            for (int i = 0; i < t.Length; i++)
            {
                try
                {
                    var result = t[i]();
                    if (result == null)
                        nullResult.Add(i);
                }
                catch (Exception ex)
                {
                    exceptionResult.Add(i);
                }
            }
            return (exceptionResult.Count > 0 ? ("Element" + (exceptionResult.Count > 1 ? "s" : "") + " at " + string.Join(",", exceptionResult) + " has issue.") : "") + " " + 
                   (nullResult.Count > 0 ? ("Element" + (nullResult.Count > 1 ? "s" : "") + " at " + string.Join(",", nullResult) + (nullResult.Count > 1 ? " are" : " is") + " null.") : "");
        }

        public static bool TryCheckException(params Func<dynamic>[] t)
        {
            var nullResult = new List<int>();
            var exceptionResult = new List<int>();
            for (int i = 0; i < t.Length; i++)
            {
                try
                {
                    var result = t[i]();
                    if (result == null)
                        nullResult.Add(i);
                }
                catch (Exception ex)
                {
                    exceptionResult.Add(i);
                }
            }
            return exceptionResult.Count > 0 ? false : true;
        }
    }
}

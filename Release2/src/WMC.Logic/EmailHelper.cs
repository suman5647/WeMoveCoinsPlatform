using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using WMC.Data;
using WMC.Logic;

namespace WMC.Utilities
{
    public class EmailHelper
    {
        public static void TestSendEmail(string toAddresses, string templateName, Dictionary<string, object> parameters, Func<string, string> getTemplateContent, string siteName)
        {
            var dc = new MonniData();
            var sites = dc.Sites.ToList();
            var emailServerSettingsJson = sites.FirstOrDefault(q => q.Text == siteName).SMTPServerSettings;
            //var appSettings = dc.AppSettings.ToList();
            //var emailServerSettingsJson = appSettings.FirstOrDefault(q => q.ConfigKey == "SMTPServerSettings").ConfigValue;
            var emailServerSettings = JsonConvert.DeserializeObject<SMTPServerSettings>(emailServerSettingsJson);
            var template = emailServerSettings.Templates.Where(q => q.TemplateName == templateName).First();

            string from = emailServerSettings.UserId;
            SmtpClient smtpClient = new SmtpClient(emailServerSettings.ServerName, int.Parse(emailServerSettings.ServerPort));
            smtpClient.Credentials = new NetworkCredential(emailServerSettings.UserId, emailServerSettings.Password);
            smtpClient.EnableSsl = emailServerSettings.EnableSsl;
            smtpClient.EnableSsl = emailServerSettings.EnableSsl;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                                   System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            string currentTemplate = getTemplateContent(template.TemplateUrl);
            var subject = template.Subject;
            foreach (var parameter in parameters)
            {
                currentTemplate = currentTemplate.Replace("{{" + parameter.Key + "}}", parameter.Value.ToString());
                subject = subject.Replace("{{" + parameter.Key + "}}", parameter.Value.ToString());
            }

            MailMessage message = new MailMessage();
            message.From = new MailAddress(from, siteName.Replace("test.app.", ""));
            message.Subject = subject;
            message.To.Add(toAddresses);
            message.IsBodyHtml = true;
            message.Body = currentTemplate;
            smtpClient.Send(message);
        }

        public static void SendEmail(string toAddresses, string templateName, Dictionary<string, object> parameters, string siteName, string bccAddress = null, string customHtml = null,string customCss = null)
        {
            try
            {
                var dc = new MonniData();
                // var sites = dc.Sites.ToList();
                var siteObject = dc.Sites.FirstOrDefault(q => q.Text == siteName);
                if (siteObject == null)
                    throw new Exception("Unable to find the site " + siteName + " in sites table.");
                //var appSettings = dc.AppSettings.ToList();
                //var emailServerSettingsJson = appSettings.FirstOrDefault(q => q.ConfigKey == "SMTPServerSettings").ConfigValue;
                var emailServerSettings = JsonConvert.DeserializeObject<SMTPServerSettings>(siteObject.SMTPServerSettings);
                var template = emailServerSettings.Templates.Where(q => q.TemplateName == templateName).First();

                string from = emailServerSettings.UserId;
                var siteNameShort = string.Empty;
                if (siteName.Contains("localhost"))
                {
                    siteNameShort = siteName;
                }
                else
                {
                    var sitenameparts = siteName.Split('.');
                    siteNameShort = sitenameparts[sitenameparts.Length - 2];
                }

                var templateUrl = template.TemplateUrl.Replace("{SiteName}", siteNameShort);
                string currentTemplate = GetTemplateContent(templateUrl);
                var subject = template.Subject;
                var parametersSummary = "SendEmail parameter";
                // currentTemplate = currentTemplate.Replace("{{SiteURL}}", siteObject.Url);
                Regex.Replace(currentTemplate, "{{SiteURL}}", siteObject.Url, RegexOptions.IgnoreCase);
                Regex.Replace(currentTemplate, Uri.EscapeUriString("{{SiteURL}}"), siteObject.Url, RegexOptions.IgnoreCase);
                Regex.Replace(currentTemplate, "SiteURL", siteObject.Url, RegexOptions.IgnoreCase);
                // Regex.Replace(currentTemplate, $"*|MC:SUBJECT|*", subject, RegexOptions.IgnoreCase);
                foreach (var parameter in parameters)
                {
                    try
                    {
                        parametersSummary += string.Format("\r\nparameter.Key:{0} parameter.Value:{1}", parameter.Key, parameter.Value);

                        currentTemplate = currentTemplate.Replace(string.Format("{{{{{0}}}}}", parameter.Key), parameter.Value.ToString());
                        subject = subject.Replace(string.Format("{{{{{0}}}}}", parameter.Key), parameter.Value.ToString());
                    }
                    catch (Exception)
                    {
                    }
                }

                currentTemplate = currentTemplate.Replace("*|MC:SUBJECT|*", subject);
                if (customHtml != null)
                {
                    // <CustomHTML/>
                    currentTemplate = currentTemplate.Replace("<CustomHTML/>", customHtml);
                }
                if (customCss != null)
                {
                    // <CustomStyle/>
                    currentTemplate = currentTemplate.Replace("<CustomStyle/>", customCss);
                }
                AuditLog.log(parametersSummary, (int)Data.Enums.AuditLogStatus.SentEmail, (int)Data.Enums.AuditTrailLevel.Info);

                List<string> bccAddresses = new List<string>() { "bcc@wemovecoins.com", bccAddress };
                if (templateName == "OrderCompleted")
                    if (!string.IsNullOrEmpty((string)parameters["BccTrustPilotAddress"]))
                        bccAddresses.Add(parameters["BccTrustPilotAddress"].ToString());

                SendSimpleEmail(emailServerSettings, new string[] { toAddresses }, subject, currentTemplate, from, siteName.Replace("test.app.", ""), bccAddresses.ToArray());
            }
            catch (Exception ex)
            {
                AuditLog.log("Error in SendEmail(" + toAddresses + "," + templateName + "," + parameters + "," + siteName + "," + bccAddress + ")\r\n" + ex.ToMessageAndCompleteStacktrace(), (int)Data.Enums.AuditLogStatus.SentEmail, (int)Data.Enums.AuditTrailLevel.Error);
            }
        }

        public static void SendSimpleEmail(SMTPServerSettingsBase emailServerSettings, string[] toAddresses, string subject, string emailBody, string from, string siteName, string[] bccAddress, bool isHtmlBody = true)
        {
            SmtpClient smtpClient = new SmtpClient(emailServerSettings.ServerName, int.Parse(emailServerSettings.ServerPort));
            smtpClient.Credentials = new NetworkCredential(emailServerSettings.UserId, emailServerSettings.Password);
            smtpClient.EnableSsl = emailServerSettings.EnableSsl;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                                   System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                                   System.Security.Cryptography.X509Certificates.X509Chain chain,
                                   System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            MailMessage message = new MailMessage();
            message.From = new MailAddress(from, siteName);
            if (!string.IsNullOrEmpty(emailServerSettings.ReplyTo))
                message.ReplyToList.Add(new MailAddress(emailServerSettings.ReplyTo));
            message.Subject = subject;
            foreach (var to in toAddresses)
            {
                message.To.Add(to);
            }

            if (bccAddress != null)
            {
                foreach (var bcc in bccAddress)
                {
                    if (!string.IsNullOrEmpty(bcc))
                        message.Bcc.Add(bcc);
                }
            }

            message.IsBodyHtml = isHtmlBody;
            message.Body = emailBody;
            smtpClient.Send(message);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static string GetTemplateContent(string templateUrl)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebResponse response = (HttpWebResponse)((HttpWebRequest)WebRequest.Create(templateUrl)).GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //StreamReader readStream = null;
                    //if (response.CharacterSet == null)
                    //    readStream = new StreamReader(response.GetResponseStream());
                    //else
                    //    readStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(response.CharacterSet));

                    //string data = readStream.ReadToEnd();
                    string data = Encoding.UTF8.GetString(ReadFully(response.GetResponseStream()));
                    if (string.IsNullOrEmpty(data))
                    {
                        throw new Exception("Email content for template " + templateUrl + " is empty after converting to UTF8. Response StatusCode:" + response.StatusCode + " Response StatusDescription" + response.StatusDescription);
                    }
                    response.Close();
                    //readStream.Close();                    
                    return data;
                }
                else
                {
                    throw new Exception("Unable to get content for template " + templateUrl + ". Response StatusCode:" + response.StatusCode + " Response StatusDescription" + response.StatusDescription);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTemplateContent(" + templateUrl + ")", ex);
            }
        }
    }

    public class Template
    {
        public string TemplateName { get; set; }
        public string TemplateUrl { get; set; }
        public string Subject { get; set; }
    }
    public class SMTPServerSettingsBase
    {
        public string ServerName { get; set; }
        public string ServerPort { get; set; }
        public string ReplyTo { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }

    public class SMTPServerSettings : SMTPServerSettingsBase
    {
        public List<Template> Templates { get; set; }
    }

    public class SMTPServerSettings2 : SMTPServerSettingsBase
    {
        public SMTPServerSettings2()
        {
            Environment = "PROD";
        }
        public string From { get; set; }
        public string Environment { get; set; }
        public List<string> To { get; set; }
    }
}
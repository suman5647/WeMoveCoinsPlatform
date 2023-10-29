using System;
using System.Data.Entity.Validation;
using System.Text;
using WMC.Data;

namespace WMC.Logic
{
    public class AuditLog : IAuditLog
    {
        public void RecordLog(string message, long status, long auditTrailLevel, long? orderId = null)
        {
            Logic.AuditLog.log(message, status, auditTrailLevel, orderId);
        }

        public static void log(string message, long status, long auditTrailLevel, long? orderId = null)
        {
            try
            {
                //var currentAuditLevel = 2;
                //if (auditTrailLevel >= currentAuditLevel)
                //{ }
                var dc = new MonniData();
                dc.AuditTrails.Add(new AuditTrail
                {
                    Message = message,
                    Status = status,
                    OrderId = orderId,
                    Created = DateTime.Now,
                    AuditTrailLevelId = auditTrailLevel
                });
                dc.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
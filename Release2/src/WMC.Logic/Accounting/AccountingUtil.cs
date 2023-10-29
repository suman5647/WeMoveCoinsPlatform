using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;
using WMC.Data.Enums;

namespace WMC.Logic.Accounting
{
    public static class AccountingUtil
    {
        public const string DATEFORMAT = "yyyy-MM-dd";
        // between 3001000 and 3999999.

        public static DateTime ACC_START_DATE = new DateTime(2019, 12, 12);
        public static double ACC_START_DAY_NO = 3001022;
        public static long GetAccountId(long type, long currency, string valueFor, int transactionType = 2, ParticularType particularType = ParticularType.NonFee)
        {
            using (var unitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories())))
            {
                return unitOfWork.Accounts.Get(x => (x.Type == type) && (x.ParticularType == (int)particularType)
                                && (x.Currency == currency) && (x.TransactionType == transactionType) && (x.ValueFor == valueFor)).
                                Select(x => x.Id).FirstOrDefault();
            }
        }
        public static double GetDayNumber(DateTime date)
        {
            double dayNo = ACC_START_DAY_NO + (date - ACC_START_DATE).TotalDays;
            return dayNo;
        }
        public static IEnumerable<DaybookRecord> GetDayBook(DateTime date)
        {
            double dayNo = GetDayNumber(date);
            using (var unitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories())))
            {
                var transQry = unitOfWork.Transactions.GetAll();
                var transactions = from trn in transQry
                                   where DbFunctions.TruncateTime(trn.Completed) == DbFunctions.TruncateTime(date)
                                   select new TransRefModel
                                   {
                                       OrderId = trn.OrderId,
                                       Amount = trn.Amount,
                                       TransactionOn = trn.Completed,

                                       RateHome = trn.Order.RateHome, // Transact Currency {==trn.Currency}
                                       RateBooks = trn.Order.RateBooks, // EUR/DKK - 1EUR = ? DKK
                                       RateBase = trn.Order.RateBase, // Crypto -> EUR Rate - 1CRYPTO CUR = ?EUR

                                       Currency = trn.CurrencyRef.Code,
                                       CurrencyType = trn.CurrencyRef.CurrencyTypeId,

                                       FromAccount = trn.FromAccount,
                                       From = trn.FromAccountRef.Text,
                                       ToAccount = trn.ToAccount,
                                       To = trn.ToAccountRef.Text,
                                   };

                Dictionary<string, DaybookRecord> daybook = new Dictionary<string, DaybookRecord>();

                //here sign accepts either -1 or 1 , -1 indicates fromaccount and 1 indicates toaccount
                Action<string, TransRefModel, int> updatePrice = (string account, TransRefModel trn, int sign) =>
                 {
                     if (!trn.Amount.HasValue || !trn.RateBase.HasValue || !trn.RateBooks.HasValue || !trn.RateHome.HasValue)
                     {
                         AuditLog.log($"Invalid Rates and Tx amount Account:{account} OrderId:{trn.OrderId} Amount:{trn.Amount} RateBase:{trn.RateBase} RateBooks:{trn.RateBooks} RateHome:{trn.RateHome}", (int)AuditLogStatus.ApplicationError, (int)WMC.Data.Enums.AuditTrailLevel.Error);
                     }

                     daybook[account].DayNumber = dayNo;

                     decimal amount = 0M;
                     if (trn.CurrencyType.Value == (long)CurrencyTypes.Digital)
                     {
                         amount = trn.Amount.GetValueOrDefault() * trn.RateBase.GetValueOrDefault() * trn.RateBooks.GetValueOrDefault();
                     }
                     else
                     {
                         if (!trn.RateBase.HasValue || !trn.RateBooks.HasValue || !trn.RateHome.HasValue)
                         {
                             amount = -9999;
                         }
                         else
                         {
                             amount = (trn.Amount.GetValueOrDefault() / trn.RateHome.GetValueOrDefault()) * trn.RateBooks.GetValueOrDefault();
                         }
                     }

                     daybook[account].Time = trn.TransactionOn.Value;
                     // daybook[account].Currency = trn.Currency; // always converted to Rate Books
                     daybook[account].Amount += amount * sign; // minus amount
                     daybook[account].Amount2 = daybook[account].Amount;


                 };

                foreach (var acc in transactions)
                {
                    if (IsOrderCompleted(acc.OrderId))
                    {
                        if (acc.FromAccount.GetValueOrDefault() > 0)
                        {
                            string account = acc.From;
                            if (!daybook.ContainsKey(account))
                            {
                                daybook.Add(account, new DaybookRecord() { AccountText = Convert.ToInt32(account) });
                            }

                            updatePrice(account, acc, -1); // minus amount
                        }

                        if (acc.ToAccount.GetValueOrDefault() > 0)
                        {
                            string account = acc.To;
                            if (!daybook.ContainsKey(account))
                            {
                                daybook.Add(account, new DaybookRecord() { AccountText = Convert.ToInt32(account) });
                            }

                            updatePrice(account, acc, 1); // plus amount
                        }
                    }
                }
                return daybook.Values;
            }
        }

        public static bool IsOrderCompleted(long? orderId)
        {
            using (var unitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories())))
            {
                var getOrder = unitOfWork.Orders.GetById((long)orderId).Status;
                if (getOrder == (int)WMC.Data.Enums.OrderStatus.Completed)
                {
                    return true;
                }
            }
            return false;
        }
    }


    public class TransRefModel
    {
        public long? OrderId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionOn { get; set; }
        public decimal? RateHome { get; set; }
        public decimal? RateBooks { get; set; }
        public decimal? RateBase { get; set; }
        public string Currency { get; set; }
        public long? CurrencyType { get; set; }
        public long? FromAccount { get; set; }
        public string From { get; set; }
        public long? ToAccount { get; set; }
        public string To { get; set; }
    }

    public class DaybookRecord
    {
        public static string Seperator = ";";
        public DaybookRecord()
        {
            this.RecordType = "Finansbilag";
            this.Currency = "DKK";
        }

        public string RecordType { get; set; } // Finansbilag
        public DateTime Time { get; set; } // Datetime
        public int AccountText { get; set; } // Account Number
        public double DayNumber { get; set; } // Day Number
        public string Column5 { get; set; } // Blank
        public decimal Amount { get; set; } // Amount DKK
        public string Currency { get; set; } // Currency, Fixed DKK
        public decimal Amount2 { get; set; } // Amount DKK (Can't fix)

        public static string[] Columns = new string[] { "RecordType", "Date", "Account", "Day", "Column5", "Amount", "Currency", "Column8" };
        public override string ToString()
        {
            return string.Join(Seperator, this.RecordType, this.Time.ToString(AccountingUtil.DATEFORMAT), this.AccountText, this.DayNumber, this.Column5, this.Amount, this.Currency, this.Amount2);
        }
    }
}

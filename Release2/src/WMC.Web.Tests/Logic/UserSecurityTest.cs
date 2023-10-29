using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;
using WMC.Logic;
using WMC.Logic.Accounting;
using static WMC.Data.Enums.OrderType;
using static WMC.Data.Enums.OrderPaymentType;
using WMC.Data.Enums;
using Newtonsoft.Json;

namespace WMC.Web.Tests.Logic
{
    [TestClass]
    public class UserSecurityTest
    {
        [TestMethod]
        public void testOrder()
        {
            var orderLogic = new OrderLogic();
            orderLogic.ProcessOrder();
        }

        //public void TestAccountUtils()
        //{
        //    var accUtils = new AccountingUtil();
        //    var dateTime = new DateTime(2020, 5, 4);
        //    accUtils.GetDayBook(dateTime);
        //}

        [TestMethod]
        public void testKycfiles()
        {
        }
            [TestMethod]
        public void changeTx()
        {
            var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            var getTx = dataUnitOfWork.Transactions.Get(x => x.Id == 21339 && x.Type == Constansts.Transactions.IncomingType).FirstOrDefault();
            getTx.Completed = DateTime.Now;
           //dataUnitOfWork.Transactions.Update(getTx);
            dataUnitOfWork.Commit();
        }
        [TestMethod]
        public void TestFlags()
        {
            CurrencyAcceptance bank = (CurrencyAcceptance.BankBuy | CurrencyAcceptance.BankSell);
            CurrencyAcceptance creditCard = CurrencyAcceptance.BankBuy | CurrencyAcceptance.BankSell | CurrencyAcceptance.CreditCard;
            var bankValue = CurrencyAcceptance.BankBuy & CurrencyAcceptance.BankSell;  
            // 1 | 2 => (01 | 10) => 11 => 3 Bank
            // 1 | 2 | 4 => (001 | 010) | 100  =>  011 | 100 => 111 => 7 Bank and Creditcard     
            // 0 | 0 | 4 =>  000 | 100  => 100 => 4 CreditCard
            // 0 | 0 | 0 =>  000  => 0 None
                                                                                    
        }

        [TestMethod]
        public void insertTx()
        {

            //dynamic sendBTCResponseContent = JsonConvert.DeserializeObject(sendBTCResponse);
            int bitGoWalletDetection = -430410; //sendBTCResponseContent.transfer.value;
            int bitGoWalletMinerFees = Convert.ToInt32(14728); //sendBTCResponseContent.transfer.feeString
            decimal absoluteBitGoWalletDetection = Convert.ToDecimal((Math.Abs(bitGoWalletDetection))) / 100000000;
            decimal minerFees = Convert.ToDecimal(bitGoWalletMinerFees) / 100000000; //todo 100000000 move to constant
            int bitGoPayAsYouGo = 0;//sendBTCResponseContent.transfer.payGoFee;
            decimal absoluteBitGoPayAsYouGo = Convert.ToDecimal(Math.Abs(bitGoPayAsYouGo)) / 100000000;
            string uuid = CommonUUID.UUID();
            //var FromAccountTx1 = 3;//WMC.Logic.Accounting.AccountingUtil.GetAccountId(1, 26, AccountValueFor.FromAccount, 2, ParticularType.NonFee);
            //var ToAccountTx1 = null;
            //var FromAccountTx2 = null;//WMC.Logic.Accounting.AccountingUtil.GetAccountId(1, 26, AccountValueFor.FromAccount, 2, ParticularType.NonFee);
            //var ToAccountTx2 = 9;
            //var FromAccountTx3 = null;//WMC.Logic.Accounting.AccountingUtil.GetAccountId(1, 26, AccountValueFor.FromAccount, 2, ParticularType.NonFee);
            //var ToAccountTx3 = 29;
            //var FromAccountTx4 = null;//WMC.Logic.Accounting.AccountingUtil.GetAccountId(1, 26, AccountValueFor.FromAccount, 2, ParticularType.NonFee);
            //var ToAccountTx4 = 30;
        }
        [TestMethod]
        public void TestSiteNames()
        {
            string[] hostnames = new string[] { "localhost", "test.app.monni.com", "app.monni.com", "monni.com" };

            foreach (var hostname in hostnames)
            {
                string SiteName = hostname.Contains("localhost") ? hostname : hostname.Contains("test") ? hostname.Split('.')[2] + "." + hostname.Split('.')[3] : hostname.Contains("app") ? hostname.Split('.')[1] + "." + hostname.Split('.')[2] : hostname;
            }

            //string SiteName1 = hostname1.Contains("localhost") ? hostname1 : hostname1.Split('.')[1] + "." + hostname1.Split('.')[2] + "." + hostname1.Split('.')[3];

//            string SiteName2 = hostname2.Contains("test") ?  hostname2.Split('.')[1] + "." + hostname2.Split('.')[2] : hostname1.Split('.')[1] + "." + hostname2.Split('.')[2];
//
  //          string SiteName3 = !hostname3.Contains("test") ? hostname3 : hostname2.Split('.')[1] + "." + hostname3.Split('.')[2];
        }

        [TestMethod]
        public void TestAmount()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            long amount = Convert.ToInt64(200.000 * 100);
            Assert.AreEqual(amount, 20000);
            amount = Convert.ToInt64(200.2345 * 100);
            Assert.AreEqual(amount, 20023);
            amount = Convert.ToInt64(200.455 * 100);
            Assert.AreEqual(amount, 20046);
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("da-DK");
            amount = Convert.ToInt64(200.000 * 100);
            Assert.AreEqual(amount, 20000);
            amount = Convert.ToInt64(200.2345 * 100);
            Assert.AreEqual(amount, 20023);
            amount = Convert.ToInt64(200.455 * 100);
            Assert.AreEqual(amount, 20046);
        }


        [TestMethod]
        public void PWDTest()
        {
            string wrongPassword = "compu@ueen";
            string password = "Compu@ueen";
            string salt;
            string passwordHash = SecurityUtil.EncryptPassword(password, out salt);
            bool valid = SecurityUtil.ValidatePassword(password, salt, passwordHash);
            Assert.IsTrue(valid);
            bool notvalid = SecurityUtil.ValidatePassword(wrongPassword, salt, passwordHash);
            Assert.IsFalse(notvalid);
        }

        [TestMethod]
        public void UpdatePassword()
        {
            using (MonniData context = new MonniData())
            {

                var users = context.Users.Where(x => x.Password != null && x.Password.Length > 0);
                foreach (var user in users)
                {
                    string salt;
                    string passwordHash = SecurityUtil.EncryptPassword(user.Password, out salt);
                    user.Password = passwordHash;
                    user.PasswordSalt = salt;
                }

                context.SaveChanges();
            }
        }
        [TestMethod]
        public void testExportCSV()
        {
        }
    }
}

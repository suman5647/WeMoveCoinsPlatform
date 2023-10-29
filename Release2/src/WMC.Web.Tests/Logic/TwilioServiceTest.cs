using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Twilio;
using WMC.Logic;

namespace WMC.Web.Tests.Logic
{
    [TestClass]
    public class TwilioServiceTest
    {
        public TwilioServiceTest() { }

        [TestInitialize]
        public void Init() { }

        //[TestMethod]
        //public void TestSMS_Test_ShouldReturn_CustomMessage()
        //{
        //    var accountid = "ACCOUNT_ID";
        //    var fromNo = "+4592451774";
        //    var authtoken = "AUTH_Token";
        //    var to = "9686622751";
        //    var otp = "654321";
        //    var uname = "Shiju Madamchery";
        //    var messageFormat = "a message to {0}, should show Otp {1}";
        //    var expectedMessage = string.Format("a message to {0}, should show Otp {1}", uname, otp);

        //    // ICacheObject cacheMock = Mock.Of<ICacheObject>();
        //    ISettingsManager settingMock = Mock.Of<ISettingsManager>();

        //    Mock.Get(settingMock).Setup(x => x.Get(TwilioService.KEY_TWILIO_TEST_OR_PROD, false)).Returns(new SettingsValue() { Key = TwilioService.KEY_TWILIO_TEST_OR_PROD, Value = "Test" });
        //    Mock.Get(settingMock).Setup(x => x.Get(TwilioService.KEY_TWILIO_TEST_SETTINGS, false)).Returns(new SettingsValue() { Key = TwilioService.KEY_TWILIO_TEST_OR_PROD, Value = "{ " + $"'From' : 'WeMoveCoins', 'FromNumber' : '{fromNo}', 'AccountSid' : '{accountid}', 'AuthToken' : '{authtoken}'," + " 'Message' : 'Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}' }" });

        //    TwilioService twilio = new TwilioService(settingMock);

        //    Assert.IsTrue(twilio.IsConfigured());
        //    Assert.IsTrue(twilio.IsTest());

        //    var message = twilio.SendMessage(to, messageFormat, null, uname, otp);

        //    Assert.IsNotNull(message);
        //    Assert.IsInstanceOfType(message, typeof(Message));
        //    Message tiliomeg = (Message)message;
        //    Assert.AreEqual(tiliomeg.To, to);
        //    Assert.AreEqual(tiliomeg.From, fromNo);
        //    Assert.AreEqual(tiliomeg.AccountSid, accountid);
        //    Assert.AreEqual(tiliomeg.Body, expectedMessage);
        //}
    }
}

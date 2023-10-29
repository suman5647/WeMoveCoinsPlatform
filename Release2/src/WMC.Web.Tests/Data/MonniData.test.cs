using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;

namespace WMC.Web.Tests.Data
{
    [TestClass]
    public class MonniDataTest
    {
        [TestMethod]
        public void testData()
        {
            MonniData dataContext = new MonniData();
            var user = dataContext.Users.FirstOrDefault();
            var userTire = user.Tier;
        }
    }
}

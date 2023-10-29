using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WMC.Data;
using WMC.Helpers;
using WMC.Logic;
using System.Linq;

namespace WMC.Web.Tests.Logic
{
    [TestClass]
    public class StringResourceServiceTest
    {

        public StringResourceServiceTest() { }

        [TestInitialize]
        public void Init() { }


        [TestMethod]
        public void TestOrderNumberGeneration()
        {
            var newOrderNumber = getUniqueOrderNumber();
        }

        private static volatile object syncRoot = new object();
        private long getUniqueOrderNumber()
        {
            long new_OrderId = 0;
            lock (syncRoot)
            {
                var dataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
                List<long> triedIds = new List<long>();
                bool idExists = true;
                do
                {
                    List<long> newIds = new List<long>();
                    do
                    {
                        new_OrderId = new Random().Next(100000, 999999);
                    }
                    while (triedIds.Contains(new_OrderId));

                    triedIds.Add(new_OrderId);

                    //if newly generated OrderId is already present in Order table then again generate new random number
                    idExists = dataUnitOfWork.Orders.Get(q => q.Number == new_OrderId.ToString()).Select(x => x.Number).Any();
                }
                while (idExists);
            }

            return new_OrderId;
        }

        [TestMethod]
        public void StringResource_Replace_MustReplaceWithGivenLookup()
        {
            string resourceKey = "TEST_Resource_Key";
            string resourceValue = "TEST Resource Value hello {{name}} your otp is {0}";
            string name = "Shiju Madamchery";
            string otp = "654321";
            Dictionary<string, string> lookup = new Dictionary<string, string>() { { "{{name}}", name } };
            string expectedResourceValue = $"TEST Resource Value hello {name} your otp is " + "{0}";
            string expectedFromatedResourceValue = $"TEST Resource Value hello {name} your otp is {otp}";

            IResourceManager settingMock = Mock.Of<IResourceManager>();
            Mock.Get(settingMock).Setup(x => x.Get(resourceKey, "")).Returns(new ResourceValue() { Key = resourceKey, Value = resourceValue });

            StringResourceService stringResources = new StringResourceService(settingMock);
            var resValue = stringResources.Get(resourceKey, "", lookup);

            Assert.IsNotNull(resValue);
            Assert.AreEqual(resValue.Key, resourceKey);
            Assert.AreEqual(resValue.Value, expectedResourceValue);
            Assert.AreEqual(resValue.Format(otp), expectedFromatedResourceValue);
        }
    }
}

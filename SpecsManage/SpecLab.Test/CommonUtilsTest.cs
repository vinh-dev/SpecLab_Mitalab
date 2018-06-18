using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Database;
using SpecLab.Business.Services;
using SpecLab.Business;
using log4net;

namespace SpecLab.Test
{
    [TestClass]
    public class CommonUtilsTest
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        

        [TestMethod]
        public void Test_EncryptData()
        {
            string testStr = "speclab";
            string encryptData = CommonUtils.EncryptData(testStr);
            string decryptData = CommonUtils.Decrypt(encryptData);

            Assert.AreEqual(testStr, decryptData);

            testStr = "SpecLab.vsdt@123";
            encryptData = CommonUtils.EncryptData(testStr);
            decryptData = CommonUtils.Decrypt(encryptData);

            Assert.AreEqual(testStr, decryptData);
        }
    }
}

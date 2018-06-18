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
    public class UserServiceTest
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            var temp = testContext.TestDir;
            log4net.Config.XmlConfigurator.Configure(
                new FileInfo(@"D:\home\mitalab\SpecimenManagement\source\SpecsManage\SpecLab.Test\SpecLab.Test.log4net.xml")); 
        }

        [TestMethod]
        public void Test_GeneratePasswordSystemAdmin()
        {
            UserService service = new UserService();
            string result = service.GeneratePasswordSystemAdmin();

            _logger.DebugFormat("New Password for SystemAdmin:{0}", result);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_GetSystemAdminLoginInfo()
        {
            UserService service = new UserService();

            LoginUserInfo loginUserInfo = service.GetSystemAdminLoginInfo();

            Assert.IsNotNull(loginUserInfo);
            Assert.IsTrue(loginUserInfo.Rights.Count == Enum.GetValues(typeof(UserRightCode)).Length);
        }

        [TestMethod]
        public void Test_GeneratePasswordHash()
        {
            UserService service = new UserService();
            string hash = service.GeneratePasswordHash("test", "12345678");

            Assert.AreEqual("12390e65b2efbb08612dbeb26a321e3d", hash);
        }

        [TestMethod]
        public void Test_CreateNewUser_GetLoginUserInfo_DeleteUser()
        {
            UserService service = new UserService();
            var result = service.CreateNewUser(new LoginUserInfo()
            {
                FullName = "SpecLab.Test",
                UserId = "test" /*+ Guid.NewGuid().ToString()*/,
                Status = UserStatus.Enable,
                Rights = new List<UserRightCode>(CommonUtils.GetListDefaultUserRightCode())
            }, "SpecLab@123");

            Assert.IsNotNull(result);

            service.GetLoginUserInfo(result.UserId);

            //Assert.IsTrue(service.DeleteUser(result.UserId));
        }

        [TestMethod]
        public void Test_ResetPassword_Login()
        {
            UserService service = new UserService();

            service.ResetPassword(new UserService.ResetPassParams()
                                      {
                                          UserId = "test",
                                          NewPass = "123456"
                                      });

            service.Login("test", "123456");
        }

        [TestMethod]
        public void Test_CreateNewUser_UpdateUser_DeletesUser()
        {
            string userId = "test" + Guid.NewGuid().ToString();
            string oldName = "SpecLab.Test";
            string newName = "SpecLab.Test.Update";

            UserService service = new UserService();
            var result = service.CreateNewUser(new LoginUserInfo()
            {
                FullName = oldName,
                UserId = userId,
                Status = UserStatus.Enable,
                Rights = new List<UserRightCode>(CommonUtils.GetListDefaultUserRightCode())
            }, "SpecLab@123");

            Assert.IsNotNull(result);
            Assert.AreEqual(oldName, result.FullName);

            result = service.GetLoginUserInfo(userId);
            result.FullName = newName;

            result = service.UpdateUser(result);
            Assert.AreEqual(newName, result.FullName);

            Assert.IsTrue(service.DeleteUser(result.UserId));
        }
    }
}

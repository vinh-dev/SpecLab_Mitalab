using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Database;
using Spring.Context.Support;
using log4net;

namespace SpecLab.Business.Services
{

    public class UserService : BaseService
    {
        private class CreateNewUserParams
        {
            public LoginUserInfo LoginUserInfo { get; set; }
            public string Password { get; set; }
        }

        public class LoginParams
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class ChangePassParams
        {
            public string UserId { get; set; }
            public string OldPass { get; set; }
            public string NewPass { get; set; }
        }

        public class ResetPassParams
        {
            public string UserId { get; set; }
            public string NewPass { get; set; }
        }

        public class UpdateRightParams
        {
            public string UserId { get; set; }
            public List<UserRightCode> UpdateRights { get; set; }

            public UpdateRightParams()
            {
                UpdateRights = new List<UserRightCode>();
            }
        }

        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static RNGCryptoServiceProvider _cryptoServiceProvider = new RNGCryptoServiceProvider();

        private UserRightCode ConvertUserRightCode(UserRight userRight)
        {
            return CommonUtils.GetEnumFromValue<UserRightCode>(userRight.RightCode);
        }

        internal LoginUserInfo GetSystemAdminLoginInfo()
        {
            LoginUserInfo loginUserInfo = new LoginUserInfo()
            {
                UserId = CommonConstant.SystemAdmin,
                FullName = CommonConstant.SystemAdmin,
                Status = UserStatus.Enable,
                Rights = new List<UserRightCode>()
            };

            foreach (UserRightCode right in Enum.GetValues(typeof(UserRightCode)))
            {
                loginUserInfo.Rights.Add(right);
            }

            return loginUserInfo;
        }

        private string ConvertFromHashToString(byte[] hash)
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                strBuilder.Append(hash[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        internal string GeneratePasswordSystemAdmin()
        {
            string salt = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
                .ToString(CommonConstant.DateFormatDisplay);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(CommonConstant.ApplicationId + salt));

            return ConvertFromHashToString(hash);
        }

        private void GetLoginUserInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<LoginParams, LoginUserInfo> paramCommand)
        {
            if (paramCommand.CallingInfo.Username.ToLower().Equals(CommonConstant.SystemAdmin.ToLower()))
            {
                string password = GeneratePasswordSystemAdmin();
                if(paramCommand.CallingInfo.Password.Equals(password))
                {
                    paramCommand.ReturnValue = this.GetSystemAdminLoginInfo();
                } 
                else
                {
                    throw new BusinessException(ErrorCode.PasswordNotMatch);
                }
            } 
            else
            {
                var existsUser = _entities.UserInfoes.FirstOrDefault(u => u.UserId.ToLower()
                        .Equals(paramCommand.CallingInfo.Username.ToLower()));

                if (existsUser == null)
                {
                    throw new BusinessException(ErrorCode.UserIdNotExists);
                }
                else if (!existsUser.Status.ToString().Equals(UserStatus.Enable.ToString()))
                {
                    throw new BusinessException(ErrorCode.UserNotActive);
                }
                else
                {
                    string passwordHash = GeneratePasswordHash(
                        paramCommand.CallingInfo.Password,
                        existsUser.PasswordSalt);
                    if (!passwordHash.Equals(existsUser.Password))
                    {
                        throw new BusinessException(ErrorCode.PasswordNotMatch);
                    }

                    paramCommand.ReturnValue = this.GetLoginUserInfo(paramCommand.CallingInfo.Username);
                }
            }
        }

        public LoginUserInfo Login(string username, string password)
        {
            var command = new DatabaseCommand<LoginParams, LoginUserInfo>()
            {
                CallingInfo = new LoginParams()
                {
                    Username = username,
                    Password = password
                }
            };
            this.ProxyCalling(GetLoginUserInfoProxy, command);
            return command.ReturnValue;
        }

        internal string GeneratePasswordHash(string password, string salt)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(
                password + salt));

            string paswordHash = ConvertFromHashToString(hash);
            return paswordHash;
        }

        private void GetLoginUserInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<string, LoginUserInfo> paramCommand)
        {
            if (paramCommand.CallingInfo != null)
            {
                if (CommonConstant.SystemAdmin.ToLower().Equals(paramCommand.CallingInfo.ToLower()))
                {
                    paramCommand.ReturnValue = GetSystemAdminLoginInfo();
                }
                else
                {
                    var dbItem = _entities.UserInfoes
                            .FirstOrDefault(u => u.UserId.ToLower().Equals(paramCommand.CallingInfo.ToLower()));

                    if (dbItem != null)
                    {
                        var existsUser = new LoginUserInfo()
                        {
                            FullName = dbItem.FullName,
                            UserId = dbItem.UserId,
                            Status = CommonUtils.GetEnumFromValue<UserStatus>(dbItem.Status)
                        };

                        var rights = dbItem.UserRights.ToList();

                        existsUser.Rights.AddRange(dbItem.UserRights.ToList().ConvertAll(ConvertUserRightCode));

                        paramCommand.ReturnValue = existsUser;
                    }
                }
            }
        }

        public LoginUserInfo GetLoginUserInfo(string userId)
        {
            var command = new DatabaseCommand<string, LoginUserInfo>()
            {
                CallingInfo = userId
            };
            this.ProxyCalling(GetLoginUserInfoProxy, command);
            return command.ReturnValue;
        }

        private void DeleteUserProxy(SpecLabEntities _entities,
            DatabaseCommand<string, bool> paramCommand)
        {
            var userInfo = (from u in _entities.UserInfoes
                            where u.UserId.Equals(paramCommand.CallingInfo)
                            select u).FirstOrDefault();

            var listRightDelete = userInfo.UserRights.ToList();

            foreach (var rightDelete in listRightDelete)
            {
                _entities.UserRights.Remove(rightDelete);
            }
            _entities.UserInfoes.Remove(userInfo);
            _entities.SaveChanges();

            paramCommand.ReturnValue = true;
        }

        public bool DeleteUser(string userId)
        {
            var command = new DatabaseCommand<string, bool>()
            {
                CallingInfo = userId
            };
            this.ProxyCalling(DeleteUserProxy, command);
            return command.ReturnValue;
        }

        private void CreateNewUserProxy(SpecLabEntities _entities,
            DatabaseCommand<CreateNewUserParams, LoginUserInfo> paramCommand)
        {
            if (paramCommand.CallingInfo.LoginUserInfo.UserId.Equals(CommonConstant.SystemAdmin))
            {
                throw new BusinessException(ErrorCode.UserIdExists);
            }

            var existsUser = _entities.UserInfoes
                .FirstOrDefault(u => u.UserId.ToLower()
                    .Equals(paramCommand.CallingInfo.LoginUserInfo.UserId.ToLower()));

            if (existsUser != null)
            {
                throw new BusinessException(ErrorCode.UserIdExists);
            }
            else
            {
                string passwordSalt = Guid.NewGuid().ToString();
                string paswordHash = GeneratePasswordHash(paramCommand.CallingInfo.Password, passwordSalt);

                var dbItem = new UserInfo()
                {
                    UserId = paramCommand.CallingInfo.LoginUserInfo.UserId,
                    FullName = paramCommand.CallingInfo.LoginUserInfo.FullName,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Password = paswordHash,
                    PasswordSalt = passwordSalt,
                    Status = UserStatus.Enable.ToString()
                };
                _entities.UserInfoes.Add(dbItem);

                foreach (var rightCode in paramCommand.CallingInfo.LoginUserInfo.Rights)
                {
                    _entities.UserRights.Add(new UserRight()
                    {
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        UserId = paramCommand.CallingInfo.LoginUserInfo.UserId,
                        RightCode = rightCode.ToString()
                    });
                }

                _entities.SaveChanges();

                paramCommand.ReturnValue = this.GetLoginUserInfo(paramCommand.CallingInfo.LoginUserInfo.UserId);
            }
        }

        public LoginUserInfo CreateNewUser(LoginUserInfo loginUserInfo, string password)
        {
            var command = new DatabaseCommand<CreateNewUserParams, LoginUserInfo>()
            {
                CallingInfo = new CreateNewUserParams()
                {
                    LoginUserInfo = loginUserInfo,
                    Password = password
                }
            };
            this.ProxyCalling(CreateNewUserProxy, command);
            return command.ReturnValue;
        }

        private void UpdateUserProxy(SpecLabEntities _entities,
            DatabaseCommand<LoginUserInfo, LoginUserInfo> paramCommand)
        {
            var dbItem = _entities.UserInfoes
                .FirstOrDefault(c => c.UserId.ToLower().Equals(
                    paramCommand.CallingInfo.UserId));

            dbItem.FullName = paramCommand.CallingInfo.FullName;
            _entities.SaveChanges();

            paramCommand.ReturnValue = this.GetLoginUserInfo(paramCommand.CallingInfo.UserId);
        }

        public LoginUserInfo UpdateUser(LoginUserInfo loginUserInfo)
        {
            var command = new DatabaseCommand<LoginUserInfo, LoginUserInfo>()
            {
                CallingInfo = loginUserInfo
            };
            this.ProxyCalling(UpdateUserProxy, command);
            return command.ReturnValue;
        }

        private void ChangePasswordProxy(SpecLabEntities _entities,
            DatabaseCommand<ChangePassParams, bool> paramCommand)
        {
            paramCommand.ReturnValue = false;

            var selectedUser = (from u in _entities.UserInfoes
                        where u.UserId.ToLower().Equals(paramCommand.CallingInfo.UserId.ToLower())
                        select u).FirstOrDefault();

            if(selectedUser == null)
            {
                throw new BusinessException(ErrorCode.UserIdNotExists);
            }
            else
            {
                string passwordHash = GeneratePasswordHash(
                    paramCommand.CallingInfo.OldPass, selectedUser.PasswordSalt);
                if (!passwordHash.Equals(selectedUser.Password))
                {
                    throw new BusinessException(ErrorCode.PasswordNotMatch);
                }
                else
                {
                    string passwordSalt = Guid.NewGuid().ToString();
                    passwordHash = GeneratePasswordHash(paramCommand.CallingInfo.NewPass, passwordSalt);

                    selectedUser.Password = passwordHash;
                    selectedUser.PasswordSalt = passwordSalt;

                    _entities.SaveChanges();

                    paramCommand.ReturnValue = true;
                }
            }
        }

        public bool ChangePassword(ChangePassParams passParams)
        {
            var command = new DatabaseCommand<ChangePassParams, bool>()
            {
                CallingInfo = passParams
            };
            this.ProxyCalling(ChangePasswordProxy, command);
            return command.ReturnValue;
        }

        private void ResetPasswordProxy(SpecLabEntities _entities,
            DatabaseCommand<ResetPassParams, bool> paramCommand)
        {
            paramCommand.ReturnValue = false;

            var selectedUser = (from u in _entities.UserInfoes
                                where u.UserId.ToLower().Equals(paramCommand.CallingInfo.UserId.ToLower())
                                select u).FirstOrDefault();

            if (selectedUser == null)
            {
                throw new BusinessException(ErrorCode.UserIdNotExists);
            }
            else
            {
                selectedUser.PasswordSalt = Guid.NewGuid().ToString();
                selectedUser.Password = GeneratePasswordHash(
                    paramCommand.CallingInfo.NewPass, selectedUser.PasswordSalt);
                _entities.SaveChanges();

                paramCommand.ReturnValue = true;
            }
        }

        public bool ResetPassword(ResetPassParams passParams)
        {
            var command = new DatabaseCommand<ResetPassParams, bool>()
            {
                CallingInfo = passParams
            };
            this.ProxyCalling(ResetPasswordProxy, command);
            return command.ReturnValue;
        }

        private void GetAllShortUserInfoProxy(SpecLabEntities _entities,
            DatabaseCommand<object, List<ShortUserInfo>> paramCommand)
        {
            var userStatusEnable = UserStatus.Enable.ToString();
            var listUsers = (from u in _entities.UserInfoes
                             where u.Status.Equals(userStatusEnable) 
                             select u).ToList();

            paramCommand.ReturnValue = listUsers.ConvertAll(ConvertToShortUserInfo);
        }

        private ShortUserInfo ConvertToShortUserInfo(UserInfo userInfo)
        {
            return new ShortUserInfo()
                       {
                           UserId = userInfo.UserId,
                           FullName = userInfo.FullName
                       };
        }

        public List<ShortUserInfo> GetAllShortUserInfo()
        {
            var command = new DatabaseCommand<object, List<ShortUserInfo>>()
            {
                CallingInfo = null,
                ReturnValue = new List<ShortUserInfo>()
            };
            this.ProxyCalling(GetAllShortUserInfoProxy, command);
            return command.ReturnValue;
        }

        private void UpdateUserRightsProxy(SpecLabEntities _entities,
            DatabaseCommand<UpdateRightParams, bool> paramCommand)
        {
            var listRights = (from u in _entities.UserRights
                              where u.UserInfo.UserId.ToLower().Equals(paramCommand.CallingInfo.UserId.ToLower())
                              select u);

            foreach (var right in listRights)
            {
                _entities.UserRights.Remove(right);
            }
            _entities.SaveChanges();

            foreach (var rightCode in paramCommand.CallingInfo.UpdateRights)
            {
                _entities.UserRights.Add(new UserRight()
                {
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    UserId = paramCommand.CallingInfo.UserId,
                    RightCode = rightCode.ToString()
                });
            }
            _entities.SaveChanges();

            paramCommand.ReturnValue = true;
        }

        public bool UpdateUserRights(UpdateRightParams rightParams)
        {
            var command = new DatabaseCommand<UpdateRightParams, bool>()
            {
                CallingInfo = rightParams,
                ReturnValue = false
            };
            this.ProxyCalling(UpdateUserRightsProxy, command);
            return command.ReturnValue;
        }
    }
}

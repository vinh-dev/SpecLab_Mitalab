using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Database;
using SpecLab.FrontEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecLab.Business.Services
{
    public class ContentService : BaseService
    {
        public ContentInfo GetContentInfo(string contentId)
        {
            var command = new DatabaseCommand<string, ContentInfo>()
            {
                CallingInfo = contentId
            };
            this.ProxyCalling(GetContentInfoByIdProxy, command);

            return command.ReturnValue;
        }

        public void SaveContentInfo(ContentInfo contentInfo)
        {
            var command = new DatabaseCommand<ContentInfo, object>()
            {
                CallingInfo = contentInfo
            };
            this.ProxyCalling(SaveContentProxy, command);
        }

        private void SaveContentProxy(SpecLabEntities _entities,
            DatabaseCommand<ContentInfo, object> paramCommand)
        {
            var existContent = (from content in _entities.Contents
                                where content.ContentId == paramCommand.CallingInfo.ContentId
                                select content).FirstOrDefault();

            var loginInfo = SessionUtils.LoginUserInfo;
            if (existContent == null)
            {
                //throw new BusinessException(ErrorCode.ContentIdNotExists, paramCommand.CallingInfo);
                _entities.Contents.Add(new Content()
                {
                    
                    CreateUser = loginInfo == null ? null : loginInfo.UserId,
                    UpdateUser = loginInfo == null ? null : loginInfo.UserId, 
                    UpdateDate = DateTime.Now,
                    CreateDate = DateTime.Now,
                    ContentId = paramCommand.CallingInfo.ContentId, 
                    ContentText = paramCommand.CallingInfo.ContentText
                });
                _entities.SaveChanges();

            }
            else
            {
                existContent.UpdateDate = DateTime.Now;
                existContent.UpdateUser = loginInfo == null ? null : loginInfo.UserId;
                existContent.ContentText = paramCommand.CallingInfo.ContentText;
                _entities.SaveChanges();
            }
        }

        private void GetContentInfoByIdProxy(SpecLabEntities _entities,
            DatabaseCommand<string, ContentInfo> paramCommand)
        {
            var existContent = (from u in _entities.Contents
                                where u.ContentId == paramCommand.CallingInfo
                                select new ContentInfo()
                                {
                                    ContentId = u.ContentId,
                                    ContentText = u.ContentText
                                }).FirstOrDefault();

            if (existContent == null)
            {
                throw new BusinessException(ErrorCode.ContentIdNotExists, paramCommand.CallingInfo);
            }
            else
            {
                paramCommand.ReturnValue = existContent;
            }
        }
    }
}

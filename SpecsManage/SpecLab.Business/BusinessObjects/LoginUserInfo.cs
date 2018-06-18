using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.Database;

namespace SpecLab.Business.BusinessObjects
{
    public class LoginUserInfo
    {
        public string UserId { get; set; }

        public string FullName { get; set; }

        public UserStatus Status { get; set; }

        public List<UserRightCode> Rights { get; set; }

        public LoginUserInfo()
        {
            Rights = new List<UserRightCode>();
        }

        public bool IsAuthorize(UserRightCode rightCode)
        {
            return Rights.Contains(rightCode);
        }
    }
}

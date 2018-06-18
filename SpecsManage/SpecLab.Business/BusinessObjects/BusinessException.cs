using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;

namespace SpecLab.Business.BusinessObjects
{
    public class BusinessException : Exception
    {
        public ErrorCode ErrorCode { get; set; }

        public object[] ErrorParams { get; set; }

        public BusinessException(ErrorCode code, params object[] inputParams)
            : base(CommonUtils.GetErrorMessage(code, inputParams))
        {
            this.ErrorCode = code;
            this.ErrorParams = inputParams;
        }

        public BusinessException(ErrorCode code, Exception innerException)
            : base(CommonUtils.GetErrorMessage(code), innerException)
        {
            this.ErrorCode = code;
        }
    }
}

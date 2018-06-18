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

    public class RemovalService : BaseService
    {
        public class GetNumberRemovalParams
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        private void GetNumberRemovalProxy(SpecLabEntities _entities,
            DatabaseCommand<GetNumberRemovalParams, int> paramCommand)
        {
            var query = (from Removal in _entities.Removals
                         select Removal);

            if (paramCommand.CallingInfo.FromDate != null)
            {
                query = query.Where(t => t.RemovalDate >= paramCommand.CallingInfo.FromDate);
            }

            if (paramCommand.CallingInfo.ToDate != null)
            {
                var limitDate = paramCommand.CallingInfo.ToDate.GetValueOrDefault().AddDays(1);
                query = query.Where(t => t.RemovalDate < limitDate);
            }

            paramCommand.ReturnValue = query.Count();
        }

        public int GetNumberRemoval(GetNumberRemovalParams rightParams)
        {
            var command = new DatabaseCommand<GetNumberRemovalParams, int>()
            {
                CallingInfo = rightParams,
                ReturnValue = 0
            };
            this.ProxyCalling(GetNumberRemovalProxy, command);
            return command.ReturnValue;
        }
    }
}

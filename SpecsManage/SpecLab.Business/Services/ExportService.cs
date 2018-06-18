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
using SpecLab.Business.BusinessObjects.Export;

namespace SpecLab.Business.Services
{

    public class ExportService : BaseService
    {
        public class GetTubeExportCountParams
        {
            public List<string> TubeIds { get; set; }

            public GetTubeExportCountParams()
            {
                TubeIds = new List<string>();
            }
        }

        public class GetNumberExportParams
        {
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }

        private void GetNumberExportProxy(SpecLabEntities _entities,
            DatabaseCommand<GetNumberExportParams, int> paramCommand)
        {
            var query = (from export in _entities.Exports
                         select export);

            if (paramCommand.CallingInfo.FromDate != null)
            {
                query = query.Where(t => t.ExportDate >= paramCommand.CallingInfo.FromDate);
            }

            if (paramCommand.CallingInfo.ToDate != null)
            {
                var limitDate = paramCommand.CallingInfo.ToDate.GetValueOrDefault().AddDays(1);
                query = query.Where(t => t.ExportDate < limitDate);
            }

            paramCommand.ReturnValue = query.Count();
        }

        public int GetNumberExport(GetNumberExportParams rightParams)
        {
            var command = new DatabaseCommand<GetNumberExportParams, int>()
            {
                CallingInfo = rightParams,
                ReturnValue = 0
            };
            this.ProxyCalling(GetNumberExportProxy, command);
            return command.ReturnValue;
        }

        public List<TubeExportCount> GetTubeExportCount(GetTubeExportCountParams rightParams)
        {
            var command = new DatabaseCommand<GetTubeExportCountParams, List<TubeExportCount>>()
            {
                CallingInfo = rightParams,
                ReturnValue = new List<TubeExportCount>()
            };
            this.ProxyCalling(GetTubeExportCountProxy, command);
            return command.ReturnValue;
        }

        private void GetTubeExportCountProxy(SpecLabEntities _entities,
            DatabaseCommand<GetTubeExportCountParams, List<TubeExportCount>> paramCommand)
        {
            var query = (from tube in _entities.TubeSamples
                         select new TubeExportCount()
                         {
                             TubeId = tube.TubeId,
                             NumberExport = tube.NumberExport
                         });

            if(paramCommand.CallingInfo.TubeIds.Count > 0)
            {
                query = query.Where(t => paramCommand.CallingInfo.TubeIds.Contains(t.TubeId));
            }

            paramCommand.ReturnValue = query.ToList();
        }
    }
}

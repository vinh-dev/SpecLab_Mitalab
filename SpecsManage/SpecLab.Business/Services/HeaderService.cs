using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.Database;
using log4net;
using System.Reflection;

namespace SpecLab.Business.Services
{
    class HeaderService
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static Header GetHeader(string _labId)
        {
            using (SpecLabEntities _entities = CommonUtils.GetBusinessObject<SpecLabEntities>())
            {
                try
                {
                    var query = _entities.Headers.Where(o => o.LabID == _labId).FirstOrDefault();
                    return query;
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Failed to saved database: {0}", ex));
                    return null;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using SpecLab.Business.Database;
using log4net;

namespace SpecLab.Business.Services
{
    public class BaseService
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected DatabaseCommand<InVal, ReturnVal> ProxyCalling<InVal, ReturnVal>(
            Action<SpecLabEntities, DatabaseCommand<InVal, ReturnVal>> actionEntity,
            DatabaseCommand<InVal, ReturnVal> dbCommandInfo)
        {
            using (SpecLabEntities _entities = CommonUtils.GetBusinessObject<SpecLabEntities>())
            {
                try
                {
                    actionEntity(_entities, dbCommandInfo);
                    return dbCommandInfo;
                }
                catch (DbUpdateException updateException)
                {
                    _logger.Error(string.Format("Failed to saved database: {0}", updateException));
                    throw new BusinessException(ErrorCode.InternalErrorException, updateException);
                }
                catch (DbEntityValidationException entityValidationException)
                {
                    _logger.Error(string.Format("Failed to saved database: {0}", entityValidationException));
                    foreach (var entityValidationError in entityValidationException.EntityValidationErrors)
                    {
                        foreach (var error in entityValidationError.ValidationErrors)
                        {
                            _logger.ErrorFormat("Failed to saved database: {0}", error.ErrorMessage);
                        }
                    }
                    throw new BusinessException(ErrorCode.InternalErrorException, entityValidationException);
                }
                catch (BusinessException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _logger.Error(string.Format("Iternal error: {0}", exception));
                    throw new BusinessException(ErrorCode.InternalErrorException, exception);
                }
            }
        }
    }
}

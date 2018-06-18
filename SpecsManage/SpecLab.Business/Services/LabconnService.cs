using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.SqlClient;

namespace SpecLab.Business.Services
{
    public class LabconnService
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public class ListPatientCriteria
        {
            public string SID { get; set; }

            public string PatientName { get; set; }

            public DateTime? DateSearch { get; set; }

            public string Sequence { get; set; }
        }

        private const string QueryPatient = "select " +
            "p.SID, p.PatientName, p.Sex, p.LocationID, p.Age, p.DateIN " +
            "from dbo.tbl_Patient p " +
            "where " +
            "p.SID not in (select spec.SampleSpecId from SpecLab.dbo.SampleSpec spec) ";

        private const string QueryPatientCount = "select count(*) " +
            "from dbo.tbl_Patient p " +
            "where " +
            "p.SID not in (select spec.SampleSpecId from SpecLab.dbo.SampleSpec spec) ";

        private const string QueryPatient_DateInSelect = "and p.DateIN >= @startDate and p.DateIN <= @endDate ";
        private const string QueryPatient_SIDSelect = "and p.SID like @sid ";
        private const string QueryPatient_PatientName = "and p.PatientName like @patientName ";
        private const string QueryPatient_Seq = "and p.Seq like @seq ";

        private LabConnSampleInfo ConvertToLabConnSampleInfo(SqlDataReader reader)
        {
            LabConnSampleInfo info = new LabConnSampleInfo()
            {
                SID = reader["SID"] == DBNull.Value ? null : (string)reader["SID"],
                Patientname = reader["PatientName"] == DBNull.Value ? null : (string)reader["PatientName"],
                LocationId = reader["LocationID"] == DBNull.Value ? null : (string)reader["LocationID"],
                Age = reader["Age"] == DBNull.Value ? 0 : (int)reader["Age"],
            };

            var sexStr = reader["Sex"] == DBNull.Value ? null : (string)reader["Sex"];
            if (string.IsNullOrEmpty(sexStr))
            {
                info.Sex = SampleSex.NotSpecifiy;
            }
            else if ((char)SampleSex.Female == sexStr[0])
            {
                info.Sex = SampleSex.Female;
            }
            else if ((char)SampleSex.Male == sexStr[0])
            {
                info.Sex = SampleSex.Male;
            }
            else
            {
                info.Sex = SampleSex.NotSpecifiy;
            }

            info.DateInput = (DateTime)reader["DateIN"];

            return info;
        }

        private void ApplyQueryCriteria(SqlCommand command, ListPatientCriteria criteria)
        {
            SqlParameter parameter = null;

            if (criteria.DateSearch != null)
            {
                const string startDateParamName = "startDate";
                const string endDateParamName = "endDate";

                command.CommandText += QueryPatient_DateInSelect;

                parameter = command.Parameters.Add(new SqlParameter(startDateParamName, SqlDbType.DateTime));
                parameter.Value = criteria.DateSearch;

                parameter = command.Parameters.Add(new SqlParameter(endDateParamName, SqlDbType.DateTime));
                parameter.Value = criteria.DateSearch;
            }

            if (!string.IsNullOrEmpty(criteria.SID))
            {
                const string sidParamName = "sid";

                command.CommandText += QueryPatient_SIDSelect;

                parameter = command.Parameters.Add(new SqlParameter(sidParamName, SqlDbType.NVarChar));
                parameter.Value = string.Format("%{0}%", criteria.SID);
            }

            if (!string.IsNullOrEmpty(criteria.PatientName))
            {
                const string patientNameParamName = "patientName";

                command.CommandText += QueryPatient_PatientName;

                parameter = command.Parameters.Add(new SqlParameter(patientNameParamName, SqlDbType.NVarChar));
                parameter.Value = string.Format("%{0}%", criteria.PatientName);
            }

            if (!string.IsNullOrEmpty(criteria.Sequence))
            {
                const string sequenceParamName = "seq";

                command.CommandText += QueryPatient_Seq;

                parameter = command.Parameters.Add(new SqlParameter(sequenceParamName, SqlDbType.NVarChar));
                parameter.Value = string.Format("{0}%", criteria.Sequence);
            }
        }

        public int GetCountPatientId(ListPatientCriteria criteria)
        {
            int result = 0;
            try
            {
                using (SqlConnection connection = CommonUtils.GetBusinessObject<ConnectionService>().getLabconnectSqlConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(QueryPatientCount, connection))
                    {
                        ApplyQueryCriteria(command, criteria);
                        result = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                throw new BusinessException(ErrorCode.InternalErrorException, exception);
            }

            return result;
        }

        public List<LabConnSampleInfo> GetListPatientId(ListPatientCriteria criteria)
        {
            List<LabConnSampleInfo> result = new List<LabConnSampleInfo>();
            try
            {
                using (SqlConnection connection = CommonUtils.GetBusinessObject<ConnectionService>().getLabconnectSqlConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(QueryPatient, connection))
                    {
                        ApplyQueryCriteria(command, criteria);

                        using(var reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                LabConnSampleInfo info = ConvertToLabConnSampleInfo(reader);
                                result.Add(info);
                            }
                            reader.Close();
                        }
                    }
                }
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("Iternal error {0}", exception));
                throw new BusinessException(ErrorCode.InternalErrorException, exception);
            }

            return result;
        }
    }
}

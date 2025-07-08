using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// user added below
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
using NLog;

namespace SQLJobManagerWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SQLJobMgrWS : ifSQLMgr
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        string con = ConfigurationManager.ConnectionStrings["AppSupportFASSIS"].ConnectionString;
        public Boolean StartJob(string JobName, string myUser)
        {
            //Log debugging information if needed
            logger.Info("StartJob()::StartJob was called");
            logger.Debug("StartJob()::Job submitted: " + JobName);
            logger.Debug("StartJob()::User submitted: " + myUser);

            SqlDataReader rdr = null;
            string IsItGood = "Yes";

            // 1. create a command object identifying
            // the stored procedure
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AppSupportFASSIS"].ConnectionString);
            SqlCommand cmd = new SqlCommand("usp_start_ssis_job", conn);

            // 2. set the command object so it knows
            // to execute a stored procedure
            cmd.CommandType = CommandType.StoredProcedure;

            // 3. add parameter to command, which
            // will be passed to the stored procedure
            cmd.Parameters.Add(new SqlParameter("NetworkUser", myUser));
            cmd.Parameters.Add(new SqlParameter("Jobname", JobName));

            logger.Trace("StartJob()::Before StartJob Try");
            try
            {
                conn.Open();
                // execute the command
                //cmd.ExecuteNonQuery();
                rdr = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "StartJob()::Something bad happened.");
                logger.Error("StartJob()::Exception Message: " + ex.Message);
                logger.Error("StartJob()::Inner Exception: " + ex.InnerException);
                IsItGood = "No";
            }

            finally
            {
                logger.Trace("StartJob()::Start of finally.");
                if (conn != null)
                {
                    conn.Close();
                }
                if (rdr != null)
                {
                    rdr.Close();
                }
                
            }

            logger.Trace("StartJob()::Outside of try/catch/finally.");
            logger.Trace("StartJob()::IsItGood=" + IsItGood.ToString());

            if (IsItGood == "Yes")
            {
                return true;
            }
            else
            { return false; }
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}
        public JobDetails GetJobList()
        {

            logger.Info("GetJobList()::GetJobList was called");

            JobDetails ObjJobDetail = new JobDetails();
            SqlConnection conn = new SqlConnection(con);
            //SqlDataAdapter da = new SqlDataAdapter();
            conn.Open();
            SqlCommand cmd = new SqlCommand("db_app_support.dbo.usp_WS_get_job_list", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            //da.SelectCommand = cmd;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataSet ds = new DataSet();
            DataTable dt = new DataTable("TableName");

            logger.Trace("GetJobList()::Before GetJobList try");
            try
            {
                logger.Trace("GetJobList::Before da.Fill()");
                da.Fill(dt);
                ObjJobDetail.JobRecord = dt;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                // log something here once logging is added
                logger.Error(ex, "GetJobList()::Something bad happened.");
                logger.Error("GetJobList()::Exception Message: " + ex.Message);
                logger.Error("GetJobList()::Inner Exception: " + ex.InnerException);

            }

            logger.Trace("GetJobList()::After try after catch.");

            return ObjJobDetail;
        }
    }


}

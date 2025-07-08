using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;

//user added
using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;
using NLog;
using NLog.Targets;

namespace SQLJobManagerWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "http://dbenedict.contoso.webservice")]
    public interface ifSQLMgr
    {

        [OperationContract]
        Boolean StartJob(string JobName, string User);

        //[OperationContract]
        //CompositeType GetDataUsingDataContract(CompositeType composite);


        [OperationContract]
        JobDetails GetJobList();
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class JobDetails
    {
        [DataMember]
        public DataTable JobRecord
        {
            get; // { return stringValue; }
            set; // { stringValue = value; }
        }
    }

    // DMB - Added to help with installation
    public class SQLWindowsService : ServiceBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ServiceHost serviceHost = null;
        public SQLWindowsService()
        {
            // Name the Windows Service
            ServiceName = "SQLJobManagerWS";
        }

        public static void Main()
        {
            ServiceBase.Run(new SQLWindowsService());
        }
		
        protected override void OnStart(string[] args)
        {

            logger.Debug("Starting The Service");
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.
            serviceHost = new ServiceHost(typeof(SQLJobMgrWS));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();

        }

        protected override void OnStop()
        {
            logger.Debug("Stopping The Service");
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
            NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
        }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "SQLJobManagerWS";
            service.Description = "This webservice has been written by David Benedict to execute SQL Jobs when called from Service Now.";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}

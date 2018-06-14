using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using SISPOSProxy.Core;

namespace SISPOSProxy.Service
{
    /// <summary>
    /// The run service (proxy host app).
    /// </summary>
    public partial class MainService : ServiceBase
    {
        private readonly Proxy _proxy;

        public MainService()
        {
            InitializeComponent();

            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = true;

            InitEventLog();

            _proxy = new Proxy();
        }

        private void InitEventLog()
        {
            EventLog.Source = ServiceName;
            EventLog.Log = "Application";

            ((ISupportInitialize)EventLog).BeginInit();
            if (!EventLog.SourceExists(EventLog.Source))
            {
                EventLog.CreateEventSource(EventLog.Source, EventLog.Log);
            }
            ((ISupportInitialize)EventLog).EndInit();
        }

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            _proxy.Start();

            AddLogInfo("Service was started succesfully");
        }

        protected override void OnStop()
        {
            _proxy.Stop();
            _proxy.Dispose();

            AddLogInfo("Service was stopped");
        }

        protected void AddLogInfo(string msg)
        {
            EventLog.WriteEntry(msg, EventLogEntryType.Information);
        }

        protected void AddLogError(string msg)
        {
            EventLog.WriteEntry(msg, EventLogEntryType.Error);
        }
    }
}

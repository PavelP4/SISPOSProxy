using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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

            _proxy = new Proxy();
        }

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            _proxy.Start();
        }

        protected override void OnStop()
        {
            _proxy.Stop();
            _proxy.Dispose();
        }

        //protected void AddLog(string log)
        //{
        //    try
        //    {
        //        if (!EventLog.SourceExists("SISPOSProxyService"))
        //        {
        //            EventLog.CreateEventSource("SISPOSProxyService", "SISPOSProxyService");
        //        }
        //        EventLog.Source = "SISPOSProxyService";
        //        EventLog.WriteEntry(log);
        //    }
        //    catch { }
        //}
    }
}

using System;
using System.ServiceProcess;
using log4net;
using SISPOSProxy.Core;

namespace SISPOSProxy.Service
{
    /// <summary>
    /// The run service (proxy host app).
    /// </summary>
    public partial class MainService : ServiceBase
    {
        private Proxy _proxy;

        private readonly ILog _logger;

        public MainService()
        {
            InitializeComponent();

            CanStop = true;
            CanPauseAndContinue = false;
            AutoLog = false;
           
            _logger = LogManager.GetLogger(GetType());
        }



        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                _proxy = new Proxy();
                _proxy.Start();
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }

            LogInfo("Service started successfully");
        }

        protected override void OnStop()
        {
            try
            {
                _proxy.Stop();
                _proxy.Dispose();

                LogInfo("Service stopped successfully");
            }
            catch (Exception e)
            {
                LogError(e);
            }            
        }

        protected void LogInfo(string msg)
        {
            _logger.Info(msg);
        }

        protected void LogError(string msg)
        {
            _logger.Error(msg);
        }

        protected void LogError(Exception ex)
        {
            _logger.Error(ex);
        }
    }
}

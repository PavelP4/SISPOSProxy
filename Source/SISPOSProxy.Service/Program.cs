using System.ServiceProcess;

namespace SISPOSProxy.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //new MainService().Start();
            var servicesToRun = new ServiceBase[]
            {
                new MainService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}

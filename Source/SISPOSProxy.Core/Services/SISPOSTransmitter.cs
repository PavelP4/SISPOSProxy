using SISPOSProxy.Core.Caches;
using SISPOSProxy.Core.Config;

namespace SISPOSProxy.Core.Services
{
    public class SISPOSTransmitter : BaseProcessService
    {
        private readonly MessageCache _outputCache;

        public SISPOSTransmitter(Settings settings, MessageCache outputCache) 
            : base(settings)
        {
            _outputCache = outputCache;
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }
}

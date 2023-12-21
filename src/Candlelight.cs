using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Candlelight
{
     public class Core : ModSystem
    { 
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            api.RegisterBlockClass("BlockCandelabra", typeof(BlockCandelabra));
        }
    }
}

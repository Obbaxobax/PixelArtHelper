using ClientSideTest.HologramUI;
using Terraria.ModLoader.Config;

namespace ClientSideTest
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public bool hologramMode;

        public override void OnChanged()
        {
            Hologram.hologramMode = hologramMode;
        }
    }
}

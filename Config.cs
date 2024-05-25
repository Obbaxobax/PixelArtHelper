using Terraria.ModLoader.Config;

namespace ClientSideTest
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public string folderLocation;

        public bool hologramMode;

        public override void OnChanged()
        {
            MenuBar.folderLocation = folderLocation;
            Hologram.hologramMode = hologramMode;
        }
    }
}

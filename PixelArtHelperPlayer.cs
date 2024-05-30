using Terraria.ModLoader;

namespace ClientSideTest
{
    public class PixelArtHelperPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            base.PostUpdate();

            //check for keybind presses
            if (ModContent.GetInstance<PixelArtHelper>().toggleImageMenu.JustPressed)
            {
                ModContent.GetInstance<PixelArtHelper>().ToggleImageMenu();
            }
        }
    }
}

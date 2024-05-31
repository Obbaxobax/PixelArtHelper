using ClientSideTest.HologramUI;
using Microsoft.Xna.Framework;
using Terraria;
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

            //Update the position and check for placement of hologram
            if (PixelArtHelper.hologramUIState.imageReady == true && Main.mouseLeft && !PixelArtHelper.imageMenu.ContainsPoint(Main.MouseScreen))
            {
                Vector2 pos = Main.MouseWorld;

                //Round coordinates to nearest multiple of 16 (because tiles are 16x16)
                float dif = pos.X % 16;
                pos.X = pos.X - dif;

                dif = pos.Y % 16;
                pos.Y = pos.Y - dif;

                ModContent.GetInstance<PixelArtHelper>().openPos = pos;

                //Show the hologram and change the menu to the required blocks page
                ModContent.GetInstance<PixelArtHelper>().ShowUi();
                Main.NewText("Bex");

                PixelArtHelper.hologramUIState.imageReady = false;
                PixelArtHelper.hologramUIState.processing = false;
            }
        }      
    }
}

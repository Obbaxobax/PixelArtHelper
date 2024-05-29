using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ClientSideTest.UIAssets.HologramUI
{
    public class HologramOutline : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(PixelArtHelper.hologramUIState.imageReady)
            {
                Rectangle rect = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, (int)PixelArtHelper.hologramUIState.currentDimensions.X * 16, (int)PixelArtHelper.hologramUIState.currentDimensions.Y * 16);

                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, rect, Color.Lerp(Color.Black, Color.Transparent, 0.5f));
            }

            base.Draw(spriteBatch);
        }
    }
}

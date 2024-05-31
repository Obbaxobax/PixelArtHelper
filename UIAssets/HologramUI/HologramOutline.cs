using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ClientSideTest.UIAssets.HologramUI
{
    //The outline displayed to help line up the hologram
    public class HologramOutline : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            //This is necessary to ensure the hologram lines up at all sizes
            float scale = 1 / Main.UIScale;

            //Only draw if actively placing the hologram
            if (PixelArtHelper.hologramUIState.imageReady)
            {
                Vector2 pos = Main.MouseWorld;

                //Round coordinates to nearest multiple of 16 (because tiles are 16x16)
                float dif = pos.X % 16;
                pos.X = pos.X - dif;

                dif = pos.Y % 16;
                pos.Y = pos.Y - dif;

                pos = pos.ToScreenPosition();

                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)(PixelArtHelper.hologramUIState.currentDimensions.X * 16 * scale), (int)(PixelArtHelper.hologramUIState.currentDimensions.Y * 16 * scale));
                Rectangle inner = rect;
                
                inner.Inflate(-2, -2);
                inner.Offset(2, 2);

                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, inner, Color.Lerp(Color.Black, Color.Transparent, 0.5f));

                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), Color.Lerp(Color.White, Color.Transparent, 0.5f));
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y + 2, 2, rect.Height - 4), Color.Lerp(Color.White, Color.Transparent, 0.5f));
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X + rect.Width - 2, rect.Y + 2, 2, rect.Height - 4), Color.Lerp(Color.White, Color.Transparent, 0.5f));
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), Color.Lerp(Color.White, Color.Transparent, 0.5f));
            }

            base.Draw(spriteBatch);
        }
    }
}

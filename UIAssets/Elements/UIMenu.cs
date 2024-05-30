using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ClientSideTest.UIAssets
{
    //Base class for the menus (just a box)
    public class UIMenu : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate dimensions, turn them to a rectangle, and draw the input box
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();

            UITools.DrawBoxWithTitleBar(spriteBatch, ModContent.Request<Texture2D>("ClientSideTest/Assets/Box").Value, rect, Color.Lerp(Color.BlueViolet, Color.Black, 0.4f), "PixelArtHelper");

            base.Draw(spriteBatch);
        }
    }
}

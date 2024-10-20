using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace ClientSideTest.UIAssets
{
    //Base button element
    public class Button : UIElement
    {
        public string hoverText = ""; //Text to display on button hover
        public string texture = "ClientSideTest/Assets/addButton"; //Texture to display within the button
        public Color boxColor = Color.BlueViolet; //Color of the button
        public bool useTexture = true;

        private int hoverTextColor;

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate the ensure the dimensions are updated
            Recalculate();

            //Get the dimensions, draw the box, then the button texture
            Rectangle rect = GetDimensions().ToRectangle();

            //Draw box cool-ly
            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, boxColor);

            if (useTexture)
            {
                //Draw box texture
                spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>(texture), new Rectangle(rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10), Color.White);
            }

            //Display hover text
            if (IsMouseHovering)
            {
                hoverTextColor = PixelArtHelper.hoverTextColor;
                Main.instance.MouseText(hoverText, rare:hoverTextColor);
            }

            base.Draw(spriteBatch);
        }
    }
}

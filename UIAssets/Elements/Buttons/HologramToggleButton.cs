using ClientSideTest.HologramUI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Button to toggle between normal and alternate mode
    public class HologramToggleButton : Button
    {
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            //Changes bool to opposite value and toggles the mode
            Hologram.hologramMode = !Hologram.hologramMode;

            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate the ensure the dimensions are updated
            Recalculate();

            //Get the dimensions, draw the box, then the button texture
            Rectangle rect = GetDimensions().ToRectangle();

            //Draw box...
            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, boxColor);

            //Display the texture if the mode is on. (In this case it is a checkmark)
            if (Hologram.hologramMode)
            {
                spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>(texture), new Rectangle(rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10), Color.White);
            }

            //Display hover text
            if (IsMouseHovering)
            {
                Main.instance.MouseText(hoverText);
            }
        }
    }
}

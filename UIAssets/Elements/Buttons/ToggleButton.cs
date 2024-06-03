
using ClientSideTest.HologramUI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.UI;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    public class ToggleButton : Button
    {
        public bool state = false;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            state = !state;

            base.LeftMouseDown(evt);
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
            if (state)
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

    public class ListToggleButton : ToggleButton
    {
        private List parent; //Parent list
        private int i; //Index of the button
        private ListElement le; //Associated list element

        public ListToggleButton(List parent, int i, ListElement le)
        {
            this.parent = parent;
            this.i = i;
            this.le = le;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Check if our parent and index are set
            if (parent != null && i != -1)
            {
                //Set our height based on scroll position
                Top.Set(i * (le.parent.elementHeight + 5) + parent.scrollPos, 0);
            }

            base.Draw(spriteBatch);
        }
    }
}

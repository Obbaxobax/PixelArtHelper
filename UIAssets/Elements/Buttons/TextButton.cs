using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Button but with text instead of an image
    public class TextButton : UIElement
    {
        public string hoverText = ""; //Text to display while hovering
        public string displayText = ""; //Text to display on the button
        public Color boxColor = Color.Lerp(Color.BlueViolet, Color.White, 0.1f); //Color of button background

        public override void OnInitialize()
        {
            OverflowHidden = true;

            base.OnInitialize();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate the ensure the dimensions are updated
            Recalculate();

            //Get the dimensions, draw the box, then the button texture
            Rectangle rect = GetDimensions().ToRectangle();

            //Draw box
            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, boxColor);

            //Calculate the position which the text needs to be placed
            Vector2 pos = GetDimensions().Position() + new Vector2(rect.Width / 2, rect.Height / 2) + Vector2.UnitY * 6;

            Utils.DrawBorderString(spriteBatch, displayText, pos, Color.LightPink, scale: 1.5f, anchorx: 0.5f, anchory: 0.5f);

            //Display hover text
            if (IsMouseHovering)
            {
                Main.instance.MouseText(hoverText);
            }

            base.Draw(spriteBatch);
        }
    }
}

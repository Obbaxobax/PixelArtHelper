using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace ClientSideTest.UIAssets.Elements
{
    public class ProgressBar : UIElement
    {
        public float percentage = 0f;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = GetDimensions().ToRectangle();

            UITools.DrawProgressBar(spriteBatch, ModContent.Request<Texture2D>("ClientSideTest/Assets/Box").Value, rect, Color.BlueViolet, percentage);
        }
    }
}

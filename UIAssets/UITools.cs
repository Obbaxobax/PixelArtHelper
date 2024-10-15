using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace ClientSideTest.UIAssets
{
    public static class UITools
    {
        //I stole this from UltraSonic :)
        public static void DrawBoxWith(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color)
        {
            //Generate rectangles to cut the box texture up into pieces
            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            //Create a rectangle for the inner portion of the box
            Rectangle inner = target;
            inner.Inflate(-6, -6);

            //Draw center of the box
            spriteBatch.Draw(tex, inner, sourceCenter, color);

            //Draw the top of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);

            //Draw the right of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the bottom of the box
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);

            //Draw the left of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the corners of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }

        public static void DrawProgressBar(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color, float percentage)
        {
            //Generate rectangles to cut the box texture up into pieces
            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            //Create a rectangle for the inner portion of the box
            Rectangle inner = target;
            inner.Inflate(-6, -6);
            inner.Width = (int)(inner.Width * percentage);

            //Draw center of the box
            spriteBatch.Draw(tex, inner, sourceCenter, color);

            //Draw the top of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);

            //Draw the right of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the bottom of the box
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);

            //Draw the left of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the corners of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);

            Vector2 pos = target.Location.ToVector2();
            pos.X += target.Width / 2;
            pos.Y += target.Height / 2;
            string percentageString = (percentage * 100).ToString("0.00") + "%";
            Utils.DrawBorderStringBig(spriteBatch, percentageString, pos, Color.LightPink, scale: 0.5f, anchorx: 0.5f, anchory: 0.4f);
        }

        public static void DrawBoxWithTitleBar(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color, string title)
        {
            //Generate squares to cut the texture up
            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            //Create a rectangle for the inner portion of the box, this time shrinking it and moving it down
            Rectangle inner = target;
            inner.Inflate(-6, -18);
            inner.Offset(0, 12);

            //Create a smaller rectangle for the title bar
            Rectangle top = target;
            top.Inflate(-6, -(target.Height/2 - 18));
            top.Offset(0, -(target.Height/2) + 18);

            //Draw the main center box
            spriteBatch.Draw(tex, inner, sourceCenter, color);

            //Draw the title bar
            spriteBatch.Draw(tex, top, sourceCenter, Color.Lerp(color, Color.Black, 0.2f));

            //Draw the top of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);

            //Draw the right of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the bottom of the box
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);

            //Draw the bar dividing the titlebar from the main rectangle
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + 36, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);

            //Draw the left of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the corners
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);

            //Calculate the position for the title text
            Vector2 pos = target.Location.ToVector2() + Vector2.One * 6;

            //Get color for the title text
            Color textColor = Color.LightPink;
            textColor = Color.Lerp(textColor, color, 0.6f);

            //Draw the title text
            Utils.DrawBorderStringBig(spriteBatch, title, pos, textColor, scale: 0.5f);
        }

        public static void DrawBoxThinBorder(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color)
        {
            //Generate squares to cut the texture up
            var sourceCorner = new Rectangle(2, 2, 4, 4);
            var sourceEdge = new Rectangle(6, 2, 4, 4);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            //Generate new color
            color = Color.Lerp(color, Color.Black, 0.4f);

            //Create a rectangle for the inner portion of the box
            Rectangle inner = target;
            inner.Inflate(-6, -6);

            //Draw center of the box
            spriteBatch.Draw(tex, inner, sourceCenter, color);

            //Draw the top of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);

            //Draw the right of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the bottom of the box
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);

            //Draw the left of the box
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            //Draw the corners of the box
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }
    }
}

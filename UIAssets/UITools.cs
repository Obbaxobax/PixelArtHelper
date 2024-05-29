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
            //Generate squares to cut the texture up
            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            Rectangle inner = target;
            inner.Inflate(-6, -6);

            //Draw the boxes
            spriteBatch.Draw(tex, inner, sourceCenter, color);

            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }

        public static void DrawBoxWithTitleBar(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color, string title)
        {
            //Generate squares to cut the texture up
            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            Rectangle inner = target;
            inner.Inflate(-6, -18);
            inner.Offset(0, 12);

            Rectangle top = target;
            //Main.NewText(-(target.Height - 18));
            top.Inflate(-6, -(target.Height/2 - 18));
            top.Offset(0, -(target.Height/2) + 18);

            //Draw the boxes
            spriteBatch.Draw(tex, inner, sourceCenter, color);
            spriteBatch.Draw(tex, top, sourceCenter, Color.Lerp(color, Color.Black, 0.2f));

            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + 36, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);

            Vector2 pos = target.Location.ToVector2() + Vector2.One * 6;
            Color textColor = Color.LightPink;
            textColor = Color.Lerp(textColor, color, 0.6f);

            Utils.DrawBorderStringBig(spriteBatch, title, pos, textColor, scale: 0.5f);
        }

        public static void DrawBoxThinBorder(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color)
        {
            //Generate squares to cut the texture up
            var sourceCorner = new Rectangle(2, 2, 4, 4);
            var sourceEdge = new Rectangle(6, 2, 4, 4);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            color = Color.Lerp(color, Color.Black, 0.4f);

            Rectangle inner = target;
            inner.Inflate(-6, -6);

            //Draw the boxes
            spriteBatch.Draw(tex, inner, sourceCenter, color);

            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }
    }
}

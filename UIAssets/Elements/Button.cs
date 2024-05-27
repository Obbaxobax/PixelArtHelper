using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using System.IO;
using Microsoft.Xna.Framework;

namespace ClientSideTest.UIAssets
{
    public class Button : UIElement
    {

        public string hoverText = "";
        public string texture = "ClientSideTest/Assets/addButton";
        public Color boxColor = Color.CornflowerBlue;


        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate the ensure the dimensions are updated
            Recalculate();

            //Get the dimensions, draw the box, then the button texture
            Rectangle rect = GetDimensions().ToRectangle();

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, boxColor);

            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>(texture), new Rectangle(rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10), Color.White);

            if (IsMouseHovering)
            {
                Main.instance.MouseText(hoverText);
            }

            base.Draw(spriteBatch);
        }
    }
    public class deleteButton : Button
    {
        private List parent;
        private int i;
        private ListElement le;

        public deleteButton(List parent, int i, ListElement le)
        {
            this.parent = parent;
            this.i = i;
            this.le = le;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (parent != null && i != -1)
            {
                Top.Set(i * 35f + parent.scrollPos, 0);
            }

            base.Draw(spriteBatch);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            deleteItem();

            base.LeftClick(evt);
        }

        public void deleteItem()
        {

            try
            {
                //Main.NewText(Menu.images.Keys.ToArray()[3]);
                foreach (string file in Directory.EnumerateFiles(MainMenu.savePath, "*.png"))
                {
                    if (Path.GetFileNameWithoutExtension(file) == MainMenu.images.Keys.ToArray()[i])
                    {
                        File.Delete(file);
                    }
                }

                MainMenu.images.Remove(MainMenu.images.Keys.ToArray()[i]);

                RemoveChild(le);
            }
            catch (Exception ex)
            {
                Main.NewText($"{ex.Message} : {i}", Color.PaleVioletRed);
            }
        }
    }
}

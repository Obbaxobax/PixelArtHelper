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
using Tile = ClientSideTest.HologramUI.Tile;
using ClientSideTest.HologramUI;

namespace ClientSideTest.UIAssets
{
    public class Button : UIElement
    {

        public string hoverText = "";
        public string texture = "ClientSideTest/Assets/addButton";
        public Color boxColor = Color.BlueViolet;


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

    public class TextButton : UIElement
    {
        public string hoverText = "";
        public string displayText = "";
        public Color boxColor = Color.Lerp(Color.BlueViolet, Color.White, 0.1f);

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

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, boxColor);

            Vector2 pos = GetDimensions().Position() + new Vector2(rect.Width / 2, rect.Height / 2) + Vector2.UnitY * 6;

            Utils.DrawBorderString(spriteBatch, displayText, pos, Color.LightPink, scale: 1.5f, anchorx: 0.5f, anchory: 0.5f);

            if (IsMouseHovering)
            {
                Main.instance.MouseText(hoverText);
            }

            base.Draw(spriteBatch);
        }
    }

    public class ListElementButton : Button
    {
        private List parent;
        private int i;
        private ListElement le;

        public ListElementButton(List parent, int i, ListElement le)
        {
            this.parent = parent;
            this.i = i;
            this.le = le;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (parent != null && i != -1)
            {
                Top.Set(i * (le.parent.elementHeight + 5) + parent.scrollPos, 0);
            }

            base.Draw(spriteBatch);
        }
    }

    public class ExceptionsListButton : ListElementButton
    {
        private int i;
        private List<Tile> elements;
        private Exceptions exList;

        public ExceptionsListButton(List parent, int i, ListElement le, List<Tile> elements, Exceptions exList) : base(parent, i, le)
        {
            this.i = i;
            this.elements = elements;
            this.exList = exList;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            exList.exceptionsDict[elements[i].Name] = !exList.exceptionsDict[elements[i].Name];
            texture = exList.exceptionsDict[elements[i].Name] ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";

            base.LeftClick(evt);
        }
    }

    public class PaintToggleButton : Button
    {
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            PixelArtHelper.hologramUIState.usePaints = !PixelArtHelper.hologramUIState.usePaints;
            texture = PixelArtHelper.hologramUIState.usePaints ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";

            base.LeftClick(evt);
        }
    }
    public class HologramToggleButton : Button
    {
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            Hologram.hologramMode = !Hologram.hologramMode;

            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate the ensure the dimensions are updated
            Recalculate();

            //Get the dimensions, draw the box, then the button texture
            Rectangle rect = GetDimensions().ToRectangle();

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, boxColor);

            if (Hologram.hologramMode)
            {
                spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>(texture), new Rectangle(rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10), Color.White);
            }

            if (IsMouseHovering)
            {
                Main.instance.MouseText(hoverText);
            }
        }
    }

    public class DeleteButton : ListElementButton
    {
        private List parent;
        private int i;
        private ListElement le;

        public DeleteButton(List parent, int i, ListElement le) : base(parent, i, le)
        {
            this.parent = parent;
            this.i = i;
            this.le = le;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
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
            catch
            {
                Main.NewText("There was an issue removing the image. Everything should still be fine, but still create an issue on the github.", Color.PaleVioletRed);
            }
        }
    }
}

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Tile = ClientSideTest.HologramUI.Tile;

namespace ClientSideTest.UIAssets
{
    public class ImageList : List
    {
        public string[] names;

        public override void OnInitialize()
        {
            for (int i = 0; i < names.Length; i++)
            {
                //Generate an element for each image, give it an index and it's name
                ImageListElement le = new ImageListElement(i, names[i], this);
                le.Height.Set(30f, 0);
                le.Width.Set(315f, 0);
                le.Top.Set(i * 35f + scrollPos, 0);


                Button butt = new deleteButton(this, i, le);
                butt.Width.Set(30f, 0);
                butt.Height.Set(30f, 0);
                butt.Left.Set(315f, 0);
                butt.Top.Set(i * 35f + scrollPos, 0);
                butt.hoverText = "Delete";
                butt.texture = "ClientSideTest/Assets/deleteButton";
                butt.boxColor = Color.CornflowerBlue.MultiplyRGB(Color.Red);

                Append(le);
                Append(butt);
            }

            OverflowHidden = true;
            base.OnInitialize();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Update the ui if there is a new image
            if (names.Length != Children.Count())
            {
                RemoveAllChildren();
                OnInitialize();
            }

            base.Draw(spriteBatch);
        }
    }

    public class List : UIElement
    {
        public int scrollPos = 0;

        public override void OnInitialize()
        {   
            OverflowHidden = true;
            base.OnInitialize();
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            scrollPos += (int)Math.Floor((double)(evt.ScrollWheelValue / 120)) * 10;
            float min = Children.Count() * 35 - Height.Pixels > 0 ? Children.Count() * -35 + Height.Pixels : 0;
            scrollPos = (int)Math.Clamp(scrollPos, min, 0);
            //Main.NewText(scrollPos);

            base.ScrollWheel(evt);
        }
    }

    public class ExceptionsList : List
    {
        public List<Tile> elements;

        public override void OnInitialize()
        {
            int i = 0;
            foreach(Tile tile in elements)
            {
                i++;


            }

            base.OnInitialize();
        }
    }

    public class ExceptionsListElement : ListElement
    {
        private int i;
        private string text;

        public ExceptionsListElement(int i, string text, List parent) : base(i, text, parent)
        {
            this.i = i;
            this.text = text;
        }
    }

    public class ImageListElement : ListElement
    {
        private int i;
        private string text;

        public ImageListElement(int i, string text, List parent) : base(i, text, parent)
        {
            this.i = i;
            this.text = text;
        }

        async public override void LeftClick(UIMouseEvent evt)
        {
            //Generate the pixels for the hologram
            await Task.Run(() => PixelArtHelper.hologramUIState.createPixels(MainMenu.images[text]));

            base.LeftClick(evt);
        }
    }

    public class ListElement : UIElement
    {
        private int i;
        private string text;
        private List parent;

        public ListElement(int i, string text, List parent)
        {
            this.i = i;
            this.text = text;
            this.parent = parent;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Top.Set(i * 35f + parent.scrollPos, 0);

            //Gets the size and position of the ui element
            Rectangle rect = GetDimensions().ToRectangle();

            //Create the position for the text, offsetting it slightly to line up with the boxes
            Vector2 pos = new Vector2(rect.X, rect.Y) + Vector2.One * 6;

            //First draw the box, then the text.
            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.CornflowerBlue);
            Utils.DrawBorderString(spriteBatch, text, pos, Color.White, 1f);

            base.Draw(spriteBatch);
        }
    }
}

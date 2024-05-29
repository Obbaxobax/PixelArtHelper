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
using Terraria.ID;
using Terraria.Enums;

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


                Button butt = new DeleteButton(this, i, le);
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
            if (names.Length != Children.Count()/2)
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
        public int scrollSpeed = 10;
        public float elementHeight = 30;
        public float elementPerRow = 1;
        public Color color = Color.Lerp(Color.BlueViolet, Color.Black, 0.5f);

        public override void OnInitialize()
        {   
            OverflowHidden = true;
            base.OnInitialize();
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            scrollPos += (int)Math.Floor((double)(evt.ScrollWheelValue / 120)) * scrollSpeed;

            int numberOfRows = (int)Math.Floor(Children.Count() / elementPerRow);

            float min = numberOfRows * (elementHeight + 5) - Height.Pixels > 0 ? numberOfRows * -(elementHeight + 5) + Height.Pixels : 0;
            scrollPos = (int)Math.Clamp(scrollPos, min, 0);

            base.ScrollWheel(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();
            rect = new Rectangle(rect.X - 5, rect.Y - 5, rect.Width + 10, rect.Height + 10);

            UITools.DrawBoxThinBorder(spriteBatch, ModContent.Request<Texture2D>("ClientSideTest/Assets/Box").Value, rect, color);

            base.Draw(spriteBatch);
        }
    }

    public class ExceptionsList : List
    {
        public List<Tile> elements;
        private Exceptions exList;

        public ExceptionsList(Exceptions exList)
        {
            this.exList = exList;  
        }

        public override void OnInitialize()
        {
            elementHeight = 50f;
            scrollSpeed = 50;

            for (int i = 0; i < elements.Count; i++)
            {
                ExceptionsListElement ele = new ExceptionsListElement(i, elements[i].Name, this);
                ele.Width.Set(295f, 0);
                ele.Height.Set(50f, 0);
                ele.stringOffset = 8f;
                Append(ele);

                ExceptionsListButton butt = new ExceptionsListButton(this, i, ele, elements, exList);
                butt.Width.Set(50f, 0);
                butt.Height.Set(50f, 0);
                butt.Left.Set(295f, 0);
                butt.texture = exList.exceptionsDict[elements[i].Name] ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";

                Append(butt);
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
        public readonly List parent;

        private float elementHeight;

        public float stringOffset = 6f;

        public ListElement(int i, string text, List parent)
        {
            this.i = i;
            this.text = text;
            this.parent = parent;

            elementHeight = parent.elementHeight;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Top.Set(i * (elementHeight + 5) + parent.scrollPos, 0);

            //Gets the size and position of the ui element
            Rectangle rect = GetDimensions().ToRectangle();

            //Create the position for the text, offsetting it slightly to line up with the boxes
            Vector2 pos = new Vector2(rect.X, rect.Y) + Vector2.One * stringOffset;

            //First draw the box, then the text.
            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.BlueViolet);
            Utils.DrawBorderString(spriteBatch, text, pos, Color.LightPink, 1f);

            base.Draw(spriteBatch);
        }
    }

    public class RequiredList : List
    {
        public Dictionary<string, int> list = new Dictionary<string, int>();

        public override void OnInitialize()
        {
            list = list.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
            for (int i = 0; i < list.Count; i++)
            {
                string displayString = $"{list.ElementAt(i).Key}: {list.ElementAt(i).Value}";
                ListElement le = new ListElement(i, displayString, this);
                le.Height.Set(30f, 0);
                le.Width.Set(345f, 0);

                Append(le);
            }

            base.OnInitialize();
        }
    }
}

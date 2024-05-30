﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace ClientSideTest.UIAssets
{
    public class List : UIElement
    {
        public int scrollPos = 0; //The scroll position
        public int scrollSpeed = 10; //The scroll speed
        public float elementHeight = 30; //The height of each element in the list (used for calculating scroll bounds)
        public float elementPerRow = 1; //The number of UI elements used for one row
        public Color color = Color.Lerp(Color.BlueViolet, Color.Black, 0.5f); //The color of the background box

        public override void OnInitialize()
        {   
            OverflowHidden = true;
            base.OnInitialize();
        }

        //Fires when scrolling
        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            //Downwards scroll is negative multiple of 12. Below makes it a more managable number
            scrollPos += (int)Math.Floor((double)(evt.ScrollWheelValue / 120)) * scrollSpeed;

            //Calculate how many rows there are for the sake of clamping
            int numberOfRows = (int)Math.Floor(Children.Count() / elementPerRow);

            //Calculate the minimum y-value that can be scrolled to (number of elements * the height they take up)
            float min = numberOfRows * (elementHeight + 5) - Height.Pixels > 0 ? numberOfRows * -(elementHeight + 5) + Height.Pixels : 0;
            scrollPos = (int)Math.Clamp(scrollPos, min, 0); //Clamp the scroll to be within bounds

            base.ScrollWheel(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();

            //Get the dimensions of the list and expand it by 5 in all directions
            Rectangle rect = GetDimensions().ToRectangle();
            rect = new Rectangle(rect.X - 5, rect.Y - 5, rect.Width + 10, rect.Height + 10);

            //Draw a box :)
            UITools.DrawBoxThinBorder(spriteBatch, ModContent.Request<Texture2D>("ClientSideTest/Assets/Box").Value, rect, color);

            base.Draw(spriteBatch);
        }
    }

    //The base element to be used with a list
    public class ListElement : UIElement
    {
        private int i; //Index of the element
        private string text; //Text to display for the element
        public readonly List parent; //The list that this element is attached to

        private float elementHeight; //The height of the element

        public float stringOffset = 6f; //The offset of the string displayed

        public ListElement(int i, string text, List parent)
        {
            this.i = i;
            this.text = text;
            this.parent = parent;

            elementHeight = parent.elementHeight;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Each element sets their own position vertically by using the scrollPos from the parent.
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
}
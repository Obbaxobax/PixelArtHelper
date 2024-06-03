using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using ClientSideTest.UIAssets.Elements.Buttons;

namespace ClientSideTest.UIAssets.Elements.Lists
{
    //Class for the required items list
    public class RequiredList : List
    {
        public requiredItemsElement list; //An empty dictionary to store the blocks that will be required
        public bool mode = false;
        public bool paints;

        public override void OnInitialize()
        {
            elementHeight = 30f;
            elementPerRow = 2;

            //order the dictionary in descending order by amount required
            var sortedList = list.requiredListElements.OrderByDescending(pair => pair.Value[0]).ToDictionary(pair => pair.Key, pair => pair.Value);

            //create a list element for each required block
            for (int i = 0; i < sortedList.Count; i++)
            {
                if(paints) 
                {
                    //Generate a display name which shows "*name*: *amount*"
                    string displayString = $"{sortedList.ElementAt(i).Key}: {sortedList.ElementAt(i).Value[0]}";
                    ListElement le = new ListElement(i, displayString, this, 45);
                    le.Height.Set(30f, 0);

                    ListToggleButton toggleButton = new ListToggleButton(this, i, le);
                    toggleButton.Width.Set(30f, 0);
                    toggleButton.Height.Set(30f, 0);
                    toggleButton.Left.Set(Width.Pixels - 45, 0);
                    toggleButton.hoverText = "Use this to help keep track of which items you have";
                    toggleButton.texture = "ClientSideTest/Assets/activeButton";
                    toggleButton.boxColor = Color.BlueViolet;

                    Append(le);
                    Append(toggleButton);
                }
                else
                {
                    //Generate a display name which shows "*name*: *amount*"
                    string displayString = $"{sortedList.ElementAt(i).Key}: {sortedList.ElementAt(i).Value[0]}";
                    RequiredListElement le = new RequiredListElement(i, displayString, this, 45, sortedList);
                    le.Height.Set(30f, 0);

                    ListToggleButton toggleButton = new ListToggleButton(this, i, le);
                    toggleButton.Width.Set(30f, 0);
                    toggleButton.Height.Set(30f, 0);
                    toggleButton.Left.Set(Width.Pixels - 45, 0);
                    toggleButton.hoverText = "Use this to help keep track of which items you have";
                    toggleButton.texture = "ClientSideTest/Assets/activeButton";
                    toggleButton.boxColor = Color.BlueViolet;

                    Append(le);
                    Append(toggleButton);
                }  
            }

            base.OnInitialize();
        }
    }

    public class RequiredListElement : ListElement
    {
        private float elementHeight; //The height of the element
        private int i;
        private RequiredList list;
        private Dictionary<string, int[]> sortedList;

        private string text;

        public RequiredListElement(int i, string displayString, RequiredList list, int buttonOffset, Dictionary<string, int[]> sortedList) : base(i, displayString, list)
        {
            this.i = i;
            this.list = list;
            this.sortedList = sortedList;
            text = displayString;

            elementHeight = list.elementHeight;
            Width.Set(list.Width.Pixels - buttonOffset, 0f);

            base.OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Each element sets their own position vertically by using the scrollPos from the parent.
            Top.Set(i * (elementHeight + 5) + list.scrollPos, 0);

            //Gets the size and position of the ui element
            Rectangle rect = GetDimensions().ToRectangle();

            //Create the position for the text, offsetting it slightly to line up with the boxes
            Vector2 pos = new Vector2(rect.X, rect.Y) + Vector2.One * stringOffset;

            //First draw the box, then the text.
            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.BlueViolet);
            Utils.DrawBorderString(spriteBatch, text, pos, Color.LightPink, 1f);
        }

        public override void Update(GameTime gameTime)
        {
            if (list.mode)
            {
                for (int v = 0; v < 50; v++)
                {
                    var id = Main.LocalPlayer.inventory[v].createTile;
                    if (id != -1)
                    {
                        if (id == sortedList.ElementAt(i).Value[1])
                        {
                            text = $"{sortedList.ElementAt(i).Key}: {Math.Clamp(sortedList.ElementAt(i).Value[0] - Main.LocalPlayer.inventory[v].stack, 0, sortedList.ElementAt(i).Value[0])}";
                            break;
                        }
                    }

                    id = Main.LocalPlayer.inventory[v].createWall;
                    if (id != -1)
                    {
                        if (id == sortedList.ElementAt(i).Value[1])
                        {
                            text = $"{sortedList.ElementAt(i).Key}: {Math.Clamp(sortedList.ElementAt(i).Value[0] - Main.LocalPlayer.inventory[v].stack, 0, sortedList.ElementAt(i).Value[0])}";
                            break;
                        }
                    }
                }
            }
            else
            {
                text = $"{sortedList.ElementAt(i).Key}: {sortedList.ElementAt(i).Value[0]}";
            }

            base.Update(gameTime);
        }
    }
}

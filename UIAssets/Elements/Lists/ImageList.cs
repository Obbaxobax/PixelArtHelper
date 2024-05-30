﻿using ClientSideTest.UIAssets.Elements.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;

namespace ClientSideTest.UIAssets.Elements.Lists
{
    //List class for displaying the image list
    public class ImageList : List
    {
        public List<string> names; //The names of all the images

        public override void OnInitialize()
        {
            //Create a row for each saved image
            for (int i = 0; i < names.Count; i++)
            {
                //A list element, passing it its name
                ImageListElement le = new ImageListElement(i, names[i], this);
                le.Height.Set(30f, 0);
                le.Width.Set(315f, 0);
                le.Top.Set(i * 35f + scrollPos, 0);

                //Generate a button used to delete the image
                DeleteButton butt = new DeleteButton(this, i, le);
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
            if (names.Count != Children.Count() / 2)
            {
                RemoveAllChildren();
                OnInitialize();
            }

            base.Draw(spriteBatch);
        }
    }

    public class ImageListElement : ListElement
    {
        private string text; //Name of the image

        public ImageListElement(int i, string text, List parent) : base(i, text, parent)
        {
            this.text = text;
        }

        async public override void LeftMouseDown(UIMouseEvent evt)
        {
            //Generate the pixels for the hologram when clicked
            await Task.Run(() => PixelArtHelper.hologramUIState.createPixels(MainMenu.images[text]));
            PixelArtHelper.imageMenu.state = "required";

            base.LeftClick(evt);
        }
    }
}

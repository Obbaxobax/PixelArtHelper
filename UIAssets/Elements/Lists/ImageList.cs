using ClientSideTest.UIAssets.Elements.Buttons;
using ClientSideTest.UIAssets.Menus;
using ClientSideTest.UIAssets.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Color = Microsoft.Xna.Framework.Color;

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
                ImageListElement listElement = new ImageListElement(i, names[i], this, 75);
                listElement.Height.Set(30f, 0);
                listElement.Top.Set(i * 35f + scrollPos, 0);

                PaintToggleButton ptb = new PaintToggleButton(listElement);
                ptb.Width.Set(30f, 0);
                ptb.Height.Set(30f, 0);
                ptb.Left.Set(listElement.Width.Pixels, 0);
                ptb.Top.Set(i * 35f + scrollPos, 0);
                ptb.hoverText = "Use paints?";
                ptb.texture = "ClientSideTest/Assets/deleteButton";

                //Generate a button used to delete the image
                DeleteButton butt = new DeleteButton(this, i, listElement, names[i]);
                butt.Width.Set(30f, 0);
                butt.Height.Set(30f, 0);
                butt.Left.Set(listElement.Width.Pixels + 30, 0);
                butt.Top.Set(i * 35f + scrollPos, 0);
                butt.hoverText = "Delete";
                butt.texture = "ClientSideTest/Assets/deleteButton";
                butt.boxColor = Color.CornflowerBlue.MultiplyRGB(Color.Red);

                Append(listElement);
                Append(ptb);
                Append(butt);
            }

            OverflowHidden = true;
            base.OnInitialize();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Update the ui if there is a new image
            if (names.Count != Children.Count() / 3)
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
        public bool usePaints;

        public ImageListElement(int i, string text, List parent, int buttonOffset) : base(i, text, parent, buttonOffset)
        {
            this.text = text;
            usePaints = false;
        }

        async public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!PixelArtHelper.hologramUIState.processing) {            
                PixelArtHelper.imageMenu.state = "proc";
                PixelArtHelper.imageMenu.procMenu.SetImage(MainMenu.images[text]);

                //Generate the pixels for the hologram when clicked
                await Task.Run(() => PixelArtHelper.hologramUIState.createPixels(MainMenu.images[text], usePaints));
            }
            else
            {
                Main.NewText("Please wait for the current image to finish processing first.", Color.PaleVioletRed);
            }
        }
    }
}

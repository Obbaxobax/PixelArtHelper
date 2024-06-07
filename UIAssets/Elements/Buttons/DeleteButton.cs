using System.IO;
using System.Linq;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using ClientSideTest.UIAssets.Elements.Lists;
using System;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Delete button used in the image list
    public class DeleteButton : ListElementButton
    {
        private int i; //Index of button in the list
        private ImageList parent;
        private string key;

        public DeleteButton(ImageList parent, int i, ListElement le, string key) : base(parent, i, le)
        {
            this.i = i;
            this.key = key;
            this.parent = parent;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            deleteItem();

            base.LeftClick(evt);
        }

        public void deleteItem()
        {
            Main.NewText(key);

            //Attempts to delete the image associated with this button
            try
            {
                //Finds the file and deletes it
                foreach (string file in Directory.EnumerateFiles(MainMenu.savePath, "*.png"))
                {
                    if (Path.GetFileNameWithoutExtension(file) == key)
                    {
                        File.Delete(file);
                        break;
                    }
                }

                //Remove name from image list
                parent.names.Remove(key);

                //Removes the bitmap from the cache
                MainMenu.images.Remove(key);
            }
            catch
            {
                Main.NewText("There was an issue removing the image. Create an issue on the github.", Color.PaleVioletRed);
            }
        }
    }
}

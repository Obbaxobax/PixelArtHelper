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
        private ListElement le; //The associated element
        private ImageList parent;

        public DeleteButton(ImageList parent, int i, ListElement le) : base(parent, i, le)
        {
            this.i = i;
            this.le = le;
            this.parent = parent;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            deleteItem();

            base.LeftClick(evt);
        }

        public void deleteItem()
        {
            //Attempts to delete the image associated with this button
            try
            {
                //Finds the file and deletes it
                foreach (string file in Directory.EnumerateFiles(MainMenu.savePath, "*.png"))
                {
                    if (Path.GetFileNameWithoutExtension(file) == MainMenu.images.Keys.ToArray()[i])
                    {
                        File.Delete(file);
                        break;
                    }
                }

                //Remove name from image list
                parent.names.Remove(MainMenu.images.Keys.ToArray()[i]);

                //Removes the bitmap from the cache
                MainMenu.images.Remove(MainMenu.images.Keys.ToArray()[i]);
            }
            catch
            {
                Main.NewText("There was an issue removing the image. Create an issue on the github.", Color.PaleVioletRed);
            }
        }
    }
}

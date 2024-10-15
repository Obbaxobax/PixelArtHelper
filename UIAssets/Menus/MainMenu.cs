using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using Terraria;
using ReLogic.OS;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using System.Linq;
using ClientSideTest.UIAssets.Elements.Lists;
using ClientSideTest.UIAssets.Elements.Buttons;
using Terraria.Utilities.FileBrowser;
using ClientSideTest.HologramUI;

namespace ClientSideTest.UIAssets.Menus
{
    public class MainMenu : UIMenu
    {
        private HttpClient client;
        public readonly static string savePath = Path.Combine(Main.SavePath, "PixelArtHelperImages/");

        private TextField locationField;
        private TextField sizeX;
        private TextField sizeY;
        private TextField saveName;
        private ImageList il;

        public static Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();

        public void Reinitialize()
        {
            RemoveAllChildren();
            OnInitialize();
        }

        public override void OnInitialize()
        {
            images.Clear();

            //Cache each of the images as a bitmap
            foreach (string file in Directory.EnumerateFiles(savePath, "*.png"))
            {
                Bitmap bm = new Bitmap(Image.FromFile(file));
                images.Add(Path.GetFileNameWithoutExtension(file), bm);

                Main.NewText(Path.GetFileNameWithoutExtension(file));
            }

            client = new HttpClient();

            sizeX = new TextField();
            sizeX.Width.Set(175f, 0);
            sizeX.Height.Set(50f, 0);
            sizeX.Top.Set(41f, 0);
            sizeX.maxChar = 8;

            sizeY = new TextField();
            sizeY.CopyStyle(sizeX);
            sizeY.Left.Set(190f, 0);
            sizeY.placeholderText = "Height";
            sizeY.hoverText = "Defaults to the height of the image (Not required)";

            sizeX.Left.Set(10f, 0);
            sizeX.placeholderText = "Width";
            sizeX.hoverText = "Defaults to the width of the image (Not required)";

            saveName = new TextField();
            saveName.Width.Set(355f, 0);
            saveName.Height.Set(50f, 0);
            saveName.Left.Set(10f, 0);
            saveName.Top.Set(97f, 0);
            saveName.placeholderText = "Name to save as...";
            saveName.hoverText = "Name which this image will be saved to list as. (Required)";

            TextButton exButt = new TextButton();
            exButt.Width.Set(355f, 0);
            exButt.Height.Set(50f, 0);
            exButt.Left.Set(10f, 0);
            exButt.Top.Set(152f, 0);
            exButt.hoverText = "Choose blocks not to include. (Some values are preset)";
            exButt.displayText = "Exceptions";

            exButt.OnLeftMouseDown += (evt, args) =>
            {
                PixelArtHelper.imageMenu.state = "exceptions";
            };

            Button butt = new Button();
            butt.Width.Set(50f, 0);
            butt.Height.Set(50f, 0);
            butt.Left.Set(10f, 0);
            butt.Top.Set(207f, 0);
            butt.hoverText = "Add image from current link or path.\nPastes the current clipboard if field is empty.\nRight click to open to select a local file.";

            butt.OnLeftMouseDown += (evt, args) =>
            {
                if (string.IsNullOrEmpty(locationField.currentValue))
                {
                    locationField.currentValue = Platform.Get<IClipboard>().Value;
                    return;
                }

                addImage();
                locationField.currentValue = "";
            };

            butt.OnRightMouseDown += (evt, args) =>
            { 
                openImageFile(); 
            };

            TextButton cancelButt = new TextButton();
            cancelButt.Width.Set(355f, 0);
            cancelButt.Height.Set(50f, 0);
            cancelButt.Left.Set(10f, 0);
            cancelButt.Top.Set(440f, 0);
            cancelButt.displayText = "Cancel";
            cancelButt.hoverText = "Cancel image processing";

            cancelButt.OnLeftMouseDown += (evt, args) =>
            {
                HologramUIState.cancel = true;
            };

            locationField = new TextField();
            locationField.Width.Set(300f, 0);
            locationField.Height.Set(50f, 0);
            locationField.Top.Set(207f, 0);
            locationField.Left.Set(65f, 0);
            locationField.placeholderText = "Input link or file path";
            locationField.hoverText = "The file path or the link to the image (Required)";

            il = new ImageList();
            il.Width.Set(345f, 0);
            il.Height.Set(165f, 0);
            il.Left.Set(15f, 0);
            il.Top.Set(267f, 0);
            il.names = images.Keys.ToList();
            il.elementPerRow = 2;

            Append(sizeX);
            Append(sizeY);
            Append(saveName);
            Append(exButt);
            Append(butt);
            Append(cancelButt);
            Append(locationField);
            Append(il);
            base.OnInitialize();
        }

        private void openImageFile()
        {
            NativeFileDialog fileDialog = new NativeFileDialog();

            ExtensionFilter[] filter = [new ExtensionFilter("Image Files", ["png", "jpg", "jpeg"])];

            string image = fileDialog.OpenFilePanel("Select file for image", filter);

            if (image == null) return;

            locationField.currentValue = Path.GetFullPath(image);

            return;
        }

        async private void addImage()
        {
            //return if there is no set file location or URL
            if (locationField.currentValue == null)
            {
                Main.NewText("Please supply a url or file path.", Color.PaleVioletRed);
                return;
            }

            //Check if the input is a URL
            if (locationField.currentValue.Contains("https://"))
            {
                try
                {
                    //Get the image from the link
                    Stream fileStream = await client.GetStreamAsync(locationField.currentValue);
                    Image image = Image.FromStream(fileStream);

                    //Get the dimensions to use for the image
                    Vector2 dimensions = getBitmapDimensions(image);

                    Bitmap bm = new Bitmap(image, new Size((int)dimensions.X, (int)dimensions.Y));

                    //Check if a save name is set, otherwise send an error in chat
                    if (saveName != null)
                    {
                        bm.Save(savePath + saveName.currentValue + ".png");
                        images[saveName.currentValue] = bm;
                        il.names = il.names.Append(saveName.currentValue).ToList();
                        return;
                    }
                    else
                    {
                        Main.NewText("Please provide a name to save as.", Color.PaleVioletRed);
                    }
                }
                catch (HttpRequestException ex)
                {
                    //Check if the failure was due to internet or an invalid link
                    if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Main.NewText("To use links, please ensure a proper link or internet connection.", Color.PaleVioletRed);
                    }
                    else
                    {
                        Main.NewText("Something unexpected occured. Please open an issue on the github with your client.log file included.");
                    }
                    return;
                }
                catch
                {
                    Main.NewText("Something unexpected occured, possibly related to the image type. Please open an issue on the github with your client.log file included.");
                    return;
                }
            }
            else
            {
                try
                {
                    //Get image from file path
                    Image image = Image.FromFile(locationField.currentValue.Replace("\"", ""));

                    //This is a patchwork solutions for ensuring the filepath is valid <-----------------------------------------------------
                    if (image == null)
                    {
                        Main.NewText("Invalid image! Please provide a proper file path.", Color.PaleVioletRed);
                    }

                    //Get the dimensions and create the bitmap
                    Vector2 dimensions = getBitmapDimensions(image);

                    Bitmap bm = new Bitmap(image, new Size((int)dimensions.X, (int)dimensions.Y));

                    //Check if a save name is set, otherwise send an error in chat
                    if (saveName != null)
                    {
                        bm.Save(savePath + saveName.currentValue + ".png");
                        images[saveName.currentValue] = bm;
                        il.names = il.names.Reverse<string>().Append(saveName.currentValue).Reverse().ToList();
                        return;
                    }
                    else
                    {
                        Main.NewText("Please provide a name to save as.", Color.PaleVioletRed);
                    }
                }
                catch
                {
                    Main.NewText("Something unexpected occured. Please open an issue on the github with your client.log file included.", Color.PaleVioletRed);
                    return;
                }
            }
        }

        //Gets the dimensions to be used for bitmap image
        //If the dimensions are not set, it will choose the ones of the input image
        private Vector2 getBitmapDimensions(Image image)
        {
            int width = image.Width;
            int height = image.Height;

            if (int.TryParse(sizeX.currentValue, out int widthValue))
            {
                width = widthValue;
            }

            if (int.TryParse(sizeX.currentValue, out int heightValue))
            {
                height = heightValue;
            }

            return new Vector2(width, height);
        }
    }
}

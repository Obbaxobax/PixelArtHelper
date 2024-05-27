using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using Terraria.ModLoader;
using Terraria;
using ReLogic.OS;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.Linq;

namespace ClientSideTest.UIAssets
{
    public class MainMenu : DraggableUIMenu
    {
        private HttpClient client;
        public readonly static string savePath = Path.Combine(Main.SavePath, "PixelArtHelperImages/");

        private TextField locationField;
        private TextField sizeX;
        private TextField sizeY;
        private TextField saveName;

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
            sizeX.Width.Set(170, 0);
            sizeX.Height.Set(50, 0);
            sizeX.Top.Set(15, 0);

            sizeY = new TextField();
            sizeY.CopyStyle(sizeX);
            sizeY.Left.Set(190, 0);
            sizeY.placeholderText = "Height";
            sizeY.hoverText = "Defaults to the height of the image";

            sizeX.Left.Set(15, 0);
            sizeX.placeholderText = "Width";
            sizeX.hoverText = "Defaults to the width of the image";

            saveName = new TextField();
            saveName.Width.Set(345, 0);
            saveName.Height.Set(50, 0);
            saveName.Left.Set(15, 0);
            saveName.Top.Set(70, 0);
            saveName.placeholderText = "Name to save as...";

            Button butt = new Button();
            butt.Width.Set(50, 0);
            butt.Height.Set(50, 0);
            butt.Left.Set(15, 0);
            butt.Top.Set(125, 0);
            butt.hoverText = "Add image from current link or path.";

            butt.OnLeftMouseDown += (evt, args) =>
            {
                if (string.IsNullOrEmpty(locationField.currentValue))
                {
                    locationField.currentValue = Platform.Get<IClipboard>().Value;
                    return;
                }

                addImage();
            };

            locationField = new TextField();
            locationField.Width.Set(285, 0);
            locationField.Height.Set(50, 0);
            locationField.Top.Set(125f, 0);
            locationField.Left.Set(75f, 0);
            locationField.placeholderText = "Input link or file path";

            List il = new List();
            il.Width.Set(345f, 0);
            il.Height.Set(300f, 0);
            il.Left.Set(15f, 0);
            il.Top.Set(180f, 0);
            il.names = images.Keys.ToArray();

            Append(sizeX);
            Append(sizeY);
            Append(saveName);
            Append(butt);
            Append(locationField);
            Append(il);
            base.OnInitialize();
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
                        Main.NewText(ex.Message);
                    }
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
                        return;
                    }
                    else
                    {
                        Main.NewText("Please provide a name to save as.", Color.PaleVioletRed);
                    }
                }
                catch (Exception ex)
                {
                    Main.NewText(ex.Message, Color.PaleVioletRed);
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

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate dimensions, turn them to a rectangle, and draw the input box
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();

            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/bg"), rect, Color.White);

            base.Draw(spriteBatch);
        }
    }
}

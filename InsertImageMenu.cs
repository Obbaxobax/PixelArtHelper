using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;
using System.Drawing.Drawing2D;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using Microsoft.CodeAnalysis.Text;
using System.Net.Http;
using System.IO;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using ReLogic.OS;
using System.Text.RegularExpressions;

namespace PixelArtHelper
{
    public class Menu : UIElement
    {
        private HttpClient client;
        public readonly static string savePath = Path.Combine(Main.SavePath, "PixelArtHelperImages/");

        private bool dragging = false;
        private Vector2 offset;
        private TextField locationField;
        private TextField sizeX;
        private TextField sizeY;
        private TextField saveName;

        public static Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();

        public override void OnInitialize()
        {
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

            sizeX.Left.Set(15, 0);
            sizeX.placeholderText = "Width";

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

            butt.OnLeftMouseDown += (evt, args) =>
            {
                if (string.IsNullOrEmpty(locationField.CurrentValue))
                {
                    locationField.CurrentValue = Platform.Get<IClipboard>().Value;
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

            ImageList il = new ImageList();
            il.Width.Set(345f, 0);
            il.Height.Set(300f, 0);
            il.Left.Set(15f, 0);
            il.Top.Set(180f, 0);

            Append(sizeX);
            Append(sizeY);
            Append(saveName);
            Append(butt);
            Append(locationField);
            Append(il);

            Left.Set(0, 0.7f);
            Top.Set(0, 0.5f);
            base.OnInitialize();
        }

        async private void addImage()
        {
            if (locationField.CurrentValue == null)
            {
                Main.NewText("Please supply a url or file path.", Color.PaleVioletRed);
                return;
            }

            if (locationField.CurrentValue.Contains("https://"))
            {
                try
                {
                    Stream fileStream = await client.GetStreamAsync(locationField.CurrentValue);
                    Image image = Image.FromStream(fileStream);

                    Vector2 dimensions = getDimensions(image);

                    Bitmap bm = new Bitmap(image, new Size((int)dimensions.X, (int)dimensions.Y));

                    if(saveName != null)
                    {
                        bm.Save(savePath + saveName.CurrentValue + ".png");
                        return;
                    }
                    else
                    {
                        Main.NewText("Please provide a name to save as.", Color.PaleVioletRed);
                    }
                }
                catch (HttpRequestException ex)
                {
                    if(ex.StatusCode == System.Net.HttpStatusCode.NotFound)
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
                    Image image = Image.FromFile(locationField.CurrentValue.Replace("\"", ""));

                    if(image == null)
                    {
                        Main.NewText("Invalid image! Please provide a proper file path.", Color.PaleVioletRed);
                    }

                    Vector2 dimensions = getDimensions(image);

                    Bitmap bm = new Bitmap(image, new Size((int)dimensions.X, (int)dimensions.Y));

                    if (saveName != null)
                    {
                        bm.Save(savePath + saveName.CurrentValue + ".png");
                        return;
                    }
                    else
                    {
                        Main.NewText("Please provide a name to save as.", Color.PaleVioletRed);
                    }
                }
                catch(Exception ex)
                {
                    Main.NewText(ex.Message, Color.PaleVioletRed);
                    return;
                }
            }
        }

        private Vector2 getDimensions(Image image)
        {
            int width = image.Width;
            int height = image.Height;

            if (sizeX.CurrentValue != null)
            {
                try
                {
                    width = int.Parse(sizeX.CurrentValue);
                }
                catch
                {
                    Main.NewText("Invalid width, using image's height.", Color.Azure);
                }
            }

            if (sizeY.CurrentValue != null)
            {
                try
                {
                    height = int.Parse(sizeY.CurrentValue);
                }
                catch
                {
                    Main.NewText("Invalid height, using image's height.", Color.Azure);
                }
            }

            return new Vector2(width, height);
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            if (evt.MousePosition.Y < GetDimensions().Y + 12)
            {
                offset = new Vector2(evt.MousePosition.X - GetDimensions().X, evt.MousePosition.Y - GetDimensions().Y);
                dragging = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);

            if (dragging)
            {
                Vector2 endMousePosition = evt.MousePosition;
                dragging = false;

                Left.Set(endMousePosition.X - offset.X, 0f);
                Top.Set(endMousePosition.Y - offset.Y, 0f);

                Recalculate();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X and Main.mouseX are the same
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();

            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/bg"), rect, Color.White);


            base.Draw(spriteBatch);
        }
    }

    public class TextField : UIElement
    {
        private bool typing = false;
        public string CurrentValue { get; set; } = "";
        public string placeholderText = "";
        public override void LeftClick(UIMouseEvent evt)
        {
            typing = true;
            Main.blockInput = true;
            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();
            //Main.NewText($"{rect.Y} : {Parent.Top.Pixels}");

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.CornflowerBlue);

            if (Main.keyState.IsKeyDown(Keys.Escape))
            {
                typing = false;

                PlayerInput.WritingText = false;
                Main.blockInput = false;
            }

            Vector2 pos = GetDimensions().Position() + Vector2.One * 8 + Vector2.UnitY * 4;

            if (typing == true)
            {
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                string newText = Main.GetInputText(CurrentValue);
                if (newText != CurrentValue)
                {
                    CurrentValue = newText;
                }
                string displayed = CurrentValue ?? "";

                if (Main.GameUpdateCount % 40 < 15)
                {
                    displayed += "|";
                }

                Utils.DrawBorderString(spriteBatch, displayed, pos, Color.White, 1.2f);
            }
            else if (CurrentValue != "")
            {
                Utils.DrawBorderString(spriteBatch, CurrentValue, pos, Color.White, 1.2f);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, placeholderText, pos, Color.Gray, 1.2f);
            }


            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.mouseLeft && !IsMouseHovering)
            {
                typing = false;

                PlayerInput.WritingText = false;
                Main.blockInput = false;
            }

            base.Update(gameTime);
        }
    }

    public class Button : UIElement {

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.CornflowerBlue);

            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/addButton"), new Rectangle(rect.X + 5, rect.Y + 5, rect.Width - 10, rect.Height - 10), Color.White);

            base.Draw(spriteBatch);
        }
    }

    public static class UITools {

        //I stole this from UltraSonic :)
        public static void DrawBoxWith(SpriteBatch spriteBatch, Texture2D tex, Rectangle target, Color color)
        {
            if (color == default)
                color = new Color(49, 84, 141) * 0.9f;

            var sourceCorner = new Rectangle(0, 0, 6, 6);
            var sourceEdge = new Rectangle(6, 0, 4, 6);
            var sourceCenter = new Rectangle(6, 6, 4, 4);

            Rectangle inner = target;
            inner.Inflate(-6, -6);

            spriteBatch.Draw(tex, inner, sourceCenter, color);

            spriteBatch.Draw(tex, new Rectangle(target.X + 6, target.Y, target.Width - 12, 6), sourceEdge, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y - 6 + target.Height, target.Height - 12, 6), sourceEdge, color, -(float)System.Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X - 6 + target.Width, target.Y + target.Height, target.Width - 12, 6), sourceEdge, color, (float)System.Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + 6, target.Height - 12, 6), sourceEdge, color, (float)System.Math.PI * 0.5f, Vector2.Zero, 0, 0);

            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y, 6, 6), sourceCorner, color, 0, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y, 6, 6), sourceCorner, color, (float)System.Math.PI * 0.5f, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X + target.Width, target.Y + target.Height, 6, 6), sourceCorner, color, (float)System.Math.PI, Vector2.Zero, 0, 0);
            spriteBatch.Draw(tex, new Rectangle(target.X, target.Y + target.Height, 6, 6), sourceCorner, color, (float)System.Math.PI * 1.5f, Vector2.Zero, 0, 0);
        }
    }

    public class ImageList : UIElement
    {
        private int scrollPos = 0;
        public override void OnInitialize()
        {
            for (int i = 0; i < Menu.images.Count; i++)
            {
                ListElement le = new ListElement(i);
                le.Height.Set(30f, 0);
                le.Width.Set(345f, 0);
                le.Top.Set(i * 35f + scrollPos, 0);

                Append(le);
            }

            OverflowHidden = true;
            base.OnInitialize();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(Menu.images.Count != Children.Count())
            {
                RemoveAllChildren();
                OnInitialize();
            }

            for(int i = 0; i< Children.Count(); i++)
            {
                Children.ElementAt(i).Top.Set(i * 35f + scrollPos, 0);
            }

            base.Draw(spriteBatch);
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

    public class ListElement : UIElement
    {
        private int i;

        public ListElement(int i)
        {
            this.i = i;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = GetDimensions().ToRectangle();

            Vector2 pos = new Vector2(rect.X, rect.Y) + Vector2.One * 6; //GetDimensions().Position() + Vector2.One * 8 + Vector2.UnitY * 4;

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.CornflowerBlue);
            Utils.DrawBorderString(spriteBatch, Menu.images.Keys.ToArray()[i], pos, Color.White, 1f);

            base.Draw(spriteBatch);
        }
    }
}

using ClientSideTest.HologramUI;
using ClientSideTest.UIAssets.Elements;
using ClientSideTest.UIAssets.Elements.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ClientSideTest.UIAssets.Menus
{
    public class ProcessingMenu : UIMenu
    {
        public static float percentage = 0f;
        public Image currentImage;
        
        private ProgressBar progressBar;
        private Texture2D tex;
        private Asset<Texture2D> texAsset;

        private int maxImageHeight = 270;
        private int maxImageWidth = 320;
        private Rectangle imageSize = new Rectangle();

        public override void OnInitialize()
        {
            progressBar = new ProgressBar();
            progressBar.Width.Set(355f, 0);
            progressBar.Height.Set(50f, 0);
            progressBar.Left.Set(10f, 0);
            progressBar.Top.Set(100f, 0);

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
                PixelArtHelper.imageMenu.state = "main";
            };

            Append(progressBar);
            Append(cancelButt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Recalculate();
            base.Draw(spriteBatch);
            progressBar.percentage = percentage;

            Vector2 pos = GetDimensions().Position() + new Vector2(GetDimensions().Width / 2, 60);

            Utils.DrawBorderStringBig(spriteBatch, "Processing Image", pos, Color.LightPink, scale: 0.6f, anchorx: 0.5f);

            if (texAsset != null)
            {
                Rectangle rect = GetDimensions().ToRectangle();
                rect.X += (375 - imageSize.Width) / 2;
                rect.Y += 160;
                rect.Width = imageSize.Width;
                rect.Height = imageSize.Height;

                spriteBatch.Draw(tex, rect, Color.White);
            } 
        }
        public void SetImage(Image _image)
        {
            imageSize = new Rectangle();

            if(_image.Width > _image.Height)
            {
                imageSize.Width = maxImageWidth;
                float ratio = (float)_image.Width / _image.Height;
                imageSize.Height = (int)(imageSize.Width / ratio);
            }
            else
            {
                imageSize.Height = maxImageHeight;
                float ratio = (float)_image.Height / _image.Width;
                imageSize.Width = (int)(imageSize.Height / ratio);
            }

            MemoryStream memoryStream = new MemoryStream();

            try
            {
                Bitmap image = new Bitmap(_image, (int)imageSize.Size().X, (int)imageSize.Size().Y);
                image.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;
                texAsset = ModContent.GetInstance<PixelArtHelper>().Mod.Assets.CreateUntracked<Texture2D>(memoryStream, ".png");
                tex = texAsset.Value;
            }
            catch
            {
                return;
            }
        }
    }
}

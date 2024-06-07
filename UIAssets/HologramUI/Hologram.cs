using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace ClientSideTest.HologramUI
{
    public class Hologram : UIElement
    {
        public static bool hologramMode; //Used to determine the mode (normal versus highlight)

        private Vector2 positionId;
        private Color color;
        private string name;
        private bool wall;
        private string paintName;
        private int id;

        private int hoverTextColor;

        private Vector2 basePos;
        private Point pixelWorldPos;
        private float scale;

        private bool correct = false;

        public Hologram(Vector2 positionId, Color color, string paintID, string name, int id, bool wall)
        {
            Width.Set(16f, 0);
            Height.Set(16f, 0);

            this.positionId = positionId;
            this.color = color;
            this.name = name;
            this.id = id;
            this.wall = wall;

            //convert paintids class to list
            var fields = typeof(PaintID).GetFields();

            //iterate through list, comparing to our tile
            foreach (var paint in fields)
            {
                if (paintID == paint.GetValue(null).ToString() && paintID != "0")
                {
                    //Return the name of the variable which has a matching id
                    paintName = paint.Name.Replace("/([A-Z])/g", " $1").Trim();

                    //Add paint to required paints list
                    if (PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements.ContainsKey(paintName))
                    {
                        PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements[paintName][0] += 1;
                    }
                    else
                    {
                        PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements[paintName] = [1, -1];
                    }

                    break;
                }
            }

            //Add the tile of this pixel to required tiles list
            if (PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements.ContainsKey(name))
            {
                PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name][0] += 1;
            }
            else
            {
                PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name] = [1, id];
            }

            hoverTextColor = PixelArtHelper.hoverTextColor;

            ModContent.GetInstance<PixelArtHelper>().posChanged += UpdatePosition;
            UpdatePosition();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Check if the tile in the pixel position is correct. If so, don't render the pixel to make it much easier to see
            if (!wall && Main.tile[pixelWorldPos].TileType == id && Main.tile[pixelWorldPos].HasTile)
            {
                if (!correct)
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name][0] -= 1;
                }

                correct = true;
                return;
            }
            else if (wall && Main.tile[pixelWorldPos].WallType == id && !Main.tile[pixelWorldPos].HasTile)
            {
                if (!correct)
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name][0] -= 1;
                }

                correct = true;
                return;
            }
            else if(correct)
            {
                //This is to add the item back to the required blocks list if it is broken
                PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name][0] += 1;
                correct = false;
            }

            //Return if highlight mode is enabled and we are not holding a valid block
            if (hologramMode == true && !(wall == false && Main.player[Main.myPlayer].HeldItem.createTile + 1 == id || wall == true && Main.player[Main.myPlayer].HeldItem.createWall + 1 == id)) return;

            //This is necessary to ensure the hologram lines up at all sizes
            scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
            scale = scale / Main.UIScale / 1.2f;
            Width.Set(16 * scale, 0);
            Height.Set(16 * scale, 0);

            //Calculate the position of the pixel
            var pos = basePos.ToScreenPosition();

            Left.Set(pos.X, 0);
            Top.Set(pos.Y, 0);

            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();

            //Draw the pixel
            spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, rect, color);
            //spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, pos, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            //check is mouse is over a pixel and give it hovername
            if (IsMouseHovering)
            {
                Main.instance.MouseText($"{name}\n{paintName}", rare:hoverTextColor);
            }
        }

        public void UpdatePosition()
        {
            //Convert the index of the pixel to its position in the world
            pixelWorldPos = ModContent.GetInstance<PixelArtHelper>().openPos.ToTileCoordinates() + positionId.ToPoint();
            

            scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
            scale = scale / Main.UIScale / 1.2f;
            float offset = (16 - (16 * scale)) / 2;

            // Get the position of each pixel in world space.
            basePos = ModContent.GetInstance<PixelArtHelper>().openPos + new Vector2(16 * positionId.X + offset, 16 * positionId.Y + offset); 
        }
    }
}

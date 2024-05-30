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

        public Vector2 positionId { get; set; }
        public Color color { get; set; }
        public string paintId { set; get; }
        public string name { get; set; }
        public bool wall { get; set; }
        private string paintName { get; set; }
        public int id { set; get; }

        public Hologram(Vector2 positionId, Color color, string paintId, string name, int id, bool wall)
        {
            this.positionId = positionId;
            this.color = color;
            this.paintId = paintId;
            this.name = name;
            this.id = id;
            this.wall = wall;

            //convert paintids class to list
            var fields = typeof(PaintID).GetFields();

            //iterate through list, comparing to our tile
            foreach (var paint in fields)
            {
                if (paintId == paint.GetValue(null).ToString() && paintId != "0")
                {
                    //Return the name of the variable which has a matching id
                    paintName = paint.Name.Replace("/([A-Z])/g", " $1").Trim();

                    //Add paint to required paints list
                    if (PixelArtHelper.imageMenu.reqMenu.requiredPaints.ContainsKey(paintName))
                    {
                        PixelArtHelper.imageMenu.reqMenu.requiredPaints[paintName] += 1;
                    }
                    else
                    {
                        PixelArtHelper.imageMenu.reqMenu.requiredPaints[paintName] = 1;
                    }

                    break;
                }
            }

            //Add the tile of this pixel to required tiles list
            if (PixelArtHelper.imageMenu.reqMenu.requiredTiles.ContainsKey(name))
            {
                PixelArtHelper.imageMenu.reqMenu.requiredTiles[name] += 1;
            }
            else
            {
                PixelArtHelper.imageMenu.reqMenu.requiredTiles[name] = 1;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Convert the index of the pixel to its position in the world
            Vector2 pixelWorldPos = ModContent.GetInstance<PixelArtHelper>().openPos / 16 + positionId;

            //Check if the tile in the pixel position is correct. If so, don't render the pixel to make it much easier to see
            if (wall == false && Main.tile[(int)pixelWorldPos.X, (int)pixelWorldPos.Y].TileType == id)
            {
                return;
            }
            else if (wall == true && Main.tile[(int)pixelWorldPos.X, (int)pixelWorldPos.Y].WallType + 1 == id && !Main.tile[(int)pixelWorldPos.X, (int)pixelWorldPos.Y].HasTile)
            {
                return;
            }

            //Return if highlight mode is enabled and we are not holding a valid block
            if (hologramMode == true && !(wall == false && Main.player[Main.myPlayer].HeldItem.createTile + 1 == id || wall == true && Main.player[Main.myPlayer].HeldItem.createWall + 1 == id)) return;

            //This is necessary to ensure the hologram lines up at all sizes
            float scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
            scale = scale / Main.UIScale / 1.2f;

            float offset = (16 - (16 *  scale)) / 2;

            //Calculate the position of the pixel
            Vector2 basePos;
            basePos = ModContent.GetInstance<PixelArtHelper>().openPos + new Vector2(16 * positionId.X + offset, 16 * positionId.Y + offset); // Get the position of each pixel in worldSpace then convert it to screen position. (1.3 is to center tile)
            basePos = basePos.ToScreenPosition();

            //Draw the pixel
            spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, basePos, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            //check is mouse is over a pixel and give it hovername
            if (Main.MouseScreen.X >= basePos.X && Main.MouseScreen.Y >= basePos.Y && Main.MouseScreen.X < basePos.X + 15.85 && Main.MouseScreen.Y < basePos.Y + 15.85)
            {
                Main.hoverItemName = $"{name}\n{paintName}";
            }
        }
    }
}

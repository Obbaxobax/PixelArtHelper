using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Newtonsoft.Json;
using ClientSideTest.UIAssets;

namespace ClientSideTest.HologramUI
{
    public class Tile
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

    public class Pixel
    {
        public Color color { get; set; }
        public int id { get; set; }
        public string paintId { get; set; }
        public string name { get; set; }
        public Vector2 position { get; set; }
        public bool wall { get; set; }

    }

    public class tileColor
    {
        public string Tile { get; set; }
        public string Color { get; set; }
        public float[] LAB { get; set; }
    }
    public class Hologram : UIElement
    {
        public static bool hologramMode;

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
            if (wall == false && Main.tile[(int)pixelWorldPos.X, (int)pixelWorldPos.Y].TileType + 1 == id)
            {
                return;
            }
            else if (wall == true && Main.tile[(int)pixelWorldPos.X, (int)pixelWorldPos.Y].WallType + 1 == id && !Main.tile[(int)pixelWorldPos.X, (int)pixelWorldPos.Y].HasTile)
            {
                return;
            }

            if (hologramMode == true && !(wall == false && Main.player[Main.myPlayer].HeldItem.createTile + 1 == id || wall == true && Main.player[Main.myPlayer].HeldItem.createWall + 1 == id)) return;

            Vector2 basePos;
            basePos = ModContent.GetInstance<PixelArtHelper>().openPos + new Vector2(16 * positionId.X + 1.3f, 16 * positionId.Y + 1.3f); // Get the position of each pixel in worldSpace then convert it to screen position. (1.3 is to center tile)
            basePos = basePos.ToScreenPosition();

            float scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;

            spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, basePos, null, color, 0f, Vector2.Zero, scale / 1.2f, SpriteEffects.None, 0f);

            //check is mouse is over a pixel and give it hovername
            if (Main.MouseScreen.X >= basePos.X && Main.MouseScreen.Y >= basePos.Y && Main.MouseScreen.X < basePos.X + 15.85 * scale && Main.MouseScreen.Y < basePos.Y + 15.85 * scale)
            {
                Main.hoverItemName = $"{name}\n{paintName}";
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            Deactivate();
        }
    }
}

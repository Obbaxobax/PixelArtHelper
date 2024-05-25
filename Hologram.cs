using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;

namespace ClientSideTest
{
    class tile
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public class pixel
    {
        public Color color { get; set; }
        public int id { get; set; }
        public int paintId { get; set; }
        public String name { get; set; }
        public Vector2 position { get; set; }
        public bool wall { get; set; }

    }
    public class Hologram : UIElement
    {
        public static bool hologramMode;

        public Vector2 positionId { get; set; }
        public Color color { get; set; }
        public int paintId {  set; get; }
        public string name { get; set; }
        public bool wall { get; set; }
        private string paintName { get; set; }
        public int id {  set; get; }

        public Hologram(Vector2 positionId, Color color, int paintId, string name, int id, bool wall)
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
                if (paintId == int.Parse(paint.GetValue(null).ToString()) && paintId != 0)
                {
                    //Return the name of the variable which has a matching id
                    paintName = paint.Name;
                    break;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Convert the index of the pixel to its position in the world
            Vector2 pixelWorldPos = OpenCommand.position / 16 + positionId;
            Vector2 pixelNonTilePos = OpenCommand.position + positionId * 16;

            //Check if the tile in the pixel position is correct. If so, don't render the pixel to make it much easier to see
            if (wall == false && Main.tile[(int)(pixelWorldPos.X), (int)(pixelWorldPos.Y)].TileType == id)
            {
                return;
            }
            else if (wall == true && Main.tile[(int)(pixelWorldPos.X), (int)(pixelWorldPos.Y)].WallType == id && !Main.tile[(int)(pixelWorldPos.X), (int)(pixelWorldPos.Y)].HasTile)
            {
                return;
            }

            if (hologramMode == true && !((wall == false && Main.player[Main.myPlayer].HeldItem.createTile == id) || (wall == true && Main.player[Main.myPlayer].HeldItem.createWall == id))) return;

            Vector2 basePos;
            basePos = OpenCommand.position + new Vector2(16 * positionId.X + 1.3f, 16 * positionId.Y + 1.3f); // Get the position of each pixel in worldSpace then convert it to screen position. (1.3 is to center tile)
            basePos = basePos.ToScreenPosition();

            float scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
                    
            spriteBatch.Draw((Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Blank"), basePos, null, color, 0f, Vector2.Zero, scale / 1.2f, SpriteEffects.None, 0f);

            //check is mouse is over a pixel and give it hovername
            if (Main.MouseScreen.X >= basePos.X && Main.MouseScreen.Y >= basePos.Y && Main.MouseScreen.X < basePos.X + (15.85 * scale) && Main.MouseScreen.Y < basePos.Y + (15.85 * scale))
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

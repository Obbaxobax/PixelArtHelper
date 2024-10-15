using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ClientSideTest.HologramUI
{
    public class Hologram : UIElement
    {
        public static bool hologramMode; //Used to determine the mode (normal versus highlight)
        public static bool pixelOutline;

        private Vector2 basePositionId;
        private Color color;
        private string name;
        private bool wall;
        private string paintName;
        private byte paintID;
        private int id;

        private int hoverTextColor;

        private Vector2 basePos;
        private Vector2 pos;
        private Point pixelWorldPos;
        private float scale;

        private bool correct = false;

        public Hologram(Vector2 basePositionId, Color color, string paintID, string name, int id, bool wall)
        {
            Width.Set(16f, 0);
            Height.Set(16f, 0);

            this.basePositionId = basePositionId;
            this.color = color;
            this.name = name;
            this.id = id;
            this.wall = wall;

            if (!byte.TryParse(paintID, out this.paintID)) this.paintID = 0;

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
            PixelArtHelper.placeTiles += PlacedownTiles;
            UpdatePosition();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Necessary to do this here so the screen position updates.
            pos = basePos.ToScreenPosition();

            //Epic culling (I forgot to add this in the 1.0 update and the performance difference is pretty major)
            if (pos.X < 0 || pos.X > Main.ScreenSize.X || pos.Y < 0 || pos.Y > Main.ScreenSize.Y) return;

            //Return if highlight mode is enabled and we are not holding a valid block
            if (hologramMode == true && !((wall == false && Main.player[Main.myPlayer].HeldItem.createTile == id) || (wall == true && Main.player[Main.myPlayer].HeldItem.createWall == id))) return;

            //Check if the tile in the pixel position is correct. If so, don't render the pixel to make it much easier to see
            Tile tile = Main.tile[pixelWorldPos];

            if (!wall && tile.TileType == id && tile.HasTile && tile.TileColor == paintID)
            {
                if (!correct)
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name][0] -= 1;
                }

                correct = true;
                return;
            }
            else if (wall && Main.tile[pixelWorldPos].WallType == id && !Main.tile[pixelWorldPos].HasTile && tile.WallColor == paintID)
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

            //This is necessary to ensure the hologram lines up at all sizes
            scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
            scale = scale / Main.UIScale;
            Width.Set(15 * scale, 0);
            Height.Set(15 * scale, 0);

            Left.Set(pos.X, 0);
            Top.Set(pos.Y, 0);

            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();
            Rectangle inner = rect;
            inner.Inflate(-1, -1);

            //Draw the pixel
            if (pixelOutline)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, inner, color);

                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y, rect.Width, 1), Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y, 1, rect.Height), Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), Color.White);
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, rect, color);
            }

            //check is mouse is over a pixel and give it hovername
            if (IsMouseHovering)
            {
                Main.instance.MouseText($"{name}\n{paintName}", rare:hoverTextColor);

                if (ModContent.GetInstance<PixelArtHelper>().tryAutoSelectHoverBlock.JustPressed)
                {
                    Player player = Main.LocalPlayer;

                    if (!wall)
                    {
                        FieldInfo tileIDToItemID = typeof(TileLoader).GetField("tileTypeAndTileStyleToItemType", BindingFlags.NonPublic | BindingFlags.Static);
                        Dictionary<(int, int), int> dict = (Dictionary<(int, int), int>)tileIDToItemID.GetValue(null);
                        int itemID = dict[(id, 0)];

                        if (!player.HasItem(itemID)) return;

                        int ind = player.FindItem(itemID);
                        Item oldItem = player.inventory[player.selectedItem];

                        player.inventory[player.selectedItem] = player.inventory[ind];
                        player.inventory[ind] = oldItem;
                    }
                    else
                    {
                        FieldInfo wallIDToItemID = typeof(WallLoader).GetField("wallTypeToItemType", BindingFlags.NonPublic | BindingFlags.Static);
                        Dictionary<int, int> dict = (Dictionary<int, int>)wallIDToItemID.GetValue(null);
                        int itemID = dict[id];

                        if (!player.HasItem(itemID)) return;

                        int ind = player.FindItem(itemID);
                        Item oldItem = player.inventory[player.selectedItem];

                        player.inventory[player.selectedItem] = player.inventory[ind];
                        player.inventory[ind] = oldItem;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }

        private void UpdatePosition()
        {
            //Convert the index of the pixel to its basePosition in the world
            pixelWorldPos = ModContent.GetInstance<PixelArtHelper>().openPos.ToTileCoordinates() + basePositionId.ToPoint();
            

            scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
            scale = scale / Main.UIScale / 1.2f;
            float offset = (16 - (16 * scale)) / 2;

            // Get the basePosition of each pixel in world space.
            basePos = ModContent.GetInstance<PixelArtHelper>().openPos + new Vector2(16 * basePositionId.X + offset, 16 * basePositionId.Y + offset);
        }

        private void PlacedownTiles()
        {
            if (!wall)
            {
                WorldGen.KillTile(pixelWorldPos.X, pixelWorldPos.Y, noItem: true);
                WorldGen.PlaceTile(pixelWorldPos.X, pixelWorldPos.Y, id, mute: true);
                Tile tile = Main.tile[pixelWorldPos];
                tile.TileColor = paintID;
                
            }
            else
            {
                WorldGen.KillTile(pixelWorldPos.X, pixelWorldPos.Y, noItem: true);
                WorldGen.KillWall(pixelWorldPos.X, pixelWorldPos.Y);
                WorldGen.PlaceWall(pixelWorldPos.X, pixelWorldPos.Y, id, mute: true);
                Tile tile = Main.tile[pixelWorldPos];
                tile.WallColor = paintID;
            }
        }
    }
}

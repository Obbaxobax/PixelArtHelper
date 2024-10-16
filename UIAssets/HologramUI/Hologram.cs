using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ClientSideTest.DataClasses;
using Tile = Terraria.Tile;
using Microsoft.Xna.Framework.Input;

namespace ClientSideTest.HologramUI
{
    public class Hologram : UIElement
    {
        public static bool hologramMode; //Used to determine the mode (normal versus highlight)
        public static bool pixelOutline;

        private List<Pixel> pixels;
        private List<Vector2> positions = new List<Vector2>();
        private List<Point> pixelWorldPos = new List<Point>();
        private List<string> paintNames = new List<string>();
        private List<bool> correct = new List<bool>();

        private string name;

        private int hoverTextColor;

        private Vector2 basePos;
        private float scale;

        public Hologram(List<Pixel> pixels)
        {
            this.pixels = pixels;

            name = pixels[0].name;

            hoverTextColor = PixelArtHelper.hoverTextColor;

            ModContent.GetInstance<PixelArtHelper>().posChanged += UpdatePosition;
            //PixelArtHelper.placeTiles += PlacedownTiles;
            UpdatePosition();


            for (int i = 0; i < pixels.Count; i++)
            {
                string paintName = PixelArtHelper.paintIDToName[byte.Parse(pixels[i].paintId)];
                if (paintName == "None")
                {
                    paintNames.Add("");
                }
                else
                {
                    paintNames.Add(paintName);
                }
                correct.Add(false);

                //Add paint to required paints list
                if (PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements.ContainsKey(paintName))
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements[paintName][0] += 1;
                }
                else
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements[paintName] = [1, -1];
                }

                //Add the tile of this pixel to required tiles list
                if (PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements.ContainsKey(name))
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name][0] += 1;
                }
                else
                {
                    PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[name] = [1, pixels[i].id];
                }

            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < pixels.Count; i++)
            {
                //Necessary to do this here so the screen position updates.
                var pos = positions[i].ToScreenPosition();

                //Epic culling (I forgot to add this in the 1.0 update and the performance difference is pretty major)
                if (pos.X < 0 || pos.X > Main.ScreenSize.X || pos.Y < 0 || pos.Y > Main.ScreenSize.Y) continue;

                //Return if highlight mode is enabled and we are not holding a valid block
                if (hologramMode == true && !((pixels[i].wall == false && Main.player[Main.myPlayer].HeldItem.createTile == pixels[i].id) || (pixels[i].wall == true && Main.player[Main.myPlayer].HeldItem.createWall == pixels[i].id))) continue;

                //Check if the tile in the pixel position is correct. If so, don't render the pixel to make it much easier to see
                Tile tile = Main.tile[pixelWorldPos[i]];

                if (!pixels[i].wall && tile.TileType == pixels[i].id && tile.HasTile && tile.TileColor == byte.Parse(pixels[i].paintId))
                {
                    if (!correct[i])
                    {
                        PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[pixels[i].name][0] -= 1;
                    }

                    correct[i] = true;
                    continue;
                }
                else if (pixels[i].wall && Main.tile[pixelWorldPos[i]].WallType == pixels[i].id && !Main.tile[pixelWorldPos[i]].HasTile && tile.WallColor == byte.Parse(pixels[i].paintId))
                {
                    if (!correct[i])
                    {
                        PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[pixels[i].name][0] -= 1;
                    }

                    correct[i] = true;
                    continue;
                }
                else if (correct[i])
                {
                    //This is to add the item back to the required blocks list if it is broken
                    PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements[pixels[i].name][0] += 1;
                    correct[i] = false;
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
                    spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, inner, pixels[i].color);

                    spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y, rect.Width, 1), Color.White);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y, 1, rect.Height), Color.White);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), Color.White);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), Color.White);
                }
                else
                {
                    spriteBatch.Draw(ModContent.Request<Texture2D>("ClientSideTest/Assets/Blank").Value, rect, pixels[i].color);
                }
            }

            int index = -1;

            for (int i = 0; i < positions.Count; i++)
            {
                if (correct[i]) continue;

                Vector2 closePos = positions[i].ToScreenPosition();
                Vector2 farPos = positions[i].ToScreenPosition() + Vector2.One * 15 * scale;
                closePos -= Main.MouseScreen;
                farPos -= Main.MouseScreen;

                if (closePos.X <= 0 && closePos.Y <= 0 && farPos.X >= 0 && farPos.Y >= 0)
                {
                    index = i;
                    break;
                }
            }

            //check is mouse is over a pixel and give it hovername
            if (index != -1)
            {
                Main.instance.MouseText($"{name}\n{paintNames[index]}", rare: hoverTextColor);
                
                if (ModContent.GetInstance<PixelArtHelper>().tryAutoSelectHoverBlock.JustPressed)
                {
                    Player player = Main.LocalPlayer;

                    if (!pixels[index].wall)
                    {
                        FieldInfo tileIDToItemID = typeof(TileLoader).GetField("tileTypeAndTileStyleToItemType", BindingFlags.NonPublic | BindingFlags.Static);
                        Dictionary<(int, int), int> dict = (Dictionary<(int, int), int>)tileIDToItemID.GetValue(null);
                        int itemID = dict[(pixels[index].id, 0)];

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
                        int itemID = dict[pixels[index].id];

                        if (!player.HasItem(itemID)) return;

                        int ind = player.FindItem(itemID);
                        Item oldItem = player.inventory[player.selectedItem];

                        player.inventory[player.selectedItem] = player.inventory[ind];
                        player.inventory[ind] = oldItem;
                    }
                }
            }
        }

        private void UpdatePosition()
        {
            positions.Clear();
            pixelWorldPos.Clear();

            for (int i = 0; i < pixels.Count; i++)
            {
                //Convert the index of the pixel to its basePosition in the world
                pixelWorldPos.Add(ModContent.GetInstance<PixelArtHelper>().openPos.ToTileCoordinates() + pixels[i].position.ToPoint());


                scale = Main.Camera.UnscaledSize.X / Main.Camera.ScaledSize.X;
                scale = scale / Main.UIScale / 1.2f;
                float offset = (16 - (16 * scale)) / 2;

                // Get the basePosition of each pixel in world space.
                positions.Add(ModContent.GetInstance<PixelArtHelper>().openPos + new Vector2(16 * pixels[i].position.X + offset, 16 * pixels[i].position.Y + offset));
            }
        }

        /*private void PlacedownTiles()
        {
            if (!wall)
            {
                WorldGen.KillTile(pixelWorldPos.X, pixelWorldPos.Y, noItem: true);
                WorldGen.PlaceTile(pixelWorldPos.X, pixelWorldPos.Y, pixels[i].id, mute: true);
                Tile tile = Main.tile[pixelWorldPos];
                tile.TileColor = paintID;
                
            }
            else
            {
                WorldGen.KillTile(pixelWorldPos.X, pixelWorldPos.Y, noItem: true);
                WorldGen.KillWall(pixelWorldPos.X, pixelWorldPos.Y);
                WorldGen.PlaceWall(pixelWorldPos.X, pixelWorldPos.Y, pixels[i].id, mute: true);
                Tile tile = Main.tile[pixelWorldPos];
                tile.WallColor = paintID;
            }
        }*/
    }
}

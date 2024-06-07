using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using System.Text.Json;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using System.Linq;
using ClientSideTest.UIAssets;
using ClientSideTest.DataClasses;
using Terraria;
using Tile = ClientSideTest.DataClasses.Tile;

namespace ClientSideTest.HologramUI
{
    public class HologramUIState : UIState
    {
        public static List<Pixel> pixels = new List<Pixel>(); //The list of the pixels for the hologram
        public bool processing = false;
        private static HologramUIState hologramUIState; //This

        //Hologram position
        public Vector2 openPos;

        //Dimensions of the hologram
        public Vector2 currentDimensions = new Vector2(0, 0);

        public bool imageReady = false; //Is the hologram ready to display
        public bool usePaints = false; //Should we use paints

        public override void OnInitialize()
        {
            hologramUIState = PixelArtHelper.hologramUIState;
        }

        public void Update()
        {
            RemoveAllChildren();

            //Iterate through the list of pixels
            for (int i = 0; i < pixels.Count; i++)
            {
                if (pixels[i].id == -1) continue; //Skips pixel if it is transparent

                //Create a hologram for each pixel and add it to the UI.
                Hologram hg = new Hologram(pixels[i].position, pixels[i].color, pixels[i].paintId, pixels[i].name, pixels[i].id, pixels[i].wall);

                Append(hg);
            }

            imageReady = true;
            Main.NewText("Click to place the hologram!", Color.CornflowerBlue);
        }

        public void createPixels(Bitmap bm)
        {
            try
            {
                processing = true;

                Main.NewText("Processing image. Please wait", Color.CornflowerBlue);
                GetInstance<PixelArtHelper>().HideUi(); //Hide the previous hologram
                PixelArtHelper.imageMenu.reqMenu.requiredTiles.requiredListElements.Clear();
                PixelArtHelper.imageMenu.reqMenu.requiredPaints.requiredListElements.Clear();

                //Store the pixels in a temp cache to avoid deleting existing pixels in the case this process fails
                List<Pixel> pixelCache = new List<Pixel>();

                //Get the tiles from json (includes name, id, tile-type, lab color)
                byte[] tileColors = GetFileBytes("ClientSideTest/Assets/tiles.json");
                List<TileData> tiles = JsonSerializer.Deserialize<List<TileData>>(tileColors);

                //Get lists with proper names and load them
                byte[] text = GetFileBytes($"{nameof(ClientSideTest)}/Assets/blockIDs.json");
                byte[] text2 = GetFileBytes($"{nameof(ClientSideTest)}/Assets/wallIDs.json");

                List<Tile> tile = JsonSerializer.Deserialize<List<Tile>>(text);
                List<Tile> wall = JsonSerializer.Deserialize<List<Tile>>(text2);

                currentDimensions = new Vector2(bm.Width, bm.Height);

                //Iterate through each pixel in the target image
                for (int y = 0; y < bm.Height; y++)
                {
                    for (int x = 0; x < bm.Width; x++)
                    {
                        Pixel pix = new Pixel();

                        //If the pixel's alpha is beyond a given threshold create a dummy pixel
                        if (bm.GetPixel(x, y).A < 50)
                        {
                            pix.position = new Vector2(x, y);
                            pix.color = Color.Transparent;
                            pix.id = -1;
                            pix.paintId = "0";
                            pix.name = "";
                            pix.wall = false;
                            continue;
                        }

                        //Calculate the position and color of the pixel
                        pix.position = new Vector2(x, y);
                        pix.color = new Color(bm.GetPixel(x, y).R, bm.GetPixel(x, y).G, bm.GetPixel(x, y).B);

                        //Convert the rgb to L*A*B* format for comparison
                        Vector4 labColorsVector = RGBToLab(pix.color.ToVector4());
                        float[] labColorsValues = [labColorsVector.X, labColorsVector.Y, labColorsVector.Z];

                        //preset values
                        double lowestDeltaE = 100000000;
                        double deltaE = 0;
                        string closestTile = "";

                        string paintID = "0";

                        //Iterate through a list of tiles and their corresponding L*A*B* colors
                        //Calculate the deltaE between the tiles and are pixel, storing the tile which is closest
                        foreach (TileData tc in tiles)
                        {
                            //Return if the tile uses a paint and use paints is false
                            if (!usePaints && tc.Color != "0") continue;

                            //Split the tile name to get tile-type, id, and name
                            string[] temp = tc.Tile.Split(" : ");
                            string temp2 = temp[0].Split(": ")[0];

                            //Return if the tile is in the exceptions list
                            if (temp2 == "TILE" && !ExceptionsMenu.exTiles.exceptionsDict[temp[1]]) continue;
                            if (temp2 == "WALL" && !ExceptionsMenu.exWalls.exceptionsDict[temp[1]]) continue;


                            deltaE = calculateDeltaE(tc.LAB, labColorsValues);
                            if (deltaE < lowestDeltaE)
                            {
                                lowestDeltaE = deltaE;
                                closestTile = tc.Tile;
                                paintID = tc.Color;
                            }
                        }

                        pix.paintId = paintID;

                        //Grab the if the tile is a wall and it's ID
                        closestTile = closestTile.Split(" : ")[0];
                        string[] info = closestTile.Split(": ");

                        //Get the id from string
                        pix.id = int.Parse(info[1]);

                        //Check if chosen tile is a wall or block
                        if (info[0] == "WALL")
                        {
                            pix.wall = true;

                            pix.name = wall[pix.id - 1].Name;

                            /*//Go through the list of proper names and find the one for the chosen tile
                            foreach (Tile t in wall.ToList())
                            {
                                if (t.ID == info[1])
                                {
                                    pix.name = t.Name;
                                    
                                    break;
                                }
                            }*/
                        }
                        else
                        {
                            pix.wall = false;

                            pix.name = tile[pix.id].Name;

                            /*//Go through the list of proper names and find the one for the chosen tile
                            foreach (Tile t in tile.ToList())
                            {
                                if (t.ID == info[1])
                                {
                                    pix.name = t.Name;
                                    break;
                                }
                            }*/
                        }

                        //Add the pixel to the pixel cache list
                        pixelCache.Add(pix);
                    }
                }

                //Upon completion, replace the old pixels with the new ones
                pixels = pixelCache;

                hologramUIState.Update();

                return;
            }
            catch
            {
                Main.NewText("There seems to have been a issue. Please report this on the github, with your client.log file attached.", Color.PaleVioletRed);
                processing = false;
                return;
            }
        }

        //Converts RGB color's to lab (RGB > XYZ > LAB) Uses evil math
        public static Vector4 RGBToLab(Vector4 color)
        {
            float[] xyz = new float[3];
            float[] lab = new float[3];
            float[] rgb = new float[] { color.X, color.Y, color.Z, color.W };

            rgb[0] = color.X;
            rgb[1] = color.Y;
            rgb[2] = color.Z;

            if (rgb[0] > .04045f)
            {
                rgb[0] = (float)Math.Pow((rgb[0] + .055) / 1.055, 2.4);
            }
            else
            {
                rgb[0] = rgb[0] / 12.92f;
            }

            if (rgb[1] > .04045f)
            {
                rgb[1] = (float)Math.Pow((rgb[1] + .055) / 1.055, 2.4);
            }
            else
            {
                rgb[1] = rgb[1] / 12.92f;
            }

            if (rgb[2] > .04045f)
            {
                rgb[2] = (float)Math.Pow((rgb[2] + .055) / 1.055, 2.4);
            }
            else
            {
                rgb[2] = rgb[2] / 12.92f;
            }
            rgb[0] = rgb[0] * 100.0f;
            rgb[1] = rgb[1] * 100.0f;
            rgb[2] = rgb[2] * 100.0f;


            xyz[0] = rgb[0] * .412453f + rgb[1] * .357580f + rgb[2] * .180423f;
            xyz[1] = rgb[0] * .212671f + rgb[1] * .715160f + rgb[2] * .072169f;
            xyz[2] = rgb[0] * .019334f + rgb[1] * .119193f + rgb[2] * .950227f;


            xyz[0] = xyz[0] / 95.047f;
            xyz[1] = xyz[1] / 100.0f;
            xyz[2] = xyz[2] / 108.883f;

            if (xyz[0] > .008856f)
            {
                xyz[0] = (float)Math.Pow(xyz[0], 1.0 / 3.0);
            }
            else
            {
                xyz[0] = xyz[0] * 7.787f + 16.0f / 116.0f;
            }

            if (xyz[1] > .008856f)
            {
                xyz[1] = (float)Math.Pow(xyz[1], 1.0 / 3.0);
            }
            else
            {
                xyz[1] = xyz[1] * 7.787f + 16.0f / 116.0f;
            }

            if (xyz[2] > .008856f)
            {
                xyz[2] = (float)Math.Pow(xyz[2], 1.0 / 3.0);
            }
            else
            {
                xyz[2] = xyz[2] * 7.787f + 16.0f / 116.0f;
            }

            lab[0] = 116.0f * xyz[1] - 16.0f;
            lab[1] = 500.0f * (xyz[0] - xyz[1]);
            lab[2] = 200.0f * (xyz[1] - xyz[2]);

            return new Vector4(lab[0], lab[1], lab[2], color.W);
        }

        //Calculates the deltaE shared between to LAB colors
        public static double calculateDeltaE(float[] lab1, float[] lab2)
        {
            double l = Math.Pow(lab1[0] - lab2[0], 2);
            double a = Math.Pow(lab1[1] - lab2[1], 2);
            double b = Math.Pow(lab1[2] - lab2[2], 2);

            return Math.Sqrt(l + a + b);
        }
    }
}

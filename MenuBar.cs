using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Chat;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;
using Terraria.WorldBuilding;
using System.Text.Json;
using Steamworks;
using Microsoft.CodeAnalysis;
using System.Reflection;
using PixelArtHelper;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using static System.Net.Mime.MediaTypeNames;

namespace ClientSideTest
{
    public class MenuBar : UIState
    {
        public static bool active = false;

        public static string[] lines;
        static int width;
        static int height;

        static List<string> ids = new List<string>();
        static List<string> wallOrNah = new List<string>();
        static List<string> colours = new List<string>();
        static List<string> paints = new List<string>();
        public static List<pixel> pixels = new List<pixel>();

        public static string folderLocation;

        public static bool once = false;
        public Hologram hg;
        public static MenuBar menuBar;

        public override void OnInitialize()
        {
            menuBar = PixelArtHelper.menuBar;
        }

        public static void createHologram(string fileName)
        {
            try
            {
                lines = File.ReadAllLines(folderLocation + "\\" + fileName + ".txt");
            }
            catch (FileNotFoundException)
            {
                Main.NewText("Invalid File");
                return;
            }

            //Width and height of the pixel art
            width = int.Parse(lines[1]);
            height = int.Parse(lines[2]);

            //Define the funny variables
            int baseInd;
            Color color;
            Vector2 index;
            string[] rgbValues;
            pixel pix;

            //Load lists of all block ids and their names
            byte[] text = GetFileBytes($"{nameof(PixelArtHelper)}/Assets/blockIDs.json");
            byte[] text2 = GetFileBytes($"{nameof(PixelArtHelper)}/Assets/wallIDs.json");

            List<tile> tile = JsonSerializer.Deserialize<List<tile>>(text);
            List<tile> wall = JsonSerializer.Deserialize<List<tile>>(text2);


            //Clear lists from previous hologram
            ids.Clear();
            wallOrNah.Clear();
            colours.Clear();
            paints.Clear();
            pixels.Clear();

            //Load the ids of pixel art into list
            for (int i = 1; i < (lines.Length) / 4; i++)
            {
                wallOrNah.Add(lines[(i * 4)]);
                ids.Add(lines[(i * 4) + 1]);
                paints.Add(lines[(i * 4) + 2]);
                colours.Add(lines[(i * 4) + 3]);
            }

            //Go through pixel ids and record their names as well as if they are a wall
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    pix = new pixel();
                    baseInd = (i * height) + (j);

                    //Check of the tile is a wall, tile, or if its neither, and therefore empty
                    if (wallOrNah[baseInd] == "1")
                    {
                        //Go through the tiles list to find the name
                        foreach (tile t in tile)
                        {
                            if (t.ID - 1 == int.Parse(ids[baseInd]))
                            {
                                pix.name = t.Name;
                                pix.id = t.ID - 1;
                                pix.wall = false;
                                pix.paintId = int.Parse(paints[baseInd]);
                                break;
                            }
                        }
                    }
                    else if (wallOrNah[baseInd] == "0")
                    {
                        //Go through the walls list to find the name
                        foreach (tile t in wall)
                        {
                            if (t.ID == int.Parse(ids[baseInd]))
                            {
                                pix.name = t.Name;
                                pix.id = t.ID;
                                pix.wall = true;
                                pix.paintId = int.Parse(paints[baseInd]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }

                    //Index is the where in the pixel art the pixel is located.
                    //Color is the approximate color of the pixel.
                    index = new Vector2(i, j);
                    rgbValues = colours[baseInd].Split(" ");
                    color = new Color(float.Parse(rgbValues[0]) / 255f, float.Parse(rgbValues[1]) / 255f, float.Parse(rgbValues[2]) / 255f, 0.5f);

                    //Record the values to a pixel then add it to the list of pixels
                    pix.color = color;
                    pix.position = index;
                    pixels.Add(pix);
                }
            }

            menuBar.Update();
        }

        public void Update()
        {
            RemoveAllChildren();

            //Iterate through the list of pixels
            for (int i = 0; i < pixels.Count; i++)
            {
                Hologram hg = new Hologram(pixels[i].position, pixels[i].color, pixels[i].paintId, pixels[i].name, pixels[i].id, pixels[i].wall);

                Append(hg);
            }
        }

        public void createPixels(Bitmap bm)
        {
            byte[] tileColors = GetFileBytes($"{nameof(PixelArtHelper)}/Assets/tiles.json");
            List<tile> tile = JsonSerializer.Deserialize<List<tile>>(tileColors);


            for (int y = 0; y < bm.Height; y++)
            {
                for(int x = 0; x < bm.Width; x++)
                {
                    pixel pix = new pixel();
                    pix.position = new Vector2(x, y);
                    pix.color = new Color(bm.GetPixel(x, y).R, bm.GetPixel(x, y).G, bm.GetPixel(x, y).B);
                }
            }
        }
    }
}

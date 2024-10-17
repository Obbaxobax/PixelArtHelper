using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.IO;
using ClientSideTest.HologramUI;
using ClientSideTest.UIAssets;
using ClientSideTest.UIAssets.States;
using System.Text.Json;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System;
using ClientSideTest.UIAssets.Menus;
using Terraria.ID;
using System.Linq;
using ClientSideTest.DataClasses;
using System.Threading.Tasks;
using System.Threading;
using Terraria.WorldBuilding;
using static Terraria.WorldBuilding.Actions;

namespace ClientSideTest
{
    public delegate void PosChange();

    public class PixelArtHelper : ModSystem
    {
        public static HologramUIState hologramUIState;
        private UserInterface _hologramUIState;
        public static ImageMenuState imageMenu;
        private UserInterface _imageMenu;

        public static Mod m;

        private bool active; //Bool for if the hologram is active

        //position which the hologram is opened at
        public event PosChange posChanged;

        public static Dictionary<byte, string> paintIDToName = new Dictionary<byte, string>(); //Used to convert id to name

        private Vector2 _openPos;
        public Vector2 openPos { 
            get { return _openPos; }
            set {
                _openPos = value;
                posChanged?.Invoke();
            } 
        }        

        //Keybinds
        public ModKeybind toggleImageMenu;
        public ModKeybind tryAutoSelectHoverBlock;

        public static int hoverTextColor = -12; //For accessibility

        private CancellationTokenSource tokenSource;
        private CancellationToken ct;
        private Task task;

        public override void Load()
        {
            //Assign variable for mod
            m = Mod;

            //Create keybinds
            toggleImageMenu = KeybindLoader.RegisterKeybind(m, "TogglePixelArtHelperMenu", Microsoft.Xna.Framework.Input.Keys.P);
            tryAutoSelectHoverBlock = KeybindLoader.RegisterKeybind(m, "TryAutoSelectHoveredPixel", Microsoft.Xna.Framework.Input.Keys.I);

            //Create the save data directory if it doesn't exist
            if (!Main.dedServ)
            {
                Directory.CreateDirectory(MainMenu.savePath);
            }

            //Load necessary files
            LoadFiles();

            //Setup the UI
            hologramUIState = new HologramUIState();
            hologramUIState.Activate();
            _hologramUIState = new UserInterface();
            _hologramUIState.SetState(null);

            imageMenu = new ImageMenuState();
            imageMenu.Activate();
            _imageMenu = new UserInterface();
            _imageMenu.SetState(null);

            ///Create a dictionary for converting ID's to paint names
            //convert paintids class to list
            var fields = typeof(PaintID).GetFields();

            //iterate through list, comparing to our tile
            foreach (var paint in fields)
            {
                if (paintIDToName.ContainsKey((byte)paint.GetValue(null))) continue;

                paintIDToName.Add((byte)paint.GetValue(null), paint.Name.Replace("/([A-Z])/g", " $1").Trim());
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Update the UI's if they exist/Are active
            _imageMenu?.Update(gameTime);
            if (active)
            {
                _hologramUIState?.Update(gameTime);
            }
        }

        //Hides the hologram
        public void HideUi()
        {
            active = false;
            _hologramUIState?.SetState(null);
        }

        //Shows the hologram
        public void ShowUi()
        {
            active = true;
            _hologramUIState?.SetState(hologramUIState);
        }

        //Toggles the image menu
        public void ToggleImageMenu()
        {
            if (_imageMenu.CurrentState != null)
            {
                _imageMenu.SetState(null);
            }
            else
            {
                _imageMenu.SetState(imageMenu);
            }
        }

        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod heros))
            {
                heros.Call("AddPermission", "AutoPlacePixelArt", "Auto Place Pixel Art");
                heros.Call("AddSimpleButton", "AutoPlacePixelArt", ModContent.Request<Texture2D>("ClientSideTest/Assets/Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad), (Action)ModContent.GetInstance<PixelArtHelper>().InvokeBlockPlacement, null, (Func<string>)Tooltip);
            }
        }

        private string Tooltip()
        {
            return "Auto Place Active Pixel Art";
        }

        public void InvokeBlockPlacement()
        {
            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;

            _ = Task.Run(PlacedownTiles, ct);
        }

        public void CancelBlockPlacement()
        {
            if(tokenSource == null) return;

            tokenSource.Cancel();
        }

        private void PlacedownTiles()
        {
            List<Pixel> pixels = HologramUIState.pixels;

            for (int i = 0; i < pixels.Count; i++)
            {
                Point pixelWorldPos = openPos.ToTileCoordinates() + pixels[i].position.ToPoint();
                Terraria.Tile tile = Main.tile[pixelWorldPos];

                if (!pixels[i].wall)
                {
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(pixelWorldPos.X, pixelWorldPos.Y, pixels[i].id, mute: true);
                    }
                    else
                    {
                        tile.TileType = (ushort)pixels[i].id;
                    }
                    
                    tile.LiquidAmount = 0;
                    tile.TileColor = byte.Parse(pixels[i].paintId);
                }
                else
                {
                    tile.ClearTile();
                    tile.TileType = 0;
                    tile.WallType = (ushort)pixels[i].id;

                    tile.LiquidAmount = 0;
                    tile.WallColor = byte.Parse(pixels[i].paintId);
                }

                ct.ThrowIfCancellationRequested();
            }
        }

        public override void OnWorldLoad()
        {
            //Reinitialize the menu to avoid ghost images/missing images
            imageMenu.mainMenu.Reinitialize();
            hologramUIState.RemoveAllChildren();

            if (Main.LocalPlayer.name.ToLower() == "calamitas")
            {
                hoverTextColor = -12;
            }
            else
            {
                hoverTextColor = (int)ModContent.GetInstance<ClientConfig>().HoverTextColor;
            }

            base.OnWorldLoad();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //Add the UI's to their own layers
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "PixelArtHelper: PIXEL ART",
                    delegate
                    {
                        _hologramUIState.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );

                layers.Insert(mouseTextIndex + 1, new LegacyGameInterfaceLayer(
                    "PixelArtHelper: Menu",
                    delegate
                    {
                        _imageMenu.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void OnWorldUnload()
        {
            //Save the exceptions, creating the file if it does not exist
            if (File.Exists($"{MainMenu.savePath}tileExceptions.json"))
            {
                FileStream stream = File.OpenWrite($"{MainMenu.savePath}tileExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exTiles.exceptionsDict);
            }
            else
            {
                FileStream stream = File.Create($"{MainMenu.savePath}tileExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exTiles.exceptionsDict);
            }

            if (File.Exists($"{MainMenu.savePath}wallExceptions.json"))
            {
                FileStream stream = File.OpenWrite($"{MainMenu.savePath}wallExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exWalls.exceptionsDict);
            }
            else
            {
                FileStream stream = File.Create($"{MainMenu.savePath}wallExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exWalls.exceptionsDict);
            }

            imageMenu.state = "main";
            CancelBlockPlacement();

            base.OnWorldUnload();
        }

        private void LoadFiles()
        {
            string fileBytes;

            //Load the exceptions files
            if (File.Exists($"{MainMenu.savePath}wallExceptions.json"))
            {
                //Opens the saved file and copies it to the dictionary
                using (StreamReader r = new StreamReader($"{MainMenu.savePath}wallExceptions.json"))
                {
                    string json = r.ReadToEnd();
                    ExceptionsMenu.exWalls.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);
                }
            }
            else
            {
                //Creates a new file, copying the preset one from assets
                fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/wallExceptions.json"));
                ExceptionsMenu.exWalls.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);

                FileStream stream = File.Create($"{MainMenu.savePath}wallExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exWalls.exceptionsDict);
            }

            if (File.Exists($"{MainMenu.savePath}tileExceptions.json"))
            {
                //Opens the saved file and copies it to the dictionary
                using (StreamReader r = new StreamReader($"{MainMenu.savePath}tileExceptions.json"))
                {
                    string json = r.ReadToEnd();
                    ExceptionsMenu.exTiles.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);
                }

            }
            else
            {
                //Creates a new file, copying the preset one from assets
                fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/tileExceptions.json"));
                ExceptionsMenu.exTiles.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);

                FileStream stream = File.Create($"{MainMenu.savePath}tileExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exTiles.exceptionsDict);
            }

            //Load preset exceptions
            fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/wallExceptionsPreset1.json"));
            ExceptionsMenu.wallsPreset1.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);

            fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/tileExceptionsPreset1.json"));
            ExceptionsMenu.tilesPreset1.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);
        }
    }
}
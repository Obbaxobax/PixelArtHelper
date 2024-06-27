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

        private Vector2 _openPos;
        public Vector2 openPos { 
            get { return _openPos; }
            set {
                _openPos = value;
                posChanged?.Invoke();
            } 
        }        

        public ModKeybind toggleImageMenu; //Keybind

        public static int hoverTextColor = -12; //For accessibility

        public override void Load()
        {
            //Assign variable for mod
            m = Mod;

            //Create keybind
            toggleImageMenu = KeybindLoader.RegisterKeybind(m, "TogglePixelArtHelperMenu", Microsoft.Xna.Framework.Input.Keys.P);

            //Create the save data directory if it doesn't exist
            if (!Main.dedServ)
            {
                Directory.CreateDirectory(MainMenu.savePath);
            }

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
                string fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/wallExceptions.json"));
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
                string fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/tileExceptions.json"));
                ExceptionsMenu.exTiles.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);

                FileStream stream = File.Create($"{MainMenu.savePath}tileExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exTiles.exceptionsDict);
            }

            //Setup the UI
            hologramUIState = new HologramUIState();
            hologramUIState.Activate();
            _hologramUIState = new UserInterface();
            _hologramUIState.SetState(null);

            imageMenu = new ImageMenuState();
            imageMenu.Activate();
            _imageMenu = new UserInterface();
            _imageMenu.SetState(null);
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

            base.OnWorldUnload();
        }
    }
}
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
    public class PixelArtHelper : ModSystem
    {
        public static HologramUIState hologramUIState;
        private UserInterface _hologramUIState;
        public static ImageMenuState imageMenu;
        private UserInterface _imageMenu;
        public static Mod m;

        private bool active;
        public Vector2 openPos;

        public ModKeybind ToggleImageMenu;

        public override void Load()
        {
            m = Mod;

            ToggleImageMenu = KeybindLoader.RegisterKeybind(m, "ToggleImageMenu", Microsoft.Xna.Framework.Input.Keys.P);

            if (File.Exists($"{MainMenu.savePath}wallExceptions.json"))
            {
                using (StreamReader r = new StreamReader($"{MainMenu.savePath}wallExceptions.json"))
                {
                    string json = r.ReadToEnd();
                    ExceptionsMenu.exWalls.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);
                }
            }
            else
            {
                string fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/wallExceptions.json"));
                ExceptionsMenu.exWalls.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);

                FileStream stream = File.Create($"{MainMenu.savePath}wallExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exWalls.exceptionsDict);
            }

            if (File.Exists($"{MainMenu.savePath}tileExceptions.json")) 
            {
                using (StreamReader r = new StreamReader($"{MainMenu.savePath}tileExceptions.json"))
                {
                    string json = r.ReadToEnd();
                    ExceptionsMenu.exTiles.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(json);
                }

            }
            else
            {
                string fileBytes = Encoding.UTF8.GetString(ModContent.GetFileBytes("ClientSideTest/Assets/tileExceptions.json"));
                ExceptionsMenu.exTiles.exceptionsDict = JsonSerializer.Deserialize<Dictionary<string, bool>>(fileBytes);

                FileStream stream = File.Create($"{MainMenu.savePath}tileExceptions.json");
                JsonSerializer.Serialize(stream, ExceptionsMenu.exTiles.exceptionsDict);
            }

            hologramUIState = new HologramUIState();
            hologramUIState.Activate();
            _hologramUIState = new UserInterface();
            _hologramUIState.SetState(null);

            imageMenu = new ImageMenuState();
            imageMenu.Activate();
            _imageMenu = new UserInterface();
            _imageMenu.SetState(null);

            if (!Main.dedServ)
            {
                Directory.CreateDirectory(MainMenu.savePath);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _imageMenu?.Update(gameTime);
            if (active)
            {
                _hologramUIState?.Update(gameTime);
            }

            if (hologramUIState.imageReady == true && Main.mouseLeft)
            {
                Vector2 pos = Main.MouseWorld;
                float dif = pos.X % 16;
                if (dif != 0)
                {
                    pos.X = pos.X - dif;
                }

                float dif2 = pos.Y % 16;
                if (dif2 != 0)
                {
                    pos.Y = pos.Y - dif2;
                }
                openPos = pos;

                showUi();
                imageMenu.state = "required";
                hologramUIState.imageReady = false;
            }
        }

        public void hideUi()
        {
            active = false;
            _hologramUIState?.SetState(null);
        }

        public void showUi()
        {
            active = true;
            _hologramUIState?.SetState(hologramUIState);
        }

        public void ToggleImageMneu()
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
            imageMenu.mainMenu.Reinitialize();
            hologramUIState.RemoveAllChildren();

            base.OnWorldLoad();
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
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

            base.OnWorldUnload();
        }
    }
}
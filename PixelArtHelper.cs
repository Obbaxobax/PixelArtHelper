using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.IO;
using ClientSideTest.HologramUI;
using ClientSideTest.UIAssets;
using ClientSideTest.UIAssets.States;

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

        public override void Load()
		{
			hologramUIState = new HologramUIState();
			hologramUIState.Activate();
			_hologramUIState = new UserInterface();
			_hologramUIState.SetState(null);

			imageMenu = new ImageMenuState();
			imageMenu.Activate();
			_imageMenu = new UserInterface();
			_imageMenu.SetState(imageMenu);

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

        public override void OnWorldLoad()
        {
			imageMenu.menu.Reinitialize();
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
                        _imageMenu.Draw(Main.spriteBatch, new GameTime());
                        return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
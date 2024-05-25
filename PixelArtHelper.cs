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
using System.IO;
using log4net.Repository.Hierarchy;
using System.Security.Cryptography.X509Certificates;
using PixelArtHelper;

namespace ClientSideTest
{
	public class PixelArtHelper : ModSystem
	{
		public static MenuBar menuBar;
		private UserInterface _menuBar;
        public static ImageMenuState imageMenu;
        private UserInterface _imageMenu;
        public static Mod m;

        public override void Load()
		{
			menuBar = new MenuBar();
			menuBar.Activate();
			_menuBar = new UserInterface();
			_menuBar.SetState(menuBar);

			imageMenu = new ImageMenuState();
			imageMenu.Activate();
			_imageMenu = new UserInterface();
			_imageMenu.SetState(imageMenu);
        }

		public override void UpdateUI(GameTime gameTime)
		{
			_imageMenu?.Update(gameTime);
			if (MenuBar.active)
			{
				_menuBar?.Update(gameTime);
			}
		}

		public void hideUi()
		{
			_menuBar?.SetState(null);
		}

        public void showUi()
        {
            _menuBar?.SetState(menuBar);
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
						_menuBar.Draw(Main.spriteBatch, new GameTime());
                        _imageMenu.Draw(Main.spriteBatch, new GameTime());
                        return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}

	public class PixelArtHelperMod : Mod
	{
        public override void Load()
        {
			if (!Main.dedServ)
			{
				Directory.CreateDirectory(Menu.savePath);
			}

            base.Load();
        }
    }
}
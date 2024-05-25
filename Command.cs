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
namespace ClientSideTest
{
    class OpenCommand : ModCommand
    {
        public static Vector2 position = Main.MouseWorld;
        public override string Command => "openHolo";
        public override CommandType Type => CommandType.Chat;
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                Main.NewText("Please provide a file name.", Color.Red);
                return;
            }

            //Hologram.flushSpriteLists();

            //Hologram.createHologram(args[0]);
            MenuBar.createHologram(args[0]);
            MenuBar.active = true;

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
            position = pos;

            ModContent.GetInstance<PixelArtHelper>().showUi();
        }
    }

    class CloseCommand : ModCommand
    {
        public override string Command => "closeHolo";
        public override CommandType Type => CommandType.Chat;
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            MenuBar.active = false;
            ModContent.GetInstance<PixelArtHelper>().hideUi();
            MenuBar.pixels.Clear();
        }
    }

    class TestCommand : ModCommand
    {
        public override string Command => "test";
        public override CommandType Type => CommandType.Chat;
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            //Main.NewText(scale);
            Main.NewText(MenuBar.pixels.Count());
        }
    }
}

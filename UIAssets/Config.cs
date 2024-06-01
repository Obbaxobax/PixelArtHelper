using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ClientSideTest.UIAssets
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(textColor.Amber)]
        public textColor HoverTextColor;

        public override void OnChanged()
        {
            PixelArtHelper.hoverTextColor = Main.LocalPlayer.name.ToLower() == "calamitas" ? -12 : (int)HoverTextColor;

            base.OnChanged();
        }
    }

    public enum textColor
    {
        White = 0,
        Blue = 1, 
        Green = 2,
        Orange = 3,
        LightRed = 4,
        Pink = 5,
        LightPurple = 6,
        Lime = 7,
        Yellow = 8,
        Cyan = 9,
        Red = 10,
        Purple = 11,
        Amber = -11
    }
}

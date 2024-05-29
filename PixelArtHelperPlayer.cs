using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ClientSideTest
{
    public class PixelArtHelperPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            base.PostUpdate();

            if (ModContent.GetInstance<PixelArtHelper>().ToggleImageMenu.JustPressed)
            {
                ModContent.GetInstance<PixelArtHelper>().ToggleImageMneu();
            }
        }
    }
}

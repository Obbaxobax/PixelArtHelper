using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;

namespace PixelArtHelper
{
    public class ImageMenuState : UIState
    {
        public override void OnInitialize()
        {
            Menu menu = new Menu();
            menu.Height.Set(500, 0);
            menu.Width.Set(375, 0);

            Append(menu);
        }
    }
}

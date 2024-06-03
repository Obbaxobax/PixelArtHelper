using ClientSideTest.HologramUI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Button to toggle between normal and alternate mode
    public class HologramToggleButton : ToggleButton
    {
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            //Changes bool to opposite value and toggles the mode
            Hologram.hologramMode = !Hologram.hologramMode;
            state = !state;

            base.LeftClick(evt);
        }
    }
}

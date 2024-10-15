using ClientSideTest.UIAssets.Elements.Lists;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Button to toggle whether to use paints
    public class PaintToggleButton : Button
    {
        private ImageListElement listElement;

        public PaintToggleButton(ImageListElement listElement)
        {
            this.listElement = listElement;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            //Toggle paints on or off
            listElement.usePaints = !listElement.usePaints;

            //Change button texture to reflect current state
            texture = listElement.usePaints ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";
        }
    }
}

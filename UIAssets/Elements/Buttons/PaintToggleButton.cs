using Terraria.UI;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Button to toggle whether to use paints
    public class PaintToggleButton : Button
    {
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            //Toggle paints on or off
            PixelArtHelper.hologramUIState.usePaints = !PixelArtHelper.hologramUIState.usePaints;

            //Change button texture to reflect current state
            texture = PixelArtHelper.hologramUIState.usePaints ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";

            base.LeftClick(evt);
        }
    }
}

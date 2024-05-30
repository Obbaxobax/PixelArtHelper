using ClientSideTest.UIAssets.Elements.Buttons;
using ClientSideTest.UIAssets.Elements.Lists;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace ClientSideTest.UIAssets
{
    public class RequiredItemsMenu : UIMenu
    {
        //Dictionaries to store the number of required tiles and paints
        public Dictionary<string, int> requiredTiles = new Dictionary<string, int>();
        public Dictionary<string, int> requiredPaints = new Dictionary<string, int>();

        public override void OnInitialize()
        {
            //Button to close holo and return to main menu
            Button backButt = new Button();
            backButt.Left.Set(339f, 0);
            backButt.Width.Set(36f, 0);
            backButt.Height.Set(36f, 0);
            backButt.texture = "ClientSideTest/Assets/backButton";
            backButt.boxColor = Color.PaleVioletRed;
            backButt.hoverText = "Go back to main menu.";

            //Is this better than LeftClick?
            backButt.OnLeftMouseDown += (evt, args) =>
            {
                PixelArtHelper.imageMenu.state = "main";
                ModContent.GetInstance<PixelArtHelper>().HideUi();
            };

            Append(backButt);

            //Button to toggle highlight mode on and off
            HologramToggleButton holoButt = new HologramToggleButton();
            holoButt.Height.Set(36f, 0);
            holoButt.Width.Set(36f, 0);
            holoButt.Left.Set(303f, 0);
            holoButt.boxColor = Color.BlueViolet;
            holoButt.texture = "ClientSideTest/Assets/activeButton";
            holoButt.hoverText = "Toggles between displaying all pixels\n or displaying pixels based on the held tile.";

            Append(holoButt);

            //List of required tiles
            RequiredList list = new RequiredList();
            list.Top.Set(46f, 0);
            list.Left.Set(15f, 0);
            list.Width.Set(345f, 0);
            list.Height.Set(250f, 0);
            list.list = requiredTiles;

            Append(list);

            //List of required paints
            list = new RequiredList();
            list.Top.Set(316f, 0);
            list.Left.Set(15f, 0);
            list.Width.Set(345f, 0);
            list.Height.Set(165f, 0);
            list.list = requiredPaints;

            Append(list);

            base.OnInitialize();
        }
    }
}

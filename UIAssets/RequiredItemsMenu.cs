using ClientSideTest.UIAssets.Elements.Buttons;
using ClientSideTest.UIAssets.Elements.Lists;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace ClientSideTest.UIAssets
{
    public class RequiredItemsMenu : UIMenu
    {
        //Dictionaries to store the number of required tiles and paints
        public requiredItemsElement requiredTiles = new requiredItemsElement(new Dictionary<string, int>());
        public requiredItemsElement requiredPaints = new requiredItemsElement(new Dictionary<string, int>());
        public RequiredList tilesList;
        public RequiredList paintList;

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
            tilesList = new RequiredList();
            tilesList.Top.Set(46f, 0);
            tilesList.Left.Set(15f, 0);
            tilesList.Width.Set(345f, 0);
            tilesList.Height.Set(250f, 0);
            tilesList.list = requiredTiles;

            Append(tilesList);

            //List of required paints
            paintList = new RequiredList();
            paintList.Top.Set(316f, 0);
            paintList.Left.Set(15f, 0);
            paintList.Width.Set(345f, 0);
            paintList.Height.Set(165f, 0);
            paintList.list = requiredPaints;

            Append(paintList);

            base.OnInitialize();
        }
        public void UpdateChildren()
        {
            tilesList.RemoveAllChildren();
            tilesList.OnInitialize();
        }
    }

    public class requiredItemsElement
    {
        public Dictionary<string, int> requiredListElements = new Dictionary<string, int>();

        public requiredItemsElement(Dictionary<string, int> dict)
        {
            requiredListElements = dict;
        }
    }
}

using ClientSideTest.DataClasses;
using ClientSideTest.UIAssets.Elements.Lists;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace ClientSideTest.UIAssets.Menus
{
    //Menu for setting tile exceptions
    public class ExceptionsMenu : UIMenu
    {
        //Classes for the exceptions
        public static Exceptions exTiles = new Exceptions(new Dictionary<string, bool>());
        public static Exceptions exWalls = new Exceptions(new Dictionary<string, bool>());

        private ExceptionsList exTilesList;
        private ExceptionsList exWallsList;

        private List<Tile> elementsTiles;
        private List<Tile> elementsWalls;

        public override void OnInitialize()
        {
            //Button to go back to main menu
            Button butt = new Button();
            butt.Left.Set(339f, 0);
            butt.Width.Set(36f, 0);
            butt.Height.Set(36f, 0);
            butt.texture = "ClientSideTest/Assets/backButton";
            butt.boxColor = Color.PaleVioletRed;
            butt.hoverText = "Go back to main menu.";

            //LeftMouseDown may be more responsive than LeftClick but idk
            butt.OnLeftMouseDown += (evt, args) =>
            {
                PixelArtHelper.imageMenu.state = "main";
            };

            Append(butt);

            //Text which reads tiles above first list
            UIText tilesTitle = new UIText("Tiles");
            tilesTitle.TextColor = Color.LightPink;
            tilesTitle.Top.Set(41f, 0);
            tilesTitle.Left.Set(15f, 0);
            tilesTitle.Width.Set(345f, 0);
            tilesTitle.Height.Set(10f, 0);

            Append(tilesTitle);

            Button chngTilesSort = new Button();
            chngTilesSort.hoverText = "Sort: Default";
            chngTilesSort.Width.Set(25f, 0);
            chngTilesSort.Height.Set(25f, 0);
            chngTilesSort.Top.Set(36f, 0);
            chngTilesSort.Left.Set(340f, 0);
            chngTilesSort.texture = "ClientSideTest/Assets/sortButton";

            chngTilesSort.OnLeftMouseDown += (evt, args) =>
            {
                var sort = elementsTiles;
                switch (exTilesList.currentSort)
                {
                    case 0:
                        sort = elementsTiles.OrderBy(e => e.Name).ToList();
                        exTilesList.ChangeSort(sort);
                        exTilesList.currentSort += 1;
                        chngTilesSort.hoverText = "Sort: Alphabetical";
                        break;
                    case 1:
                        sort = elementsTiles.OrderByDescending(e => exTiles.exceptionsDict[e.Name]).ToList();
                        exTilesList.ChangeSort(sort);
                        exTilesList.currentSort += 1;
                        chngTilesSort.hoverText = "Sort: By Enabled";
                        break;
                    case 2:
                        exTilesList.ChangeSort(elementsTiles);
                        exTilesList.currentSort = 0;
                        chngTilesSort.hoverText = "Sort: Default";
                        break;
                }
            };

            Append(chngTilesSort);

            //Create the list and pass the exceptions dict into it (I do this as a class so I can avoid making two classes for each list of exceptions)
            exTilesList = new ExceptionsList(exTiles);

            //Get the list of all blocks
            byte[] text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/blockIDs.json");

            //Load block list into
            exTilesList.elements = elementsTiles = JsonSerializer.Deserialize<List<Tile>>(text);

            exTilesList.Top.Set(66f, 0);
            exTilesList.Left.Set(15f, 0);
            exTilesList.Width.Set(345f, 0);
            exTilesList.Height.Set(190f, 0);
            exTilesList.elementPerRow = 2;

            Append(exTilesList);

            //Text which says walls above second list
            tilesTitle = new UIText("Walls");
            tilesTitle.TextColor = Color.LightPink;
            tilesTitle.Top.Set(271f, 0);
            tilesTitle.Left.Set(15f, 0);
            tilesTitle.Width.Set(345f, 0);
            tilesTitle.Height.Set(10f, 0);

            Append(tilesTitle);

            Button chngWallsSort = new Button();
            chngWallsSort.hoverText = "Sort: Default";
            chngWallsSort.Width.Set(25f, 0);
            chngWallsSort.Height.Set(25f, 0);
            chngWallsSort.Top.Set(266f, 0);
            chngWallsSort.Left.Set(340f, 0);
            chngWallsSort.texture = "ClientSideTest/Assets/sortButton";

            chngWallsSort.OnLeftMouseDown += (evt, args) =>
            {
                var sort = elementsWalls;
                switch (exWallsList.currentSort)
                {
                    case 0:
                        sort = elementsWalls.OrderBy(e => e.Name).ToList();
                        exWallsList.ChangeSort(sort);
                        exWallsList.currentSort += 1;
                        chngWallsSort.hoverText = "Sort: Alphabetical";
                        break;
                    case 1:
                        sort = elementsWalls.OrderByDescending(e => exWalls.exceptionsDict[e.Name]).ToList();
                        exWallsList.ChangeSort(sort);
                        exWallsList.currentSort += 1;
                        chngWallsSort.hoverText = "Sort: By Enabled";
                        break;
                    case 2:
                        exWallsList.ChangeSort(elementsWalls);
                        exWallsList.currentSort = 0;
                        chngWallsSort.hoverText = "Sort: Default";
                        break;
                }
            };

            Append(chngWallsSort);

            //Create the second list and pass the exceptions dict into it (reusing the variable because I am silly)
            exWallsList = new ExceptionsList(exWalls);

            //Get the list of all blocks
            text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/wallIDs.json");

            //Load block list into
            exWallsList.elements = elementsWalls = JsonSerializer.Deserialize<List<Tile>>(text);

            exWallsList.Top.Set(296f, 0);
            exWallsList.Left.Set(15f, 0);
            exWallsList.Width.Set(345f, 0);
            exWallsList.Height.Set(190f, 0);
            exWallsList.elementPerRow = 2;

            Append(exWallsList);

            base.OnInitialize();
        }
    }

    //Epic exceptions class which is used to store exceptions dict
    public class Exceptions
    {
        public Dictionary<string, bool> exceptionsDict = new Dictionary<string, bool>();

        public Exceptions(Dictionary<string, bool> exceptionsDict)
        {
            this.exceptionsDict = exceptionsDict;
        }
    }
}

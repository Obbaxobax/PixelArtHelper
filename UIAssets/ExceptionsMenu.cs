using ClientSideTest.DataClasses;
using ClientSideTest.UIAssets.Elements.Lists;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace ClientSideTest.UIAssets
{
    //Menu for setting tile exceptions
    public class ExceptionsMenu : UIMenu
    {
        //Classes for the exceptions
        public static Exceptions exTiles = new Exceptions(new Dictionary<string, bool>());
        public static Exceptions exWalls = new Exceptions(new Dictionary<string, bool>());

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

            //Create the list and pass the exceptions dict into it (I do this as a class so I can avoid making two classes for each list of exceptions)
            ExceptionsList el = new ExceptionsList(exTiles);

            //Get the list of all blocks
            byte[] text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/blockIDs.json");

            //Load block list into
            el.elements = JsonSerializer.Deserialize<List<Tile>>(text);

            el.Top.Set(66f, 0);
            el.Left.Set(15f, 0);
            el.Width.Set(345f, 0);
            el.Height.Set(190f, 0);
            el.elementPerRow = 2;

            Append(el);

            //Text which says walls above second list
            tilesTitle = new UIText("Walls");
            tilesTitle.TextColor = Color.LightPink;
            tilesTitle.Top.Set(271f, 0);
            tilesTitle.Left.Set(15f, 0);
            tilesTitle.Width.Set(345f, 0);
            tilesTitle.Height.Set(10f, 0);

            Append(tilesTitle);

            //Create the second list and pass the exceptions dict into it (reusing the variable because I am silly)
            el = new ExceptionsList(exWalls);

            //Get the list of all blocks
            text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/wallIDs.json");

            //Load block list into
            el.elements = JsonSerializer.Deserialize<List<Tile>>(text);

            el.Top.Set(296f, 0);
            el.Left.Set(15f, 0);
            el.Width.Set(345f, 0);
            el.Height.Set(190f, 0);
            el.elementPerRow = 2;

            Append(el);

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

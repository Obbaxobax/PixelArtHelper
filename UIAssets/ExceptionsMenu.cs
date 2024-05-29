using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Tile = ClientSideTest.HologramUI.Tile;

namespace ClientSideTest.UIAssets
{
    public class ExceptionsMenu : UIMenu
    {
        public static Exceptions exTiles = new Exceptions(new Dictionary<string, bool>());
        public static Exceptions exWalls = new Exceptions(new Dictionary<string, bool>());

        public override void OnInitialize()
        {
            Button butt = new Button();
            butt.Left.Set(339f, 0);
            butt.Width.Set(36f, 0);
            butt.Height.Set(36f, 0);
            butt.texture = "ClientSideTest/Assets/backButton";
            butt.boxColor = Color.PaleVioletRed;
            butt.hoverText = "Go back to main menu.";

            //LeftMouseDown may be more responsive than LeftClick
            butt.OnLeftMouseDown += (evt, args) =>
            {
                PixelArtHelper.imageMenu.state = "main";
            };

            Append(butt);

            UIText tilesTitle = new UIText("Tiles");
            tilesTitle.TextColor = Color.LightPink;
            tilesTitle.Top.Set(41f, 0);
            tilesTitle.Left.Set(15f, 0);
            tilesTitle.Width.Set(345f, 0);
            tilesTitle.Height.Set(10f, 0);

            Append(tilesTitle);

            byte[] text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/blockIDs.json");

            ExceptionsList el = new ExceptionsList(exTiles);

            el.elements = JsonSerializer.Deserialize<List<Tile>>(text);

            el.Top.Set(66f, 0);
            el.Left.Set(15f, 0);
            el.Width.Set(345f, 0);
            el.Height.Set(190f, 0);
            el.elementPerRow = 2;

            Append(el);

            tilesTitle = new UIText("Walls");
            tilesTitle.TextColor = Color.LightPink;
            tilesTitle.Top.Set(271f, 0);
            tilesTitle.Left.Set(15f, 0);
            tilesTitle.Width.Set(345f, 0);
            tilesTitle.Height.Set(10f, 0);

            Append(tilesTitle);

            text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/wallIDs.json");

            el = new ExceptionsList(exWalls);

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

    public class Exceptions
    {
        public Dictionary<string, bool> exceptionsDict = new Dictionary<string, bool>();

        public Exceptions(Dictionary<string, bool> exceptionsDict)
        {
            this.exceptionsDict = exceptionsDict;
        }
    }
}

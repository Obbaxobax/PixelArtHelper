using ClientSideTest.HologramUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ClientSideTest.UIAssets
{
    public class ExceptionsMenu : DraggableUIMenu
    {
        public Dictionary<string, bool> exTiles = new Dictionary<string, bool>();
        public Dictionary<string, bool> exWalls = new Dictionary<string, bool>();

        public override void OnInitialize()
        {
            byte[] text = ModContent.GetFileBytes($"{nameof(ClientSideTest)}/Assets/blockIDs.json");

            ExceptionsList el = new ExceptionsList();

            el.elements = JsonSerializer.Deserialize<List<Tile>>(text);

            el.Top.Set(15f, 0);
            el.Left.Set(15f, 0);
            el.Width.Set(345f, 0);
            el.Height.Set(230f, 0);

            base.OnInitialize();
        }
    }
}

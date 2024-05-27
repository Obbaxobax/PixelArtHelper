using Terraria.UI;

namespace ClientSideTest.UIAssets.States
{
    public class ImageMenuState : UIState
    {
        public MainMenu menu;
        public ExceptionsMenu exMenu;
        public override void OnInitialize()
        {
            menu = new MainMenu();
            menu.Height.Set(500, 0);
            menu.Width.Set(375, 0);

            Append(menu);
        }
    }
}

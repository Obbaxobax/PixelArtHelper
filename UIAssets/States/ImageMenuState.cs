using Vector2 = Microsoft.Xna.Framework.Vector2;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria;
using ClientSideTest.UIAssets.HologramUI;

namespace ClientSideTest.UIAssets.States
{
    public class ImageMenuState : UIState
    {
        private bool dragging = false;
        private Vector2 offset;

        public MainMenu mainMenu;
        public ExceptionsMenu exMenu;
        public RequiredItemsMenu reqMenu;

        private string _state;
        public string state { 
            get { return _state;  } 
            set { 
                _state = value;
                OnStateChange();
            } 
        }
        public override void OnInitialize()
        {
            Left.Set(700f, 0);
            Top.Set(500f, 0);
            Width.Set(375f, 0);
            Height.Set(500f, 0);

            Vector2 offset = new Vector2(700, 500);

            mainMenu = new MainMenu();
            mainMenu.Height.Set(500f, 0);
            mainMenu.Width.Set(375f, 0);
            //menu.Left.Set(700f, 0);
            //menu.Top.Set(500f, 0);

            exMenu = new ExceptionsMenu();
            exMenu.Height.Set(500f, 0);
            exMenu.Width.Set(375f, 0);
            //exMenu.Left.Set(700f, 0);
            //exMenu.Top.Set(500f, 0);

            reqMenu = new RequiredItemsMenu();
            reqMenu.Height.Set(500f, 0);
            reqMenu.Width.Set(375f, 0);
            //reqMenu.Left.Set(700f, 0);
            //reqMenu.Top.Set(500f, 0);

            state = "main";
        }

        private void OnStateChange()
        {
            switch(state)
            {
                case "exceptions":
                    RemoveAllChildren();
                    exMenu.Activate();
                    Append(exMenu);
                    break;
                case "main":
                    RemoveAllChildren();
                    mainMenu.Activate();
                    Append(mainMenu);
                    Append(new HologramOutline());
                    break;
                case "required":
                    RemoveAllChildren();
                    reqMenu.Activate();
                    Append(reqMenu);
                    break;
            }
        }
        public override void Update(GameTime gameTime)
        {
            //Check if mouse is on screen
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Update the panel position if the player is dragging it
            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X and Main.mouseX are the same
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            base.Update(gameTime);
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            //Start dragging the panel if the top of it is clicked
            if (evt.MousePosition.Y < GetDimensions().Y + 36)
            {
                offset = new Vector2(evt.MousePosition.X - GetDimensions().X, evt.MousePosition.Y - GetDimensions().Y);
                dragging = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);

            //Disable dragging at set the position to where it ended
            if (dragging)
            {
                Vector2 endMousePosition = evt.MousePosition;
                dragging = false;

                Left.Set(endMousePosition.X - offset.X, 0f);
                Top.Set(endMousePosition.Y - offset.Y, 0f);

                Recalculate();
            }
        }
    }
}

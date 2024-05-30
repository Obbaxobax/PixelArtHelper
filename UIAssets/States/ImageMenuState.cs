using Vector2 = Microsoft.Xna.Framework.Vector2;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Terraria;
using ClientSideTest.UIAssets.HologramUI;

namespace ClientSideTest.UIAssets.States
{
    //Controls all the menus and allows for dragging of menu
    public class ImageMenuState : UIState
    {
        private bool dragging = false;
        private Vector2 offset; //Offset of the panel after dragging

        //Each of the menus
        public MainMenu mainMenu;
        public ExceptionsMenu exMenu;
        public RequiredItemsMenu reqMenu;

        //Variables to store the current state of the UI and update it upon change
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
            //Set dimensions to be used by all menus
            Left.Set(700f, 0);
            Top.Set(500f, 0);
            Width.Set(375f, 0);
            Height.Set(500f, 0);

            //Create all the menus
            mainMenu = new MainMenu();
            mainMenu.Height.Set(500f, 0);
            mainMenu.Width.Set(375f, 0);

            exMenu = new ExceptionsMenu();
            exMenu.Height.Set(500f, 0);
            exMenu.Width.Set(375f, 0);

            reqMenu = new RequiredItemsMenu();
            reqMenu.Height.Set(500f, 0);
            reqMenu.Width.Set(375f, 0);

            //Set the starting menu to main
            state = "main";
        }

        //Fires when the value of state is changed
        private void OnStateChange()
        {
            switch(state)
            {
                //each removes the previous menu and appends new one
                case "exceptions": 
                    RemoveAllChildren();
                    exMenu.Activate();
                    Append(exMenu);
                    break;
                case "main":
                    RemoveAllChildren();
                    mainMenu.Activate();
                    Append(mainMenu);
                    break;
                case "required":
                    RemoveAllChildren();
                    reqMenu.Activate();
                    Append(new HologramOutline()); //Append the hologram outline to help show where the hologram will be placed
                    Append(reqMenu);
                    break;
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Update the panel position if the player is dragging it
            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            base.Update(gameTime);
        }
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            //Start dragging the panel if the title bar is clicked
            if (evt.MousePosition.Y < GetDimensions().Y + 36)
            {
                offset = new Vector2(evt.MousePosition.X - GetDimensions().X, evt.MousePosition.Y - GetDimensions().Y);
                dragging = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);

            //Disable dragging and set the position to where it ended
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

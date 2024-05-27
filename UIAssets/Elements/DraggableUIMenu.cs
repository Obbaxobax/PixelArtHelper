using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Terraria.GameInput;
using Microsoft.Xna.Framework.Input;
using System.Net.Http;
using System.IO;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Image = System.Drawing.Image;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using ReLogic.OS;
using System.Threading.Tasks;
using Microsoft.Build.Tasks;
using ClientSideTest;

namespace ClientSideTest.UIAssets
{
    public class DraggableUIMenu : UIElement
    {
        private bool dragging = false;
        private Vector2 offset;
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
            if (evt.MousePosition.Y < GetDimensions().Y + 12)
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

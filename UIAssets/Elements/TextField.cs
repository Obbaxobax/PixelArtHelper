using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using System;

namespace ClientSideTest.UIAssets
{
    //Much of this is based on/taken from UltraSonic, so credit to OliHeamon
    public class TextField : UIElement
    {
        private bool typing = false;
        public string currentValue { get; set; } = ""; //The text that is currently written
        public string placeholderText = ""; //Text displayed if nothing written
        public string hoverText = "";
        public int maxChar = 24; //The maximum number of characters allowed to display at once

        private int position = 0; //position in the string
        private uint prevTime = 0; //Previous time the string was moved through (Used to slow down right and left arrow keys). Probably a better way.
        private int hoverTextColor;


        public override void OnInitialize()
        {
            OverflowHidden = true;

            base.OnInitialize();
        }
        public override void LeftClick(UIMouseEvent evt)
        {
            //Block input and start typing if box clicked on
            typing = true;

            Main.blockInput = true;
            position = currentValue.Length;

            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Recalculate dimensions, turn them to a rectangle, and draw the input box
            Recalculate();

            Rectangle rect = GetDimensions().ToRectangle();

            //Draw box epicly
            UITools.DrawBoxWith(spriteBatch, ModContent.Request<Texture2D>("ClientSideTest/Assets/Box").Value, rect, Color.BlueViolet);

            //Cancel typing if escape is clicked
            if (Main.keyState.IsKeyDown(Keys.Escape))
            {
                typing = false;

                PlayerInput.WritingText = false;
                Main.blockInput = false;
            }

            Vector2 pos = GetDimensions().Position() + Vector2.One * 8 + Vector2.UnitY * 4; //Text offset

            //Decide what to display
            if (typing == true)
            {
                //1 check if the arrow key is pressed to move cursor position in string
                if (Main.keyState.IsKeyDown(Keys.Left) && Main.GameUpdateCount - prevTime > 5)
                {
                    position = position > 0 ? position - 1 : position;
                    prevTime = Main.GameUpdateCount;
                }
                else if (Main.keyState.IsKeyDown(Keys.Right) && Main.GameUpdateCount - prevTime > 5)
                {
                    position = position < currentValue.Length ? position + 1 : position;
                    prevTime = Main.GameUpdateCount;
                }

                //toggle text input
                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                //Split the current text based on the cursor position
                string currentValueSubText = currentValue.Substring(position);
                currentValue = currentValue.Remove(position);

                //Check if there is new text
                string newText = Main.GetInputText(currentValue);
                if (newText != currentValue)
                {
                    //If the cursor is in the middle of the string, adjust position to match new text
                    //Else set the position to the end of the string
                    if ((currentValue + currentValueSubText).Length > (newText + currentValueSubText).Length)
                    {
                        position = position > 0 ? position - 1 : position;
                    }
                    else
                    {
                        position = newText.Length;
                    }

                    //update the current value
                    currentValue = newText;
                }

                //Create a string to display
                string displayed = currentValue ?? "";

                //Add a blinker by counting the update count
                if (Main.GameUpdateCount % 60 < 30)
                {
                    displayed += "|";
                }
                else
                {
                    displayed += " ";
                }

                //Epic math for preventing string overflow
                var lengthOfStr = (currentValue.Length + currentValueSubText.Length);
                if(lengthOfStr > maxChar && position > maxChar / 2)
                {
                    displayed = displayed.Substring(Math.Clamp(-(maxChar/2) + position, 0, lengthOfStr - (maxChar / 2)));
                    displayed += currentValueSubText.Insert(Math.Clamp(lengthOfStr - position, 0, (maxChar / 2)), "\n").Split("\n")[0];
                }
                else
                {
                    displayed += currentValueSubText.Insert(Math.Clamp(lengthOfStr - position, 0, maxChar - position), "\n").Split("\n")[0];
                }


                //2 display the current text with a blinker
                Utils.DrawBorderString(spriteBatch, displayed, pos, Color.LightPink, 1.2f, maxCharactersDisplayed: 15);
                currentValue = currentValue + currentValueSubText;
            }
            else if (currentValue != "")
            {
                //3 display only the current text
                Utils.DrawBorderString(spriteBatch, currentValue.Substring(Math.Clamp(currentValue.Length - maxChar, 0, currentValue.Length)), pos, Color.LightPink, 1.2f, maxCharactersDisplayed: 15);
            }
            else
            {
                //4 display the placeholder
                Utils.DrawBorderString(spriteBatch, placeholderText, pos, Color.Lerp(Color.LightPink, Color.Gray, 0.7f), 1.2f, maxCharactersDisplayed: 15);
            }

            //Display hover text if the mouse is hovering
            if (IsMouseHovering)
            {
                hoverTextColor = PixelArtHelper.hoverTextColor;
                Main.instance.MouseText(hoverText, rare:hoverTextColor);
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            //Stop typing when box is clicked out of
            if (Main.mouseLeft && !IsMouseHovering)
            {
                typing = false;

                PlayerInput.WritingText = false;
                Main.blockInput = false;
            }

            base.Update(gameTime);
        }
    }
}

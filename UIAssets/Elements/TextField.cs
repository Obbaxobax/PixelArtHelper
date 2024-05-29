using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace ClientSideTest.UIAssets
{
    public class TextField : UIElement
    {
        private bool typing = false;
        public string currentValue { get; set; } = "";
        public string placeholderText = ""; //Text displayed if nothing written
        public string hoverText = "";

        private int position = 0;
        private uint prevTime = 0;

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

            UITools.DrawBoxWith(spriteBatch, (Texture2D)ModContent.Request<Texture2D>("ClientSideTest/Assets/Box"), rect, Color.BlueViolet);

            //Cancel typing if escape is clicked
            if (Main.keyState.IsKeyDown(Keys.Escape))
            {
                typing = false;

                PlayerInput.WritingText = false;
                Main.blockInput = false;
            }

            Vector2 pos = GetDimensions().Position() + Vector2.One * 8 + Vector2.UnitY * 4; //Text offset

            //Decide what to display
            //0 check if the arrow key is pressed to move cursor position in string
            //1 display the current text with a blinker
            //2 display only the current text
            //3 display the placeholder
            if (typing == true)
            {
                if (Main.keyState.IsKeyDown(Keys.Left) && Main.GameUpdateCount - prevTime > 10)
                {
                    position = position > 0 ? position - 1 : position;
                    prevTime = Main.GameUpdateCount;
                }
                else if (Main.keyState.IsKeyDown(Keys.Right) && Main.GameUpdateCount - prevTime > 10)
                {
                    position = position < currentValue.Length ? position + 1 : position;
                    prevTime = Main.GameUpdateCount;
                }

                PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                string currentValueSubText = currentValue.Substring(position);
                currentValue = currentValue.Remove(position);


                string newText = Main.GetInputText(currentValue);
                if (newText != currentValue)
                {
                    if ((currentValue + currentValueSubText).Length > (newText + currentValueSubText).Length)
                    {
                        position = position > 0 ? position - 1 : position;
                    }
                    else
                    {
                        position = newText.Length;
                    }

                    currentValue = newText;
                }
                string displayed = currentValue ?? "";

                if (Main.GameUpdateCount % 60 < 30)
                {
                    displayed += "|";
                }
                else
                {
                    displayed += " ";
                }

                Utils.DrawBorderString(spriteBatch, displayed + currentValueSubText, pos, Color.LightPink, 1.2f, maxCharactersDisplayed: 20);
                currentValue = currentValue + currentValueSubText;
            }
            else if (currentValue != "")
            {
                Utils.DrawBorderString(spriteBatch, currentValue, pos, Color.LightPink, 1.2f);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, placeholderText, pos, Color.Lerp(Color.LightPink, Color.Gray, 0.7f), 1.2f);
            }

            if (IsMouseHovering)
            {
                Main.instance.MouseText(hoverText);
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

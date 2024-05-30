using Microsoft.Xna.Framework.Graphics;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Base class for a button in a list
    public class ListElementButton : Button
    {
        private List parent; //Parent list
        private int i; //Index of the button
        private ListElement le; //Associated list element

        public ListElementButton(List parent, int i, ListElement le)
        {
            this.parent = parent;
            this.i = i;
            this.le = le;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Check if our parent and index are set
            if (parent != null && i != -1)
            {
                //Set our height based on scroll position
                Top.Set(i * (le.parent.elementHeight + 5) + parent.scrollPos, 0);
            }

            base.Draw(spriteBatch);
        }
    }
}

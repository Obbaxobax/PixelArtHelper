using ClientSideTest.DataClasses;
using ClientSideTest.UIAssets.Elements.Buttons;
using System.Collections.Generic;

namespace ClientSideTest.UIAssets.Elements.Lists
{
    //List class for the exceptions page
    public class ExceptionsList : List
    {
        public List<Tile> elements; //The names and ids either the blocks or walls
        private Exceptions exList; //The class for the list of exceptions

        public ExceptionsList(Exceptions exList)
        {
            this.exList = exList;
        }

        public override void OnInitialize()
        {
            elementHeight = 50f; //Make each element taller
            elementPerRow = 2f;
            scrollSpeed = 50;

            //Create a row for each tile/wall
            for (int i = 0; i < elements.Count; i++)
            {
                //Create a box with the tile/wall name
                ListElement ele = new ListElement(i, elements[i].Name, this);
                ele.Width.Set(295f, 0);
                ele.Height.Set(50f, 0);
                ele.stringOffset = 8f;
                Append(ele);

                //Create a button to toggle usage
                ExceptionsListButton butt = new ExceptionsListButton(this, i, ele, elements, exList);
                butt.Width.Set(50f, 0);
                butt.Height.Set(50f, 0);
                butt.Left.Set(295f, 0);
                butt.texture = exList.exceptionsDict[elements[i].Name] ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";

                Append(butt);
            }

            base.OnInitialize();
        }
    }
}

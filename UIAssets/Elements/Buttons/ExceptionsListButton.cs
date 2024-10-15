using System.Collections.Generic;
using ClientSideTest.UIAssets.Menus;
using Terraria;
using Terraria.UI;
using Tile = ClientSideTest.DataClasses.Tile;

namespace ClientSideTest.UIAssets.Elements.Buttons
{
    //Button to toggle exceptions in the exception list
    public class ExceptionsListButton : ListElementButton
    {
        private int i; //Index of the button in hte list
        private List<Tile> elements; //The list of tiles
        private Exceptions exList; //The list of exceptions

        public ExceptionsListButton(List parent, int i, ListElement le, List<Tile> elements, Exceptions exList) : base(parent, i, le)
        {
            this.i = i;
            this.elements = elements;
            this.exList = exList;
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            //Swaps the value of the exception
            exList.exceptionsDict[elements[i].Name] = !exList.exceptionsDict[elements[i].Name];

            //Changes the button texture to match the state
            texture = exList.exceptionsDict[elements[i].Name] ? "ClientSideTest/Assets/activeButton" : "ClientSideTest/Assets/deleteButton";

            base.LeftClick(evt);
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace ClientSideTest.UIAssets.Elements.Lists
{
    //Class for the required items list
    public class RequiredList : List
    {
        public Dictionary<string, int> list = new Dictionary<string, int>(); //An empty dictionary to store the blocks that will be required

        public override void OnInitialize()
        {
            //order the dictionary in descending order by amount required
            list = list.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            //create a list element for each required block
            for (int i = 0; i < list.Count; i++)
            {
                //Generate a display name which shows "*name*: *amount*"
                string displayString = $"{list.ElementAt(i).Key}: {list.ElementAt(i).Value}";
                ListElement le = new ListElement(i, displayString, this);
                le.Height.Set(30f, 0);
                le.Width.Set(345f, 0);

                Append(le);
            }

            base.OnInitialize();
        }
    }
}

using Microsoft.Xna.Framework;

namespace ClientSideTest.DataClasses
{
    //Class for storing data of each pixel
    public class Pixel
    {
        public Color color { get; set; }
        public int id { get; set; }
        public string paintId { get; set; }
        public string name { get; set; }
        public Vector2 position { get; set; }
        public bool wall { get; set; }

    }
}

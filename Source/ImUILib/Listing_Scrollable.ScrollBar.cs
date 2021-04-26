using UnityEngine;

namespace ImUILib
{
    public partial class Listing_Scrollable
    {
        public class ScrollBar
        {
            public CoordinateY curY;
            public Rect rect;
            public Vector2 scrollPos;

            public ScrollBar(CoordinateY curY, Rect rect, Vector2 scrollPos)
            {
                this.curY = curY;
                this.rect = rect;
                this.scrollPos = scrollPos;
            }

            public float MaxWidth => rect.width - ScrollWidth;
        }
    }
}
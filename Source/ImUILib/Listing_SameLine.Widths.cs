using System.Collections.Generic;

namespace ImUILib
{
    public partial class Listing_SameLine
    {
        public class Widths
        {
            private readonly Queue<float> _widths = new();

            public void Enqueue(params float[] widths)
            {
                foreach (var width in widths)
                {
                    _widths.Enqueue(width);
                }
            }

            public float Dequeue()
            {
                return _widths.Dequeue();
            }

            public void Clear()
            {
                _widths.Clear();
            }

            public float Peek()
            {
                return _widths.Peek();
            }

            public int Count => _widths.Count;

            public bool HasElements => Count > 0;

            public bool IsEmpty => Count == 0;
        }
    }
}
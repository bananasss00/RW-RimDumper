namespace ImUILib
{
    public partial class Listing
    {
        public class Coordinate
        {
            public float Value { get; set; }

            public static implicit operator float(Coordinate o)
            {
                return o.Value;
            }

            public Coordinate(float value)
            {
                Value = value;
            }
        }

        public class CoordinateX : Coordinate
        {
            private readonly float _width;

            public CoordinateX(float x, float width) : base(x)
            {
                _width = width;
            }

            public void SetFromLeftPercent(float percent)
            {
                Value = _width * percent;
            }

            public void SetFromRightPercent(float percent)
            {
                Value = _width * (1f - percent);
            }

            public void SetFromLeft(float shift)
            {
                Value = shift;
            }

            public void SetFromRight(float shift)
            {
                Value = _width - shift;
            }
        }

        public class CoordinateY : Coordinate
        {
            private readonly float _height;

            public CoordinateY(float y, float height) : base(y)
            {
                _height = height;
            }

            public void SetFromTopPercent(float percent)
            {
                Value = _height * percent;
            }

            public void SetFromBottomPercent(float percent)
            {
                Value = _height * (1f - percent);
            }

            public void SetFromTop(float shift)
            {
                Value = shift;
            }

            public void SetFromBottom(float shift)
            {
                Value = _height - shift;
            }
        }
    }
}
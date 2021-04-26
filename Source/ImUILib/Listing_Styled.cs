using UnityEngine;
using Verse;

namespace ImUILib
{
    /// <summary>
    /// Listing with pushable styles
    /// </summary>
    public partial class Listing_Styled : Listing_Scrollable
    {
        public StackRef<float> ElementHeight { get; } = new();
        public StackRef<float> ElementWidth { get; } = new();

        public StackFont Font { get; } = new();
        public StackColor Color { get; } = new();

        private void RestoreStyle()
        {
            _ = Font.Pop(Font.Count);
            _ = Color.Pop(Color.Count);
        }

        private void Clear()
        {
            ElementWidth.Clear();
            ElementHeight.Clear();
            RestoreStyle();
        }

        public override void Begin(Rect canvas, float elementWidth = -1f, float elementHeight = 25f, float gapSize = 5f)
        {
            Clear();
            Font.Push(Text.Font); // backup current font
            Color.Push(GUI.color); // backup current color
            base.Begin(canvas, elementWidth, elementHeight, gapSize);
        }

        /// <summary>
        /// Clear all global UI changes and finalize GUI group
        /// </summary>
        public override void End()
        {
            RestoreStyle();
            base.End();
        }

        public override float CurElementWidth
        {
            get => ElementWidth.HasElements && _sameLineWidths.IsEmpty ? ClampWidth(ElementWidth.Peek()) : base.CurElementWidth;
            set
            {
                base.CurElementWidth = value;
                if (ElementWidth.HasElements)
                {
                    ElementWidth.Set(base.CurElementWidth);
                }
            }
        }

        public override float CurElementHeight
        {
            get => ElementHeight.HasElements ? ElementHeight.Peek() : base.CurElementHeight;
            set
            {
                base.CurElementHeight = value;
                if (ElementHeight.HasElements)
                {
                    ElementHeight.Set(base.CurElementHeight);
                }
            }
        }

        #region Stack style
        public void PushStyle(GameFont font, Color color)
        {
            Font.Push(font);
            Color.Push(color);
        }

        public void PopStyle(int count = 1)
        {
            _ = Font.Pop(count);
            _ = Color.Pop(count);
        }
        #endregion

        #region Stack size
        public void PushElementSize(float width, float height)
        {
            ElementWidth.Push(width);
            ElementHeight.Push(height);
        }

        public void PopElementSize(int count = 1)
        {
            _ = ElementWidth.Pop(count);
            _ = ElementHeight.Pop(count);
        }
        #endregion
    }
}
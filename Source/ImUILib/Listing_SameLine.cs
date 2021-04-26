using System.Linq;
using UnityEngine;

namespace ImUILib
{
    /// <summary>
    /// Listing with horizontal drawing feature
    /// </summary>
    public partial class Listing_SameLine : Listing
    {
        protected float _savedX;
        protected readonly Widths _sameLineWidths = new();

        private void BackupX()
        {
            _savedX = CurX;
        }

        private void RestoreX()
        {
            CurX.Value = _savedX;
        }

        public virtual void SameLine(params float[] widths)
        {
            BackupX();
            ClampWidthIfNeeded(widths);
            _sameLineWidths.Enqueue(widths);
        }

        public virtual void SameLinePercent(params float[] percents)
        {
            var widths = percents.Select(p => CurElementWidth * p);
            SameLine(widths.ToArray());
        }

        public override void Begin(Rect canvas, float elementWidth = -1f, float elementHeight = 25f, float gapSize = 5f)
        {
            _sameLineWidths.Clear();
            base.Begin(canvas, elementWidth, elementHeight, gapSize);
        }

        public override float CurElementWidth => _sameLineWidths.HasElements ? _sameLineWidths.Peek() : base.CurElementWidth;

        public override void NewLine(float height = -1)
        {
            if (_sameLineWidths.IsEmpty)
            {
                base.NewLine(height);
                return;
            }
            Dequeue(height);
        }

        private void Dequeue(float height)
        {
            CurX.Value += CurElementWidth;

            _ = _sameLineWidths.Dequeue();

            if (_sameLineWidths.IsEmpty)
            {
                RestoreX();
                base.NewLine(height);
            }
        }

        protected void ClampWidthIfNeeded(float[] widths)
        {
            float fullLineWidth = widths.Sum();
            if (fullLineWidth > MaxWidthFromX)
            {
                float overlapsedWidth = fullLineWidth - MaxWidthFromX;
                float clampPerElement = overlapsedWidth / widths.Length;
                SubstractValueInArray(widths, clampPerElement);
            }
        }

        private static void SubstractValueInArray(float[] widths, float clampPerElement)
        {
            for (int i = 0; i < widths.Length; i++)
            {
                widths[i] -= clampPerElement;
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ImUILib
{
    /// <summary>
    /// Listing with scrollbars
    /// </summary>
    public partial class Listing_Scrollable : Listing_SameLine
    {
        public const float ScrollWidth = 16f;
        protected static Dictionary<string, ScrollInfo> _scrollInfo = new();
        protected readonly Stack<ScrollBar> _scrollbarContext = new();

        public virtual bool IsScrollContext => _scrollbarContext.HasElements;
        public override float MaxWidthFromX => IsScrollContext ? _scrollbarContext.Peek().MaxWidth - CurX : base.MaxWidthFromX;

        public override bool ShouldDraw
        {
            get
            {
                if (_scrollbarContext.IsEmpty)
                {
                    return true;
                }

                float viewPosStart = _scrollbarContext.Peek().scrollPos.y;
                float viewPosEnd = viewPosStart + _scrollbarContext.Peek().rect.height;
                //Log.Warning($"CurY: {CurY}; scrollPos:{_scrollbarContext.Peek().scrollPos.y} = {CurY >= viewPosStart - CurElementHeight && CurY <= viewPosEnd}");
                return CurY >= viewPosStart - CurElementHeight && CurY <= viewPosEnd;
            }
        }

        /// <summary>
        /// Y adjusted for scrollbar rect's
        /// </summary>
        /// <value></value>
        public override CoordinateY CurY
        {
            get => _scrollbarContext.IsEmpty ? base.CurY : _scrollbarContext.Peek().curY;
            protected set
            {
                if (_scrollbarContext.IsEmpty)
                {
                    base.CurY = value;
                    return;
                }
                _scrollbarContext.Peek().curY = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="width">use canvas size if value < 0</param>
        /// <param name="height">use canvas size if value < 0</param>
        public virtual void ScrollStart(string uniqueId, float width = -1f, float height = -1f)
        {
            if (width < 0)
            {
                width = _canvas.width;
            }
            if (height < 0)
            {
                height = _canvas.height - CurY - GapSize;
            }
            width = ClampWidth(width);
            Rect rect = new(CurX, CurY, width, height);
            ScrollInfo info = GetScrollInfo(uniqueId);
            ScrollBar context = new(new CoordinateY(0f, rect.height), rect, info.scrollPos);

            CurY.Value += height; // add to real Y scrollbar height
            _scrollbarContext.Push(context); // add new scrollbar info, now Y zero!
            Rect viewRect = new(0, 0, width - ScrollWidth, info.viewHeight);
            Widgets.BeginScrollView(rect, ref info.scrollPos, viewRect);
        }

        public virtual void ScrollEnd(string uniqueId)
        {
            if (Event.current.type == EventType.Layout)
            {
                ScrollInfo info = GetScrollInfo(uniqueId);
                info.viewHeight = CurY;
            }
            _ = _scrollbarContext.Pop();
            Widgets.EndScrollView();
        }

        public override void Begin(Rect canvas, float elementWidth = -1f, float elementHeight = 25f, float gapSize = 5f)
        {
            _scrollbarContext.Clear();
            base.Begin(canvas, elementWidth, elementHeight, gapSize);
        }

        private static ScrollInfo GetScrollInfo(string uniqueId)
        {
            if (!_scrollInfo.TryGetValue(uniqueId, out var info))
            {
                info = new ScrollInfo();
                _scrollInfo.Add(uniqueId, info);
            }
            return info;
        }
    }
}
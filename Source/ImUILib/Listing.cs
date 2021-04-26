using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ImUILib
{
    /// <summary>
    /// Simple Listing
    /// </summary>
    public partial class Listing
    {
        protected Rect _canvas;
        private float curElementWidth;

        public virtual float GapSize { get; set; }

        public virtual CoordinateX CurX { get; protected set; } = new(0, 0);

        public virtual CoordinateY CurY { get; protected set; } = new(0, 0);

        public virtual float CurElementWidth
        {
            get => ClampWidth(curElementWidth);
            set => curElementWidth = ClampWidth(value);
        }

        public virtual float CurElementHeight { get; set; }

        public virtual float MaxWidthFromX => _canvas.width - CurX;

        public virtual bool ShouldDraw => true;

        protected float ClampWidth(float width)
        {
            if (width > MaxWidthFromX)
            {
                width = MaxWidthFromX;
            }

            return width;
        }

        public virtual void Begin(Rect canvas, float elementWidth = -1f, float elementHeight = 25f, float gapSize = 5f)
        {
            CurX = new(0, canvas.width);
            CurY = new(0, canvas.height);
            _canvas = canvas;
            GapSize = gapSize;
            CurElementWidth = elementWidth < 0f ? _canvas.width : elementWidth;
            CurElementHeight = elementHeight;
            GUI.BeginGroup(canvas);
        }

        public virtual void End()
        {
            GUI.EndGroup();
        }

        public virtual void NewLine(float height = -1f)
        {
            CurY.Value += (height < 0 ? CurElementHeight : height) + GapSize;
        }

        public virtual void Gap(float gapHeight = -1f)
        {
            CurY.Value += gapHeight < 0f ? GapSize : gapHeight;
        }

        public virtual void Label(string text)
        {
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            if (ShouldDraw)
            {
                Widgets.Label(rect, text);
            }

            NewLine();
        }

        public virtual bool ButtonText(string text)
        {
            bool result = false;
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            if (ShouldDraw)
            {
                result = Widgets.ButtonText(rect, text);
            }

            NewLine();
            return result;
        }

        public virtual bool ButtonImage(Texture2D tex)
        {
            bool result = false;
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            if (ShouldDraw)
                result = Widgets.ButtonImage(rect, tex, true);
            NewLine();
            return result;
        }

        public virtual bool RadioButton(string label, bool active, float tabIn = 0f, string? tooltip = null, float? tooltipDelay = null)
		{
            bool result = false;

			Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
			rect.xMin += tabIn;
			if (!tooltip.NullOrEmpty())
			{
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				TipSignal tip = (tooltipDelay != null) ? new TipSignal(tooltip, tooltipDelay.Value) : new TipSignal(tooltip);
				TooltipHandler.TipRegion(rect, tip);
			}
            if (ShouldDraw)
			    result = Widgets.RadioButtonLabeled(rect, label, active);
			NewLine();
			return result;
		}

        // TODO: GUI.Label(elementRect, label, HeaderStyle);
        //  ! GUIStyle HeaderStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.BoldAndItalic, alignment = TextAnchor.MiddleLeft};

        public virtual string TextArea(string text)
        {
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            if (ShouldDraw)
            {
                text = Widgets.TextArea(rect, text);
            }

            NewLine();
            return text;
        }

        public virtual bool TextFieldNumericLabeled<T>(string label, ref T val, ref string buffer, float min = 0f, float max = float.MaxValue) where T : struct
        {
            T prevValue = val;
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            if (ShouldDraw)
            {
                Widgets.TextFieldNumericLabeled(rect, label, ref val, ref buffer, min, max);
            }

            NewLine();
            return !val.Equals(prevValue);
        }

        public virtual bool CheckboxLabeled(string text, ref bool value)
        {
            bool prevValue = value;
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            if (ShouldDraw)
            {
                Widgets.CheckboxLabeled(rect, text, ref value);
            }

            NewLine();
            return value != prevValue;
        }

        public void GapLine(float gapHeight = 12f)
        {
            float y = CurY + (gapHeight / 2f);
            Color color = GUI.color;
            GUI.color = color * new Color(1f, 1f, 1f, 0.4f);
            if (ShouldDraw)
            {
                Widgets.DrawLineHorizontal(CurX, y, CurElementWidth);
            }

            GUI.color = color;
            NewLine(gapHeight);
        }

        public bool DubsCheckbox(string text, ref bool value)
        {
            Rect rect = new(CurX, CurY, CurElementWidth, CurElementHeight);
            Widgets.DrawHighlightIfMouseover(rect);
            bool prevValue = value;
            if (Widgets.ButtonInvisible(rect, true))
            {
                value = !value;
                if (value)
                {
                    SoundStarter.PlayOneShotOnCamera(SoundDefOf.Checkbox_TurnedOn);
                }
                else
                {
                    SoundStarter.PlayOneShotOnCamera(SoundDefOf.Checkbox_TurnedOff);
                }
            }
            TextAnchor anchor = Text.Anchor;
            GameFont font = Text.Font;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.Font = GameFont.Medium;
            float lineHeight = Text.LineHeight;
            Text.Font = font;
            Rect textureRect = GenUI.LeftPartPixels(rect, 30f);
            float height = rect.height;
            if (height > lineHeight)
            {
                textureRect = GenUI.TopPartPixels(textureRect, lineHeight);
                textureRect.y += (height - lineHeight) / 2f;
            }
            Widgets.DrawTextureFitted(textureRect, value ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex, 0.5f);
            rect.x += 30f;
            rect.width -= 30f;
            Widgets.Label(rect, text);
            Text.Font = font;
            Text.Anchor = anchor;
            NewLine();
            return value != prevValue;
        }
    }
}
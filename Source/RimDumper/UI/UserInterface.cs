//#define DEBUG

//using HarmonyLib;
using UnityEngine;
using RimWorld;
using RimDumper.UI.Pages;
using ImUILib;
using Verse;

namespace RimDumper.UI
{
    public class UserInterface : MainTabWindow
    {
        public const float ElementHeight = 25f;
        public const float GapSize = 5f;

        public static string CurrentPage = "";

        private readonly ImUILib.Pages _pages = new();
        public UserInterface()
        {
            doCloseX = true;
            _ = _pages.Open<MainPage>();
        }

        public override Vector2 RequestedTabSize => new(350f, 600f);

        public override void DoWindowContents(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            Rect header = canvas.TopPart(0.05f);
            Rect body = canvas.BottomPart(0.95f); // 2 buttons

            Listing_Styled imui = new();
            imui.Begin(header, elementHeight: 30f, gapSize: GapSize);
            imui.PushStyle(GameFont.Medium, Color.yellow);
            imui.Label(CurrentPage);
            imui.PopStyle();
            imui.End();

            Widgets.DrawMenuSection(body);
            body = GenUI.ContractedBy(body, 5f);
            _pages.Draw(body);

            GUI.EndGroup();
        }

        public override void PreOpen()
        {
            base.PreOpen();
            forcePause = true;
        }
    }
}
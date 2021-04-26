//#define DEBUG

//using HarmonyLib;
using UnityEngine;
using Verse;
using ImUILib;
using RimDumper.Extensions;

namespace RimDumper.UI.Pages
{
    public class TestPage : Page
    {
        public TestPage(ImUILib.Pages pages) : base(pages)
        {
        }

        private string str = "";
        private int layer;
        public override void Draw(Rect rect)
        {
            UserInterface.CurrentPage = "Test page";

            Rect rectBottom = rect.BottomPart(0.2f);

            Listing_Styled imUi = new();
            imUi.Begin(rect, 250);
            _ = imUi.TextArea("3213123");
            imUi.ElementWidth.Push(10);
            imUi.SameLine(70, 70, 50);
            if (imUi.ButtonText("AllowAll"))
            {

            }
            if (imUi.ButtonText("DisableAll"))
            {

            }
            if (imUi.ButtonText("Reset"))
            {

            }
            _ = imUi.ElementWidth.Pop();

            imUi.ScrollStart("UI.Scroll1", 300, 150);
            for (int i = 0; i < 20; i++)
            {
                //imUi.ElementWidth.Push(900);
                imUi.CurElementWidth = 900;
                _ = imUi.ButtonText("FIRST");
                //imUi.ElementWidth.Pop();

                imUi.ScrollStart("UI.Scroll4", 300, 50);
                imUi.CurX.Value = 30;
                //imUi.ElementWidth.Push(900);
                //imUi.CurElementWidth = 900;
                for (int j = 0; j < 10; j++)
                {
                    _ = imUi.ButtonText("Scroll4" + j);

                    imUi.ScrollStart("UI.Scroll5", 300, 50);
                    imUi.CurX.Value = 0;
                    imUi.ElementWidth.Push(900);
                    //imUi.CurElementWidth = 900;
                    for (int k = 0; k < 5; k++)
                    {
                        _ = imUi.ButtonText("Scroll5" + k);
                    }
                    imUi.ScrollEnd("UI.Scroll5");
                    imUi.CurX.Value = 30;
                }
                imUi.CurX.Value = 0;
                imUi.ScrollEnd("UI.Scroll4");

                imUi.ScrollStart("UI.Scroll2", 250, 50);
                //imUi.CurX = 0;
                //imUi.ElementWidth.Push(900);
                //imUi.CurElementWidth = 900;
                for (int j = 0; j < 100; j++)
                {
                    _ = imUi.ButtonText("Scroll2" + j);
                }
                imUi.ScrollEnd("UI.Scroll2");

                imUi.CurX.Value = 0;
                bool d = false;
                if (imUi.DubsCheckbox("slals" + i, ref d))
                {
                    Log.Error($"{i} new value = {d}");
                }
            }
            imUi.ScrollEnd("UI.Scroll1");

            imUi.ScrollStart("UI.Scroll3", 300, 150);
            for (int j = 0; j < 30; j++)
            {
                imUi.Label("asa" + j);
            }
            imUi.ScrollEnd("UI.Scroll3");
            imUi.Color.Push(Color.blue);
            imUi.Font.Push(GameFont.Tiny);
            imUi.End();


            imUi.Begin(rectBottom, 300);
            imUi.Label("Selector END!!");
            if (imUi.TextFieldNumericLabeled("CountPerLayer".UiTranslate(), ref layer, ref str, 1f))
            {
                Log.Error("Changed to " + layer);
            }
            imUi.End();
        }
    }
}
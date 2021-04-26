//#define DEBUG

using System;
using System.Linq;
//using HarmonyLib;
using AutoTable;
using ImUILib;
using RimDumper.Extensions;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimDumper.UI.Pages
{
    public class DumpSettingsPage : ImUILib.Page
    {
        public DumpSettingsPage(ImUILib.Pages pages) : base(pages)
        {
        }

        public override void Draw(Rect canvas)
        {
            UserInterface.CurrentPage = "SelectColumnsPage".UiTranslate();

            // Page header
            const float splitCanvasHight = 35f;
            Rect top = canvas.TopPartPixels(canvas.height - splitCanvasHight); ;
            Rect bottom = canvas.BottomPartPixels(splitCanvasHight); // 1 buttons

            Listing_Styled imui = new();
            imui.Begin(top, elementHeight: UserInterface.ElementHeight, gapSize: UserInterface.GapSize);

			imui.Font.Push(GameFont.Small);
			bool value = Settings.ColorizeValues;
            imui.DubsCheckbox("ColorizeValues".UiTranslate(), ref value);
            if (value != Settings.ColorizeValues)
            {
                Settings.ColorizeValues = value;
            }
			
			imui.GapLine();

            imui.Font.Push(GameFont.Small);
            imui.Label("SaveAction".UiTranslate());

			imui.Font.Push(GameFont.Tiny);
            if (imui.RadioButton("SaveAction_Nothing".UiTranslate(), Settings.SaveAndOpenAction == SaveAndOpenAction.Nothing))
            {
                Settings.SaveAndOpenAction = SaveAndOpenAction.Nothing;
            }
            if (imui.RadioButton("SaveAction_OpenDirectory".UiTranslate(), Settings.SaveAndOpenAction == SaveAndOpenAction.OpenDirectory))
            {
                Settings.SaveAndOpenAction = SaveAndOpenAction.OpenDirectory;
            }
            if (imui.RadioButton("SaveAction_OpenDocument".UiTranslate(), Settings.SaveAndOpenAction == SaveAndOpenAction.OpenDocument))
            {
                Settings.SaveAndOpenAction = SaveAndOpenAction.OpenDocument;
            }
			imui.Font.Pop(3);

            imui.End();

            // Back on bottom
            imui.Begin(bottom, elementHeight: 25f);
            if (imui.ButtonText("Previous".UiTranslate()))
            {
                Close();
                Settings.WriteSettings();
            }

            imui.End();
        }
    }
}
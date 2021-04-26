//#define DEBUG

using System;
using Verse;
//using HarmonyLib;
using UnityEngine;
using System.Reflection;
using System.Linq;
using RimDumper.Extensions;
using ImUILib;

namespace RimDumper.UI.Pages
{
    public class MainPage : Page
    {
        public MainPage(ImUILib.Pages pages) : base(pages)
        {
        }

        public override void Draw(Rect canvas)
        {
#pragma warning disable CS0103 // not error. compile time type generation
            var compileTime = new DateTime(Builtin.CompileTime, DateTimeKind.Utc);
#pragma warning restore CS0103
            var version = ((AssemblyFileVersionAttribute)Attribute.GetCustomAttribute(
                Assembly.GetExecutingAssembly(),
                typeof(AssemblyFileVersionAttribute),
                false)).Version;

            UserInterface.CurrentPage = "Title".UiTranslate();

            const float splitCanvasHight = 60f;
            Rect top = canvas.TopPartPixels(canvas.height - splitCanvasHight); ;
            Rect bottom = canvas.BottomPartPixels(splitCanvasHight); // 2 buttons

            Listing_Styled imui = new();
            imui.Begin(top, elementHeight: UserInterface.ElementHeight, gapSize: UserInterface.GapSize);

            imui.PushStyle(GameFont.Small, Color.gray);
            imui.Label("Version".UiTranslate(version, compileTime.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss")));
            imui.PopStyle();
            //if (imui.ButtonText("TEST")) OpenPage<TestPage>();

            // Buttons all on/off
            imui.SameLinePercent(0.5f, 0.5f);
            if (imui.ButtonText("EnableAll".UiTranslate()))
            {
                foreach (var parser in TableManager.Parsers.Keys.ToList())
                {
                    TableManager.Parsers[parser] = true;
                }
            }
            if (imui.ButtonText("DisableAll".UiTranslate()))
            {
                foreach (var parser in TableManager.Parsers.Keys.ToList())
                {
                    TableManager.Parsers[parser] = false;
                }
            }

            // Parsers
            imui.ScrollStart("MainPage.Parsers");
            foreach (var parser in TableManager.Parsers.Keys.ToList())
            {
                bool value = TableManager.Parsers[parser];
                if (imui.DubsCheckbox(parser.Name, ref value))
                {
                    TableManager.Parsers[parser] = value;
                }
            }
            imui.ScrollEnd("MainPage.Parsers");
            imui.End();

            imui.Begin(bottom, elementHeight: 25f);

            // imui.GapLine();

            if (imui.ButtonText("Dump".UiTranslate()))
            {
                var waitingPage = Open<WaitingPage>();
                TableManager.Update(() =>
                {
                    waitingPage.Close();
                    if (TableManager.Tables.Count > 0)
                    {
                        _ = Open<SelectColumnsPage>();
                    }
                });
            }

            if (TableManager.Tables.Count > 0 && imui.ButtonText("Next".UiTranslate()))
            {
                _ = Open<SelectColumnsPage>();
            }
            imui.End();
        }
    }
}
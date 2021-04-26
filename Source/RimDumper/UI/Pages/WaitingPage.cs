//#define DEBUG

using System;
using Verse;
//using HarmonyLib;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using RimDumper.Extensions;
using ImUILib;

namespace RimDumper.UI.Pages
{
    public class WaitingPage : Page
    {
        private readonly Stopwatch _sw = Stopwatch.StartNew();
        private int _dots = 0;

        public WaitingPage(ImUILib.Pages pages) : base(pages)
        {
        }

        public override void Draw(Rect canvas)
        {
            UserInterface.CurrentPage = "WaitingPage".UiTranslate();

            Listing_Styled imui = new();
            imui.Begin(canvas, elementHeight: 60f);
            //imui.Gap(100f);
            imui.PushStyle(GameFont.Medium, Color.white);
            imui.Label("Waiting".UiTranslate(String.Join("", Enumerable.Repeat(".", _dots))));
            imui.End();

            if (_sw.ElapsedMilliseconds > 1000)
            {
                _dots++;
                if (_dots > 5)
                {
                    _dots = 0;
                }

                _sw.Restart();
            }
        }
    }
}
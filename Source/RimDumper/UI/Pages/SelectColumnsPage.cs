//#define DEBUG

using System.Collections.Generic;
using System.Linq;
using ImUILib;
using RimDumper.Extensions;
using RimDumper.Parsers;
//using HarmonyLib;
using UnityEngine;
using Verse;

namespace RimDumper.UI.Pages
{
    [StaticConstructorOnStartup]
    public class SelectColumnsPage : Page
    {
        private static readonly Texture2D AutoRebuildIcon = ContentFinder<Texture2D>.Get("UI/Buttons/AutoRebuild", true);

        public SelectColumnsPage(ImUILib.Pages pages) : base(pages)
        {
        }

        public override void Draw(Rect canvas)
        {
            UserInterface.CurrentPage = "SelectColumnsPage".UiTranslate();

            // Page header
            const float splitCanvasHight = 60f;
            Rect top = canvas.TopPartPixels(canvas.height - splitCanvasHight); ;
            Rect bottom = canvas.BottomPartPixels(splitCanvasHight); // 2 buttons

            Listing_Styled imui = new();
            imui.Begin(top, elementHeight: UserInterface.ElementHeight, gapSize: UserInterface.GapSize);

            // Select columns btns
            imui.ScrollStart("SelectColumnPage.Parsers");
            var apparelParser = ParserStorage.OfType<ApparelParser>().First();
            foreach (var table in TableManager.Tables)
            {
                if (table.Name.Equals(apparelParser.Name))
                {
                    imui.SameLinePercent(0.85f, 0.15f);
                    if (imui.ButtonText(table.Name))
                    {
                        _ = Open<ColumnsPage>(table);
                    }
                    if (imui.ButtonText("!"))
                    {
                        List<FloatMenuOption> options = new()
                        {
                            new FloatMenuOption("ApparelFilterPageTitle".UiTranslate(), delegate ()
                            {
                                _ = Open<ApparelFilterPage>(table);
                            })
                        };
                        Find.WindowStack.Add(new FloatMenu(options));
                    }
                }
                else
                {
                    if (imui.ButtonText(table.Name))
                    {
                        _ = Open<ColumnsPage>(table);
                    }
                }
            }
            imui.ScrollEnd("SelectColumnPage.Parsers");
            imui.End();

            // Bottom panel
            imui.Begin(bottom, elementHeight: 25f);
            imui.SameLine(imui.CurElementWidth - 25f, 25f);
            string btnSaveName = "SaveAndOpen".UiTranslate();
            if (Settings.SaveAndOpenAction != SaveAndOpenAction.Nothing)
            {
                btnSaveName += ". ";
                switch (Settings.SaveAndOpenAction)
                {
                    case SaveAndOpenAction.OpenDirectory:
                        btnSaveName += "SaveAction_OpenDirectory".UiTranslate();
                        break;
                    case SaveAndOpenAction.OpenDocument:
                        btnSaveName += "SaveAction_OpenDocument".UiTranslate();
                        break;
                }
            }
            if (imui.ButtonText(btnSaveName))
            {
                var waitingPage = Open<WaitingPage>();
                TableManager.ExportAsync(() => waitingPage.Close());
            }
            if (imui.ButtonImage(AutoRebuildIcon))
			{
				Open<DumpSettingsPage>();
			}
            
            // Back on bottom
            if (TableManager.Tables.Count > 0 && imui.ButtonText("Previous".UiTranslate()))
            {
                Close();
            }
            imui.End();

        }
    }
}
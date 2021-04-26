//#define DEBUG

using System;
using Verse;
//using HarmonyLib;
using AutoTable;
using AutoTable.Xlsx;
using UnityEngine;
using RimWorld;
using System.Linq;
using RimDumper.Extensions;
using ImUILib;

namespace RimDumper.UI.Pages
{
    public class ApparelFilterPage : ImUILib.Page
    {
        private readonly Table _table;
        private bool _recipeOnly = true;
        private string _targetColumn = "";
        private TechLevel? _techLevel = null;
        private bool _orderByDescending = true;
        private int _countPerLayer = 1;
        private string _countPerLayerBuf = "";

        public ApparelFilterPage(ImUILib.Pages pages, Table table) : base(pages)
        {
            _table = table;
        }

        public override void Draw(Rect canvas)
        {
            UserInterface.CurrentPage = "ApparelFilterPageTitle".UiTranslate();

            const float splitCanvasHight = 60f;
            Rect top = canvas.TopPartPixels(canvas.height - splitCanvasHight); ;
            Rect bottom = canvas.BottomPartPixels(splitCanvasHight); // 2 buttons

            Listing_Styled imui = new();
            imui.Begin(top, elementHeight: UserInterface.ElementHeight, gapSize: UserInterface.GapSize);

            // with recipes checkbox
            _ = imui.DubsCheckbox("WithRecipe".UiTranslate(), ref _recipeOnly);

            // target parameter
            imui.SameLinePercent(0.5f, 0.5f);
            imui.Label("TargetParameter".UiTranslate());
            if (imui.ButtonText(_targetColumn))
            {
                var options = _table.Columns.Select(x => new FloatMenuOption(x.Name, delegate ()
                {
                    _targetColumn = x.Name;
                })).ToList();
                Find.WindowStack.Add(new FloatMenu(options));
            }

            // techLevel parameter
            imui.SameLinePercent(0.5f, 0.5f);
            imui.Label("TechLevel".ParserTranslate());
            if (imui.ButtonText(_techLevel?.ToStringHuman().CapitalizeFirst() ?? "Any".UiTranslate()))
            {
                TechLevel[] techLevels = (TechLevel[])Enum.GetValues(typeof(TechLevel));

                var options = techLevels.Select(x => new FloatMenuOption(x.ToStringHuman().CapitalizeFirst(), delegate ()
                {
                    _techLevel = x;
                })).ToList();

                options.Insert(0, new FloatMenuOption("Any".UiTranslate(), delegate ()
                {
                    _techLevel = null;
                }));

                Find.WindowStack.Add(new FloatMenu(options));
            }


            // order
            _ = imui.DubsCheckbox("OrderByDescending".UiTranslate(), ref _orderByDescending);

            // order
            _ = imui.TextFieldNumericLabeled("CountPerLayer".UiTranslate(), ref _countPerLayer, ref _countPerLayerBuf, 1f);
            imui.End();

            imui.Begin(bottom, elementHeight: 25f);
            // dump
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
                Table filtered = ApparelFilter.FilterApparels(_table, _targetColumn, _orderByDescending, _recipeOnly, _countPerLayer, _techLevel);
                string fileName = $"{TableManager.SavePath}\\ApparelFiltered_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";
                filtered.Export(fileName, TableManager.XlsxFormat);
                TableManager.OpenAction(fileName);
            }
            // back
            if (imui.ButtonText("Previous".UiTranslate()))
            {
                Close();
            }
            imui.End();
        }
    }
}
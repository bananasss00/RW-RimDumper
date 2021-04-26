//#define DEBUG

using System;
using Verse;
//using HarmonyLib;
using AutoTable;
using UnityEngine;
using System.Linq;
using RimDumper.Extensions;
using ImUILib;

namespace RimDumper.UI.Pages
{
    public class ColumnsPage : Page
    {
        private readonly Table _table;
        private string _searchStr = "";

        public ColumnsPage(ImUILib.Pages pages, Table table) : base(pages)
        {
            _table = table;
        }

        public override void Draw(Rect canvas)
        {
            UserInterface.CurrentPage = "ColumnsPageTitle".UiTranslate();

            // Page header
            const float splitCanvasHight = 35f;
            Rect top = canvas.TopPartPixels(canvas.height - splitCanvasHight); ;
            Rect bottom = canvas.BottomPartPixels(splitCanvasHight); // 1 buttons

            Listing_Styled imui = new();
            imui.Begin(top, elementHeight: UserInterface.ElementHeight, gapSize: UserInterface.GapSize);

            // Search box
            _searchStr = imui.TextArea(_searchStr);

            // ShowAll / HideAll
            imui.SameLinePercent(0.5f, 0.5f);
            if (imui.ButtonText("ShowAll".UiTranslate()))
            {
                _table.ShowAllColumns();
            }
            if (imui.ButtonText("HideAll".UiTranslate()))
            {
                _table.HideAllColumns();
            }

            // Checkboxes
            imui.ScrollStart("ColumnsPage.Columns");
            Column[] columns = _table.Columns.ToArray();
            if (!String.IsNullOrWhiteSpace(_searchStr))
            {
                columns = columns.Where(x => x.Name.IndexOf(_searchStr, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            }

            foreach (var column in columns)
            {
                // inverted value
                bool isVisible = !column.Hidden;
                if (imui.DubsCheckbox(column.Name, ref isVisible))
                {
                    column.Hidden = !isVisible;
                }
            }
            imui.ScrollEnd("ColumnsPage.Columns");
            imui.End();

            // Back on bottom
            imui.Begin(bottom, elementHeight: 25f);
            if (imui.ButtonText("Previous".UiTranslate()))
            {
                Close();
            }
            imui.End();
        }
    }
}
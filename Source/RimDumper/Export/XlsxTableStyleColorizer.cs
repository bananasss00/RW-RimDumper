//#define DEBUG

using System;
//using HarmonyLib;
using AutoTable.Xlsx;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq;
using OfficeOpenXml.ConditionalFormatting;

namespace RimDumper
{
    public class XlsxTableStyleColorizer : XlsxTableStyle
    {
        public XlsxTableStyleColorizer(string styleName) : base(styleName)
        {
        }

        public override void CreateTableStyle(string tableName, ExcelPackage pck, ExcelWorksheet ws, int rowCount, int colCount)
        {
            base.CreateTableStyle(tableName, pck, ws, rowCount, colCount);

			if (!Settings.ColorizeValues || rowCount < 2)
				return;

			for (int i = 1; i <= colCount; i++)
            {
                Type? columnType = GetFormattedColumnType(ws, i, rowCount);
				if (columnType == null)
				{
					continue;
                }
                if (columnType == typeof(float) || columnType == typeof(double) || columnType == typeof(int))
				{
					ExcelAddress address = new(2, i, rowCount, i);
					var rule = ws.ConditionalFormatting.AddThreeColorScale(address);
					rule.Priority = 1;
					rule.MiddleValue.Type = eExcelConditionalFormattingValueObjectType.Percentile;
					rule.MiddleValue.Value = 50;
					rule.StopIfTrue = true;
					rule.Style.Font.Bold = true;
				}
				else if (columnType == typeof(bool))
                {
                    ColorizeBools(ws, i, rowCount);
                }
            }
        }

        private static void ColorizeBools(ExcelWorksheet ws, int column, int rowCount)
        {
            ExcelRange columnCells = ws.Cells[2, column, rowCount, column];
            foreach (var cell in columnCells)
            {
                bool? value = cell.Value as bool?;
                if (value == null)
                {
                    continue;
                }
                var fill = cell.Style.Fill;
				fill.PatternType = ExcelFillStyle.Solid;
                fill.BackgroundColor.SetColor((bool)value ? EPPlus.Drawing.Color.Green : EPPlus.Drawing.Color.Red);
            }
        }

        private static Type? GetFormattedColumnType(ExcelWorksheet ws, int column, int rowCount)
        {
			Type[] targetTypes = {typeof(float), typeof(double), typeof(int), typeof(bool)};
			ExcelRange columnCells = ws.Cells[2, column, rowCount, column];
			Type? columnType = null;
			
            foreach (var cell in columnCells)
            {
                Type? cellType = cell.Value?.GetType();
				if (cellType == null)
				{
					continue;
				}
				if (targetTypes.Contains(cellType))
				{
					columnType = cellType;
					break;
				}
            }

            return columnType;
        }
    }
}
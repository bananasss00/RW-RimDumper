//#define DEBUG

//using HarmonyLib;
using AutoTable.Xlsx;
using OfficeOpenXml;

namespace RimDumper
{
    public class XlsxTableStyleExtended : XlsxTableStyle
    {
        private readonly CellColorizer cellColorizer;
        private readonly GoogleTableFixes googleTableFixes;

        public XlsxTableStyleExtended(string styleName) : base(styleName)
        {
            cellColorizer = new();
            googleTableFixes = new(styleName);
        }

        public override void CreateTableStyle(string tableName, ExcelPackage pck, ExcelWorksheet ws, int rowCount, int colCount)
        {
            base.CreateTableStyle(tableName, pck, ws, rowCount, colCount);

            if (Settings.ColorizeValues && rowCount >= 2)
				cellColorizer.CreateTableStyle(tableName, pck, ws, rowCount, colCount);

            if (Settings.GoogleTablesMode)
                googleTableFixes.CreateTableStyle(tableName, pck, ws, rowCount, colCount);
        }
    }
}
//#define DEBUG

//using HarmonyLib;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace RimDumper
{
    public class GoogleTableFixes
    {
        private readonly string styleName;

        public GoogleTableFixes(string styleName)
        {
            this.styleName = styleName;
        }

        public void CreateTableStyle(string tableName, ExcelPackage pck, ExcelWorksheet ws, int rowCount, int colCount)
        {
			//*** Fix for Google.Tables. Conflict with Microsoft Excel! ***//

            // auto filter on cells
            ws.Cells[ws.Dimension.Address].AutoFilter=true;

            // google.tables incorrectly parses the color of the table header.
            // remove existing table
            ws.Tables.Delete(0);
            // create new without header
            var validTableName = tableName.Replace(" ", "_");
            var table = ws.Tables.Add(ws.Cells[2, 1, rowCount, colCount], validTableName);
            table.TableStyle = TableStyles.Medium27; //Light16,Light21
            table.ShowHeader = false;
            // add header style
            var header = ws.Cells[1, 1, 1, colCount];
            header.StyleName = styleName;
        }
    }
}
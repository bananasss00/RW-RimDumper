using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace AutoTable.Xlsx
{
    public class XlsxTableStyle : IXlsxTableStyle
    {
        public virtual string? StyleName { get; protected set; }
        public XlsxTableStyle(string styleName)
        {
            StyleName = styleName;
        }

        public virtual void CreateNamedStyle(ExcelPackage pck)
        {
            var headerStyle = pck.Workbook.Styles.CreateNamedStyle(StyleName);
            headerStyle.Style.Font.Bold = true;
            headerStyle.Style.WrapText = true;
            headerStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            headerStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            headerStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerStyle.Style.Fill.BackgroundColor.SetColor(EPPlus.Drawing.Color.DarkSlateGray);
            headerStyle.Style.Font.Color.SetColor(EPPlus.Drawing.Color.White);
        }

        public virtual void CreateTableStyle(string tableName, ExcelPackage pck, ExcelWorksheet ws, int rowCount, int colCount)
        {
            var validTableName = tableName.Replace(" ", "_");
            var excelTable = ws.Tables.Add(ws.Cells[1, 1, rowCount, colCount], validTableName);
            if (StyleName != null)
            {
                excelTable.HeaderRowCellStyle = StyleName;
            }
            excelTable.TableStyle = TableStyles.Medium27; //Light16,Light21
            excelTable.ShowTotal = true;

            // Centered values
            if (rowCount > 1 && colCount > 1)
            {
                ws.Cells[2, 2, rowCount, colCount].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[2, 2, rowCount, colCount].Style.WrapText = true; // перенос по словам для длинных строк, чтобы не перекрывали другие ячейки
            }
            // Calc columns width
            for (int i = 1; i <= colCount; i++)
            {
                const int minWidth = 7;
                const int maxWidth = 50;
                var column = ws.Column(i);
                ExcelRange excelRange = ws.Cells[2, i, rowCount, i];
                int maxLenInColumn = excelRange.Max(x => x.Value?.ToString().Length ?? 0);
                column.Width = maxLenInColumn * 1.25f;
                // clamp value
                if (column.Width < minWidth)
                {
                    column.Width = minWidth;
                }

                if (column.Width > maxWidth)
                {
                    column.Width = maxWidth;
                }
            }

            // Forcing rows Height to 15(default value) after setted WrapText
            if (rowCount > 1)
            {
                for (int i = 2; i <= rowCount; i++)
                {
                    ws.Row(i).Height = 15;
                }
            }
        }
    }
}
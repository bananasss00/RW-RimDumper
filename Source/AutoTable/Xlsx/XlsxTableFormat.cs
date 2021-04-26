using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace AutoTable.Xlsx
{
    public class XlsxTableFormat : ITableFormat
    {
        private readonly IXlsxTableStyle? tableStyle;

        public XlsxTableFormat(IXlsxTableStyle? tableStyle = null)
        {
            this.tableStyle = tableStyle;
        }

        public virtual void Export(Table table, string fileName)
        {
            using ExcelPackage pck = new();

            tableStyle?.CreateNamedStyle(pck);
            CreateTable(table, pck);
            pck.SaveAs(new FileInfo(fileName));
        }

        public virtual void Export(IEnumerable<Table> tables, string fileName)
        {
            using ExcelPackage pck = new();
            tableStyle?.CreateNamedStyle(pck);
            foreach (var table in tables)
            {
                CreateTable(table, pck);
            }
            pck.SaveAs(new FileInfo(fileName));
        }

        protected virtual void CreateTable(Table table, ExcelPackage pck)
        {
            var validTableName = table.Name.Replace(" ", "_");
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(validTableName);

            Column[] columns = table.Columns.VisibleColumns.ToArray();
            object?[][] rowsValues = table.Rows.VisibleRowValues.ToArray();

            //Write header
            int col = 1;
            foreach (var tColumn in columns)
            {
                ExcelRange cell = ws.Cells[1, col];
                cell.Value = tColumn.Name;
                col++;
            }
            //Write rows
            int row = 2;
            foreach (var tRow in rowsValues)
            {
                col = 1;
                for (int i = 0; i < tRow.Length; ++i)
                {
                    ExcelRange cell = ws.Cells[row, col];
                    cell.Value = tRow[i];
                    if (cell.Value is float single)
                    {
                        cell.Value = Math.Round(single, 2); // auto precision
                        //cell.Style.Numberformat.Format = "#,##0.00";
                    }

                    if (col == 1)
                    {
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    else
                    {
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    }

                    col++;
                }

                row++;
            }

            //ws.Cells.AutoFitColumns(10f); // not available in lite version EPPlus for unity
            ws.View.FreezePanes(2, 2);

            ExcelCellAddress? endCell = ws.Dimension?.End;
            if (endCell != null)
            {
                int rowCount = endCell.Row;
                int colCount = endCell.Column;
                tableStyle?.CreateTableStyle(validTableName, pck, ws, rowCount, colCount);
            }
        }
    }
}
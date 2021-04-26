using OfficeOpenXml;

namespace AutoTable.Xlsx
{
    public interface IXlsxTableStyle
    {
        void CreateNamedStyle(ExcelPackage pck);
        void CreateTableStyle(string tableName, ExcelPackage pck, ExcelWorksheet ws, int rowCount, int colCount);
    }
}
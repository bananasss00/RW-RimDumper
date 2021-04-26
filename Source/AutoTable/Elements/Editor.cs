using System.Collections.Generic;
using System.Linq;

namespace AutoTable
{
    public sealed class Editor
    {
        private readonly ColumnCollection _columnsOriginal, _columnsNew;
        private readonly RowCollection _rowsOriginal, _rowsNew;

        private readonly Table _table;

        internal Editor(Table table)
        {
            _table = table;
            _columnsOriginal = table.Columns;
            _rowsOriginal = table.Rows;
            _rowsNew = new();
            _columnsNew = new();
        }

        public void StartEdit()
        {
            _columnsNew.Clone(_columnsOriginal);
            _rowsNew.Clone(_rowsOriginal);
        }

        public List<Column> Columns
        {
            get => _columnsNew.ListInternal;
            set => _columnsNew.ListInternal = value;
        }

        public List<Row> Rows
        {
            get => _rowsNew.ListInternal;
            set => _rowsNew.ListInternal = value;
        }

        public Table FinalizeTable()
        {
            Table newTable = new(_table.Name);
            foreach (var row in Rows)
            {
                var newRow = newTable.NewRow();
                foreach (var column in Columns)
                {
                    object? value = row[column];
                    if (value == null)
                    {
                        continue;
                    }

                    if (newTable.Columns.FirstOrDefault(x => x.Name.Equals(column.Name)) == null)
                    {
                        _ = newTable.NewColumn(column.Name, column.flags);
                    }

                    newRow[column.Name] = value;
                }
            }
            _columnsNew.Clear();
            _rowsNew.Clear();
            return newTable;
        }
    }
}
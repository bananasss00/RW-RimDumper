namespace AutoTable
{
    public class Table
    {
        private int latestColumnId = 0;
        public ColumnCollection Columns { get; private set; }
        public RowCollection Rows { get; private set; }

        public Editor Editor { get; private set; }

        public string Name { get; set; }

        public Table(string name)
        {
            Name = name;
            Columns = new();
            Rows = new();
            Editor = new(this);
        }

        public virtual Row NewRow()
        {
            Row row = new(this);
            Rows.Add(row);
            return row;
        }

        public virtual Column NewColumn(string name, ColumnFlags flags = ColumnFlags.None)
        {
            Column column = new(latestColumnId++, name, flags);
            Columns.Add(column);
            return column;
        }

        public void ShowAllColumns()
        {
            foreach (var column in Columns)
            {
                column.Hidden = false;
            }
        }

        public void HideAllColumns()
        {
            foreach (var column in Columns)
            {
                column.Hidden = true;
            }
        }

        public void Export(string fileName, ITableFormat tableFormat)
        {
            tableFormat.Export(this, fileName);
        }
    }
}
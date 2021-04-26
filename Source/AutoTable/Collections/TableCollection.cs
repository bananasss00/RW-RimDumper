using System.Collections.Generic;

namespace AutoTable
{
    public class TableCollection : ListBase<Table>
    {
        private readonly List<Table> tables = new();

        protected override List<Table> List => tables;

        public void Export(string fileName, ITableFormat tableFormat)
        {
            tableFormat.Export(tables, fileName);
        }
    }
}
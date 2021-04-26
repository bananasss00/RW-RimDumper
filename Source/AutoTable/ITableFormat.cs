using System.Collections.Generic;

namespace AutoTable
{
    public interface ITableFormat
    {
        void Export(Table table, string fileName);
        void Export(IEnumerable<Table> tables, string fileName);
    }
}
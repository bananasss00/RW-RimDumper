using System.Collections.Generic;
using System.Linq;

namespace AutoTable
{
    public sealed class ColumnCollection : ListBase<Column>
    {
        protected override List<Column> List => ListInternal;

        internal ColumnCollection()
        {
        }

        public Column? this[string columnName] => ListInternal.Find(x => x.Name.Equals(columnName));

        public IEnumerable<Column> VisibleColumns
        {
            get
            {
                for (int i = 0; i < ListInternal.Count; i++)
                {
                    Column column = ListInternal[i];
                    if (!column.Hidden)
                    {
                        yield return column;
                    }
                }
            }
        }

        #region Internal modify collections

        internal void Clone(ColumnCollection from)
        {
            ListInternal = from.ListInternal.ToList();
        }

        internal List<Column> ListInternal { get; set; } = new();

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;

namespace AutoTable
{
    public sealed class RowCollection : ListBase<Row>
    {
        protected override List<Row> List => ListInternal;

        internal RowCollection()
        {
        }

        public IEnumerable<object?[]> RowValues
        {
            get
            {
                for (int i = 0; i < ListInternal.Count; i++)
                {
                    Row row = ListInternal[i];
                    yield return row.GetValues().ToArray();
                }
            }
        }

        public IEnumerable<object?[]> VisibleRowValues
        {
            get
            {
                for (int i = 0; i < ListInternal.Count; i++)
                {
                    Row row = ListInternal[i];
                    yield return row.GetValues(false).ToArray();
                }
            }
        }

        #region Internal modify collections

        internal void Clone(RowCollection from)
        {
            ListInternal = from.ListInternal.ToList();
        }

        internal List<Row> ListInternal { get; set; } = new();

        #endregion

    }
}
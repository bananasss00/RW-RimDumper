using System.Collections.Generic;

namespace AutoTable
{
    public sealed class Row
    {
        private readonly ColumnCollection _columns;
        private readonly Table _table;
        private readonly Dictionary<Column, object?> _values = new();
        internal Row(Table table)
        {
            _table = table;
            _columns = table.Columns;
        }

        public /*virtual*/ object? this[string columnName]
        {
            get => GetValue(columnName);
            set => SetValue(columnName, value);
        }

        public /*virtual*/ object? this[Column column]
        {
            get => GetValue(column);
            set => SetValue(column, value);
        }

        #region GetValue
        public object? GetValue(Column column)
        {
            return !_values.ContainsKey(column) ? null : _values[column];
        }

        public object? GetValue(string columnName)
        {
            var column = _columns[columnName];
            return column == null ? null : GetValue(column);
        }

        public T? GetValue<T>(string columnName)
        {
            return (T?)GetValue(columnName);
        }

        public T? GetValue<T>(Column column)
        {
            return (T?)GetValue(column);
        }

        public IEnumerable<object?> GetValues(bool showHidden = true)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                Column column = _columns[i];
                if (!showHidden && column.Hidden)
                {
                    continue;
                }

                yield return GetValue(column);
            }
        }

        #endregion GetValue

        #region SetValue

        public void SetValue(string columnName, object? value)
        {
            var column = _columns[columnName];
            if (column == null)
            {
                column = _table.NewColumn(columnName);
            }
            SetValue(column, value);
        }

        public void SetValue(Column column, object? value)
        {
            if (!_values.ContainsKey(column))
            {
                _values.Add(column, value);
                return;
            }
            _values[column] = value;
        }

        #endregion SetValue

    }
}
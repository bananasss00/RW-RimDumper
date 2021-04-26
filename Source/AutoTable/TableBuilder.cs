

namespace AutoTable
{
    public class TableBuilder
    {
        public static Table? Create(IParser parser)
        {
            Table? table = parser.Create();
            return table;
        }
    }
}
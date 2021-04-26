using System;

namespace AutoTable
{
    [Flags]
    public enum ColumnFlags
    {
        None = 0,
        Hidden = 1 << 0,
        //AnotherFlag = 1 << 1,
        //AnotherFlag = 1 << 2,
        //AnotherFlag = 1 << 3,
        //AnotherFlag = 1 << 4,
        //AnotherFlag = 1 << 5,
        //AnotherFlag = 1 << 6,
        //AnotherFlag = 1 << 7,
    }
}
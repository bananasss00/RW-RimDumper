using System;

namespace AutoTable
{
    /// <summary>
    /// https://github.com/Zetrith/HotSwap/wiki
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
}
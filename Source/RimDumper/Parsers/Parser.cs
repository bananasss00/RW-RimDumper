using AutoTable;

namespace RimDumper.Parsers
{
    public abstract class Parser : IParser
    {
        public abstract string Name { get; }
        public abstract Table? Create();
    }
}
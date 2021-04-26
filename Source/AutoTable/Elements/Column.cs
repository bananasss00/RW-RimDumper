namespace AutoTable
{
    public sealed class Column
    {
        public readonly int Id;

        public readonly string Name;

        public ColumnFlags flags;

        internal Column(int id, string name, ColumnFlags flags)
        {
            Id = id;
            Name = name;
            this.flags = flags;
        }

        public bool Hidden
        {
            get => flags.HasFlag(ColumnFlags.Hidden);
            set
            {
                if (value)
                {
                    // add flag
                    flags |= ColumnFlags.Hidden;
                }
                else
                {
                    // remove flag
                    flags &= ~ColumnFlags.Hidden;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Column c && Name.Equals(c.Name);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
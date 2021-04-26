using Verse;

namespace ImUILib
{
    /// <summary>
    /// Set globally font when pushed, and removed!!!
    /// </summary>
    public class StackFont : StackRef<GameFont>
    {
        public override void Push(GameFont value)
        {
            base.Push(value);
            Text.Font = value;
        }

        public override GameFont Pop(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (Count == 1)
                {
                    return Text.Font = _stack.Pop().value;
                }
                _ = _stack.Pop(); // pop active
                Text.Font = _stack.Peek().value; // peak previous
            }
            return Text.Font;
        }

        /// <summary>
        /// Doesn't do anything!
        /// </summary>
        /// <param name="value"></param>
        public override void Set(GameFont value)
        {
            base.Set(value);
        }
    }
}
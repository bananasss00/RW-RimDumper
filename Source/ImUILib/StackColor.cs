using UnityEngine;

namespace ImUILib
{
    /// <summary>
    /// Set globally color when pushed, and removed!!!
    /// </summary>
    public class StackColor : StackRef<Color>
    {
        public override void Push(Color value)
        {
            base.Push(value);
            GUI.color = value;
        }

        public override Color Pop(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (Count == 1)
                {
                    return GUI.color = _stack.Pop().value;
                }
                _ = _stack.Pop(); // pop active
                GUI.color = _stack.Peek().value; // peak previous
            }
            return GUI.color;
        }

        /// <summary>
        /// Doesn't do anything!
        /// </summary>
        /// <param name="value"></param>
        public override void Set(Color value)
        {
            base.Set(value);
        }
    }

}
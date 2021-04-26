namespace ImUILib
{
    public class Stack<T> : System.Collections.Generic.Stack<T>
    {
        public virtual bool HasElements => Count > 0;

        public virtual bool IsEmpty => Count == 0;
    }
}
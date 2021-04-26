using System.Collections.Generic;

namespace ImUILib
{
    public class StackRef<T> where T : struct
    {
        public class Value
        {
            public T value;

            public Value(T value)
            {
                this.value = value;
            }
        }

        protected Stack<Value> _stack = new();

        public virtual void Push(T value)
        {
            _stack.Push(new Value(value));
        }

        public virtual T Pop(int count = 1)
        {
            T value = default;
            for (int i = 0; i < count; i++)
            {
                value = _stack.Pop().value;
            }
            return value;
        }

        public virtual T Peek()
        {
            return _stack.Peek().value;
        }

        public virtual void Set(T value)
        {
            _stack.Peek().value = value;
        }

        public virtual int Count => _stack.Count;

        public virtual bool HasElements => _stack.Count > 0;
        public virtual bool IsEmpty => _stack.Count == 0;

        public virtual void Clear()
        {
            _stack.Clear();
        }
    }
}
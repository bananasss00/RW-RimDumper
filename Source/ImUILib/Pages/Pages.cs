//#define DEBUG

//using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImUILib
{
    public class Pages
    {
        private readonly Stack<Page> _pages = new();

        public int Count => _pages.Count;
        public bool IsEmpty => _pages.Count == 0;

        public Page? CurrentPage => IsEmpty ? null : _pages.Peek();

        public virtual void Draw(Rect rect)
        {
            CurrentPage?.Draw(rect);
        }

        public virtual Page Open<T>(params object[] args) where T : Page
        {
            var thisAndArgs = new object[] { this }.Union(args).ToArray();
            var page = (T)Activator.CreateInstance(typeof(T), thisAndArgs);
            _pages.Push(page);
            return page;
        }

        public virtual void Close()
        {
            if (!IsEmpty)
            {
                _ = _pages.Pop();
            }
        }

        public virtual Page Switch<T>(params object[] args) where T : Page
        {
            Close();
            return Open<T>(args);
        }
    }
}
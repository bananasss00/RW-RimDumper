//#define DEBUG

//using HarmonyLib;
using UnityEngine;

namespace ImUILib
{
    public class PageContainer
    {
        private readonly Pages _pages = new();

        public virtual void Draw(Rect rect)
        {
            _pages.CurrentPage?.Draw(rect);
        }

        public virtual Page OpenPage<T>(params object[] args) where T : Page
        {
            return _pages.Open<T>(args);
        }

        public virtual void Close()
        {
            _pages.Close();
        }

        public virtual Page ReplacePage<T>(params object[] args) where T : Page
        {
            return _pages.Switch<T>(args);
        }
    }

    public abstract class Page
    {
        private readonly Pages _pages;

        protected Page(Pages pages)
        {
            _pages = pages;
        }

        public abstract void Draw(Rect rect);

        public virtual Page Open<T>(params object[] args) where T : Page
        {
            return _pages.Open<T>(args);
        }

        public virtual void Close()
        {
            _pages.Close();
        }

        public virtual Page Switch<T>(params object[] args) where T : Page
        {
            return _pages.Switch<T>(args);
        }
    }
}
using System;
using UnityEngine;

namespace nexcode.nwcore
{
    public interface IBrowser : ITexture
    {
        string Url { get; set; }
        Vector2 Size { get; set; }

        bool EnableRendering { get; set; }
        bool EnableInput { get; set; }

        bool IsReady { get; }

        float Zoom { get; set; }

        void LoadHTML(string html, string url);
        bool CanGoBack { get; }
        void GoBack();

        bool CanGoForward { get; }
        void GoForward();

        void Reload();

        void EvalJS(string js, string module);
        void RegisterFunction(string funcName, Action<object[]> func);

        void WhenReady(Action readyAction);
        void WhenLoaded(Action loadedAction);
    }
}
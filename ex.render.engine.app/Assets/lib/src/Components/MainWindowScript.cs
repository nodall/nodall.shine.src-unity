using UnityEngine;
using System;
using System.Collections;

namespace nexcode.nwcore
{
    public class MainWindowScript : MonoBehaviour
    {
        #region [ class ]
        /*[Serializable]
        public class Settings
        {
            public float x;
            public float y;
            public float width;
            public float height;
            public bool showFps;
        }*/
        #endregion

        #region [ public fields ]
        public NWCoreBase.WindowSettings settings;
        public Vector2 screenPos;
        public Vector2 screenSize;
        public bool showFps;
        #endregion

        #region [ MonoBehaviours ]

        public void Start()
        {
            bool switchToWindowed = false;

            Debug.Log("Start()");

            settings = NWCoreBase.instance.nwCoreSettings.window;
            screenPos = new Vector2(settings.x, settings.y);
            screenSize = new Vector2(settings.width, settings.height);
            showFps = NWCoreBase.instance.nwCoreSettings.showFps;

            if (screenSize.x == 0 && screenSize.y == 0)
                return;

            if (!Application.isEditor && Screen.fullScreen)
            {
                Screen.fullScreen = false;
                switchToWindowed = true;
            }


            if (!Application.isEditor)
            {
                if (switchToWindowed)
                {
                    StartCoroutine(CoSetWindowInNextFrame());
                }
                else
                {
                    WinApi.Instance.SetWindowsRectangle(screenPos, screenSize);
                    Cursor.visible = false;
                }
            }
        }

        public void RepositionWindow()
        {
            StartCoroutine(CoSetWindowInNextFrame());
        }


        IEnumerator CoSetWindowInNextFrame()
        {
            yield return null;
            WinApi.Instance.SetWindowsRectangle(screenPos, screenSize);
            Cursor.visible = false;
        }
        #endregion

        #region [ Render on Screen ]
        GUIStyle guiStyle = new GUIStyle();
        void OnGUI()
        {
            if (showFps)
            {
                guiStyle.fontSize = 20;
                guiStyle.normal.background = Texture2D.blackTexture;
                guiStyle.normal.textColor = Color.black;
                GUI.Label(new Rect(9, 9, 100, 30), (int)(1f / Time.smoothDeltaTime) + " fps", guiStyle);
                GUI.Label(new Rect(11, 11, 100, 30), (int)(1f / Time.smoothDeltaTime) + " fps", guiStyle);
                guiStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(10, 10, 100, 30), (int)(1f / Time.smoothDeltaTime) + " fps", guiStyle);
            }
        }
        #endregion
    }
}
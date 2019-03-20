using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace nexcode.nwcore
{
    public class SettingsManager
    {
        public JObject Root { get; private set; }

        #region [ Events ]
        public EventHandler EventChanged;
        #endregion

        #region [ script properties ]
        public string appSettingsPath;
        [TextArea(10, 11)]
        public string settingsContent;
        #endregion

        public void Initialise()
        {
            ReadFile();
        }

        #region [ private ]
        void ReadFile()
        {
            try
            {
                var fullPath = @"..\..\settings\" + Application.productName + @"\settings.json";
                if (Application.isEditor)
                    fullPath = @".\" + @"\settings.json";

                appSettingsPath = Path.GetFullPath(fullPath);

                Debug.Log("ReadFile() > Read File " + fullPath);
                Root = JObject.Parse(File.ReadAllText(fullPath));
                settingsContent = Root != null ? Root.ToString() : "empty";
                if (EventChanged != null)
                    EventChanged(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Debug.LogError("ReadFile() > " + ex.Message);
            }
        }
        #endregion

        #region [ public  methods]
        public T Get<T>(params string[] args)
        {
            JToken token = Root;
            foreach (var arg in args)
                token = token[arg];

            return JsonConvert.DeserializeObject<T>(token.ToString());
        }

        public void SaveSettings()
        {
            try
            {
                var fullPath = @"..\settings\" + Application.productName + @"\settings.json";
                if (Application.isEditor)
                    fullPath = @".\settings\" + Application.productName + @"\settings.json";

                appSettingsPath = Path.GetFullPath(fullPath);

                Debug.Log("SaveSettings() > Save File " + fullPath);
                var contents = Root.ToString(Formatting.Indented);
                File.WriteAllText(fullPath, contents);
            }
            catch (Exception ex)
            {
                Debug.LogError("SaveSettings() > " + ex.Message);
            }
        }
        #endregion

    }
}
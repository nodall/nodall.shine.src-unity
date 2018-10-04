using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace Components
{
    public class AppSettingsScript : MonoBehaviour
    {
        #region [ Singleton ]
        public static AppSettingsScript Instance { get; private set; }
        #endregion

        #region [ Properties ]
        public JObject Root { get { return NWCore.instance.Settings.Root; } }
        public string WWWFolder { get { return Root["Shine"]["Folders"].Value<string>("www"); } }
        public string SessionsFolder { get { return Root["Shine"]["Folders"].Value<string>("sessions"); } }
        public string AssetsFolder { get { return Root["Shine"]["Folders"].Value<string>("assets"); } }
        public string MediasFolder { get { return Root["Shine"]["Folders"].Value<string>("medias"); } }
        public string WebAppsFolder { get { return AssetsFolder + "webapps/"; } }
        #endregion

        #region [ public  methods]
        public T Get<T>(params string[] args)
        {
            return NWCore.instance.Settings.Get<T>(args);
        }
        #endregion

        private void Awake()
        {
            Debug.Log(NWCore.instance.Settings.appSettingsPath);
            Instance = this;
        }
    }
}

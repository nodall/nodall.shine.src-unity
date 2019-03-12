using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using nexcode.network.http.websocket;
using nexcode.network.http.wssharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WanzyeeStudio;

namespace nexcode.nwcore
{
    public class NWCoreBase : MonoBehaviour
    {
        public static NWCoreBase instance;

        public NWCoreSettings nwCoreSettings;

        public static WSClient wsClient;
        public static HubClientContract hub;
        public static RestClientContract rest;

        bool hasToReconnect;

        SettingsManager settingsManager = new SettingsManager();
        public SettingsManager Settings { get { return settingsManager; } }

        [Serializable]
        public class NWCoreSettings
        {
            public WSClientSettings wsClient;
            [ReadOnly]
            public string wwwFolders;
            public WindowSettings window;
            public bool showFps;
        }

        [Serializable]
        public class WSClientSettings
        {
            [ReadOnly()]
            public string url, hubChannel, restChannel;
            public BasicClientData connectionData;
        }

        [Serializable]
        public class WindowSettings
        {
            public string type;
            public int x, y, width, height;
        }

        // This method should not be called
        private void Awake()
        {
            Debug.Log("[NWCoreBase] Awake");
            Initialise();
        }

        public void Initialise()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings() { Converters = JsonNetUtility.defaultSettings.Converters, DefaultValueHandling = DefaultValueHandling.Populate };

            instance = this;
            Settings.Initialise();

            Debug.Log("<b><color=white>[NWCoreBase] Initialise "+name+"</color></b>");
            nwCoreSettings = Settings.Get<NWCoreSettings>("NWCore");

            var url = nwCoreSettings.wsClient.url;

            // create connection
            var basicConn = new BasicConnectionClientContract<BasicClientData, BasicServerData>()
                .OnSendClientData(() => { return nwCoreSettings.wsClient.connectionData; });

            wsClient = new WSClient(new SharpClientSocket(), url, basicConn);

            Debug.Log("Going to create hub");
            hub = new HubClientContract();
            wsClient.Contracts.Add(hub);

            Debug.Log("Going to create rest");
            rest = new RestClientContract();
            wsClient.Contracts.Add(rest);


            wsClient.OnError((ev) =>
            {
                Debug.Log("[NWCoreBase] WSC::OnError");
                hasToReconnect = true;
            });

            //wsClient.OnOpen((ev) => Debug.Log("[NWCoreBase] WSC::OnOpen"));
            wsClient.OnClose((ev) => Debug.Log("[NWCoreBase] WSC::OnClose"));

            // Create nexwarp hierarchy

            var mediaSources = new GameObject("Texture Components");
            mediaSources.transform.SetParent(transform, false);

            var layerCompositor = new GameObject("LayerCompositor");
            layerCompositor.transform.SetParent(transform, false);

            var screenCompositor = new GameObject("ScreenCompositor");
            screenCompositor.transform.SetParent(transform, false);

            var outputsParent = new GameObject("Outputs");
            outputsParent.transform.SetParent(transform);

            ComponentManager.DefineParentFor<ComponentBase>(mediaSources);
            ComponentManager.DefineParentFor<TextureComponentBase<ComponentProps>>(mediaSources);
            ComponentManager.DefineParentFor<MediaCompositorBase<MediaCompositorProps>>(layerCompositor);
            ComponentManager.DefineParentFor<QuadSurfaceComponent>(screenCompositor);

            if (nwCoreSettings.window.width != 0 && nwCoreSettings.window.height != 0)
            {
                gameObject.AddComponent<MainWindowScript>();
            }
        }

        public void Connect()
        {
            Debug.Log("<b><color=white>[NWCoreBase] Connect</color></b>");
            wsClient.Connect();

            // Use broker?
            gameObject.AddComponent<NWCoreBroker>();

            // setup camera
            Camera.main.orthographicSize = 0.5f;
            Camera.main.orthographic = true;
            Camera.main.transform.localPosition = new Vector3(0, 0, -10);
        }

        private void OnDestroy()
        {
            if (wsClient != null && wsClient.IsOpened)
                wsClient.Close("Unity application stopped");

            wsClient = null;
        }
    }
}

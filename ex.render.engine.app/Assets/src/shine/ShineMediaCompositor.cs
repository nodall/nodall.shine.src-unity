using UnityEngine;
using System;
using System.Collections;
using nexcode.nwcore;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using Components;
using System.IO;
using Shine.Apps;
using Newtonsoft.Json.Linq;
using nexcode.network.http.rest;
using Newtonsoft.Json;
using UnityEngine.UI;
using Shine.Components;

public class ShineMediaCompositor : LayerMediaCompositor
{
    public static ShineMediaCompositor Instance;

    //AppsManagerScript manager;

    [Serializable]
    public class Settings
    {
        public int desktopDuplicationMonitorId;
        public int width, height;
        public float transitionSeconds;
    }

    public Settings settings;

    public float TransitionSeconds { get; set; }

    public float periodicUpdateSeconds = 0.9f;

    public MediaDesktopDuplication mediaDesktopDup;
    public MediaCapture mediaCapture;
    public GalleryMediaSource mediaGallery;
    public QRGeneratorComponent mediaQR;
    public MediaCanvas mediaText;


    public DateTime lastStateUpdate;
    public bool needsToUpdateState;

    public int desktopDuplicationMonitorId = 0;

    public UnityServiceState state = new UnityServiceState();

    public List<Layer> layers;
    public MediaVideo[] videos;
    public MediaSlideshow mediaSlideshow;
    public MediaColor mediaNull;


    [Serializable]
    public class UnityServiceState
    {
        public List<LayerState> layers;
    }

    [Serializable]
    public class LayerState
    {
        public string view;
        public object state;
        public object args;
        public object metadata;
        public float[] posRect;
        public float[] uvRect;
        public float volume;
        [JsonIgnore]
        public Vector2 targetPos = Vector2.one/2, targetSize = Vector2.one;
        [JsonIgnore]
        public float currentVolume;
    }

    [Serializable]
    public class UnityGalleryState
    {
        public bool isPlaying;
        public string lastFocusedPhoto;
        public string[] photos;
        public string[] selectedPhotos;
    }


    [Serializable]
    public class UnityTextArgs
    {
        public string text;
        public string color;
        public int size;
        public float[] rect;
        public string align;  // [Upper|Middle|Lower][Left|Center|Right]
    }

    [Serializable]
    public class Layer
    {
        public bool isTransitioning;
        public bool isCurrentB;
        public LayerState stateA;
        public LayerState stateB = new LayerState { view = "none", args = new JObject(), metadata = new JObject() };
        public ComponentBase mediaA;
        public ComponentBase mediaB;

        public ComponentBase nextMedia;
        public LayerState nextState;

        public ComponentBase CurrentMedia { get { return isCurrentB ? mediaB : mediaA; } }
        public LayerState CurrentState { get { return isCurrentB ? stateB : stateA; } set { if (isCurrentB) stateB = value; else stateA = value; } }

    }

    private Layer GetLayer(int layer)
    {
        if (layers.Count > layer) {
            if (layers[layer] == null)
                layers[layer] = new Layer();

            return layers[layer];
        }
        var curLayerCount = layers.Count;
        Layer newLayer = null;
        while (curLayerCount <= layer)
        {
            newLayer = new Layer();
            layers.Insert(curLayerCount, newLayer);
            curLayerCount++;
        }
        return newLayer;
    }

    private string GetPathFromLocation(string location)
    {
        if (location == "medias")
            return AppSettingsScript.Instance.MediasFolder;
        return AppSettingsScript.Instance.WWWFolder;
    }

    private MediaVideo GetFreeVideo()
    {
        for (int i = 0; i < videos.Length; ++i)
        {
            if (videos[i].props.url == null)
                return videos[i];
        }

        return videos[0];
    }

    private ComponentBase GetMedia(int layer, LayerState state)
    {
        var args = state.args as JToken;
        var view = state.view;

        var location = args != null && args["location"] != null ? args["location"].ToString() : null;

        switch (view)
        {
            case "capture":
                if (mediaCapture == null)
                    mediaCapture = ComponentManager.New<MediaCapture>(new MediaCaptureProps { path = "/0_1920X1080_MJPG" });
                return mediaCapture;
            case "desktop":
                return mediaDesktopDup;
            case "colors":
                var colors = ColorsView.Instance;
                Debug.Log(args);
                colors.PlayColors(args.ToObject<UnityColorsArgs>());
                colors.OnEnd(() => { Debug.LogWarning("onMediaEnd"); NWCoreBase.hub.Publish(NWCore.instance.nwCoreSettings.wsClient.hubChannel, "onMediaEnd", new { layerIndex = layer, layer = state }); });
                return colors;
            case "gallery":
                var imagesPath = Path.Combine(GetPathFromLocation(location), args["src"].ToString());
                Debug.Log("--------" + imagesPath);
                var gallery = FindObjectOfType<GalleryMediaSource>();
                gallery.imagesFolder = imagesPath;
                gallery.GalleryOpen();
                return gallery;
            case "slideshow":
                //var ssw = mediaSlideshow;
                var slides = args["slides"].ToObject<List<string>>().Select(s => Path.Combine(GetPathFromLocation(location), s));
                var ssw = ComponentManager.New<MediaSlideshow>(new MediaSlideshowProps { width = 1920, height = 1080, slides = slides.ToArray(), slideFadeTime = args.Value<float>("slideFadeTime"), slideTime = args.Value<float>("slideTime") });
                ssw.Play();
                return ssw;
            case "qr":
                if (args != null)
                {
                    var props = mediaQR.props;
                    if (args["qrData"] != null)
                    {
                        props.qrData = args.Value<string>("qrData");
                    }
                    if (args["foreColor"] != null)
                        ColorUtility.TryParseHtmlString(args.Value<string>("foreColor"), out props.foreColor);
                    if (args["backColor"] != null)
                        ColorUtility.TryParseHtmlString(args.Value<string>("backColor"), out props.backColor);

                    Debug.LogWarning("Going to set props to qr");
                    mediaQR.SetProps(props);
                }
                return mediaQR;
            case "text":
                var argsText = args.ToObject<UnityTextArgs>();
                var text = mediaText.props.canvas.GetComponentInChildren<Text>();
                text.text = argsText.text;
                var color = text.color;
                ColorUtility.TryParseHtmlString(argsText.color, out color);
                text.color = color;
                text.fontSize = argsText.size;
                text.alignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), argsText.align);
                var textRect = text.GetComponent<RectTransform>();
                textRect.anchoredPosition = new Vector2(argsText.rect[0] * Screen.width, (1 - argsText.rect[1] - argsText.rect[3]) * Screen.height);
                textRect.sizeDelta = new Vector2(argsText.rect[2] * Screen.width, argsText.rect[3] * Screen.height);
                return mediaText;
            case "video":
                var video = GetFreeVideo();
                var videoUrl = Path.Combine(GetPathFromLocation(location), args["src"].ToString());
                var loop = args["loop"] != null && args.Value<bool>("loop");
                video.SetProp("isLoop", loop);
                video.SetProp("url", videoUrl);
                video.Play();
                video.OnEnd(() => { Debug.LogWarning("onMediaEnd"); NWCoreBase.hub.Publish(NWCore.instance.nwCoreSettings.wsClient.hubChannel, "onMediaEnd", new { layerIndex = layer, layer = state }); });
                return video;
            case "image":
                var imgUrl = Path.Combine(GetPathFromLocation(location), args["src"].ToString());
                return ComponentManager.New<MediaImage>(new MediaImageProps { url = imgUrl });
            case "audio":
                var audioUrl = Path.Combine(GetPathFromLocation(location), args["src"].ToString());
                var audioLoop = args["loop"] != null && args.Value<bool>("loop");
                var mediaAudio = ComponentManager.New<MediaAudio>(new MediaAudioProps { url = audioUrl, isLoop = audioLoop });
                mediaAudio.OnEnd(() => { Debug.LogWarning("onMediaEnd"); NWCoreBase.hub.Publish(NWCore.instance.nwCoreSettings.wsClient.hubChannel, "onMediaEnd", new { layerIndex = layer, layer = state }); });
                return mediaAudio;

        }
        return mediaNull;
    }

    public void Stop(ComponentBase media)
    {
        if (media is MediaImage || media is MediaAudio)
        {
            ComponentManager.DestroyComponent(media);
        }
        else if (media is ColorsView)
        {
            media.gameObject.SetActive(false);
        }
        else if (media is MediaCapture)
        {
            ComponentManager.DestroyComponent(mediaCapture);
            mediaCapture = null;
        }
        else if (media is MediaVideo)
        {
            var video = media as MediaVideo;
            video.OnEnd(null);
            video.Close();
            video.SetProp("url", null);
        }
        else if (media is MediaSlideshow)
        {
            var ssh = media as MediaSlideshow;
            ssh.Stop();
            ComponentManager.DestroyComponent(ssh);
        }
    }

    public void Transition(Layer layer)
    {
        layer.isTransitioning = true;

        var mediaToHide = layer.mediaA as ITexture;

        if (layer.isCurrentB)
        {
            mediaToHide = layer.mediaB as ITexture;
            layer.mediaA = layer.nextMedia;
            layer.stateA = layer.nextState;

            layer.isCurrentB = false;
        }
        else
        {
            layer.mediaB = layer.nextMedia;
            layer.stateB = layer.nextState;

            layer.isCurrentB = true;
        }

        // Clear nextMedia
        var nextMedia = layer.nextMedia as ITexture;
        layer.nextMedia = null;

        NotifyStateUpdate();

        // Do actual transition

        var transitionSecs = TransitionSeconds;
        if (layer.CurrentState.args != null && (layer.CurrentState.args as JToken).Contains("fadeInSecs"))
            transitionSecs = (layer.CurrentState.args as JToken).Value<float>("fadeInSecs");
    
        if (mediaToHide == null)
        {
            // Dummy hide
            var dummy = 1f;
            DOTween.To(() => { return dummy; },
                (float val) => { dummy  = val; }, 0f, transitionSecs).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    layer.isTransitioning = false;
                });
        }
        else
        {
            DOTween.To(() => { return mediaToHide.Opacity; },
                (float val) => { mediaToHide.Opacity = val; }, 0f, transitionSecs).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    layer.isTransitioning = false;
                    Stop(mediaToHide as ComponentBase);
                });
        }

        DOTween.To(() => { return nextMedia.Opacity; },
            (float val) => { nextMedia.Opacity = val; }, 1f, transitionSecs).SetEase(Ease.OutQuad);


    }


    #region [ Shine API ]
    public void Play(int layer, string view, JToken args, JToken metadata)
    {
        var lay = GetLayer(layer);
        Debug.LogWarning("Play at layer " + layer + " of view " + view + " with nextMedia " + (lay.nextMedia ? lay.nextMedia.name : "null") + " with args " + (args == null ? "null" : args.ToString()));

        lay.nextState = new LayerState() { view = view, args = args, metadata = metadata, state = null };
        var nextMedia = GetMedia(layer, lay.nextState);

        if (lay.nextMedia != nextMedia && lay.nextMedia != null)
            Stop(lay.nextMedia);

        lay.nextMedia = nextMedia;

        // if args contains posRect or uvRect, set them in nextState
        var posRect = args != null && args["posRect"] != null ? args["posRect"].ToObject<float[]>() : new float[] { 0, 0, 1, 1 };
        var uvRect = args != null && args["uvRect"] != null ? args["uvRect"].ToObject<float[]>() : new float[] { 0, 0, 1, 1 };

        if (lay.nextMedia is ColorsView)
            uvRect = new float[] { 0.25f, 0.25f, 0.5f, 0.5f };

        lay.nextState.currentVolume = 1;
        lay.nextState.volume = 1;
        lay.nextState.posRect = posRect;
        lay.nextState.uvRect = uvRect;
        lay.nextState.targetPos = new Vector2(posRect[0] + (posRect[2] / 2), posRect[1] + (posRect[3] / 2));
        lay.nextState.targetSize = new Vector2(posRect[2], posRect[3]);
    }

    public void Stop(int layer)
    {
        Play(layer, "none", null, null);
    }

    public void Pause(int layer)
    {
        var currentVideo = GetLayer(layer).CurrentMedia as IMediaPlaybackControls;
        if (currentVideo != null)
            currentVideo.Pause();

        needsToUpdateState = true;
    }

    public void Resume(int layer)
    {
        var media = GetLayer(layer).CurrentMedia as IMediaPlaybackControls;
        if (media != null)
            media.Play();

        needsToUpdateState = true;
    }

    public void Seek(int layer, float millis)
    {
        var media = GetLayer(layer).CurrentMedia as IMediaPlaybackControls;
        if (media != null)
            media.Seek(millis);

        needsToUpdateState = true;
    }

    public void Forward(int layer, float millisOffset)
    {
        var media = GetLayer(layer).CurrentMedia as IMediaPlaybackControls;
        if (media != null)
            media.Seek(media.GetPositionMillis() + millisOffset);

        needsToUpdateState = true;
    }

    public void Rewind(int layer, float millisOffset)
    {
        var media = GetLayer(layer).CurrentMedia as IMediaPlaybackControls;
        if (media != null)
            media.Seek(media.GetPositionMillis() - millisOffset);

        needsToUpdateState = true;
    }

    public void SetPosRect(int layer, float[] posRect)
    {
        var currentState = GetLayer(layer).CurrentState;
        currentState.posRect = posRect;
        currentState.targetPos = new Vector2(posRect[0] + (posRect[2] / 2), posRect[1] + (posRect[3] / 2));
        currentState.targetSize = new Vector2(posRect[2], posRect[3]);
    }

    public void SetUVRect(int layer, float[] uvRect)
    {
        var currentState = GetLayer(layer).CurrentState;
        currentState.uvRect = uvRect;
    }

    public void PanPos(int layer, float offsx, float offsy)
    {
        var currentState = GetLayer(layer).CurrentState;
        currentState.targetPos += new Vector2(offsx, offsy);
    }

    public void Zoom(int layer, float scale)
    {
        var currentState = GetLayer(layer).CurrentState;
        currentState.targetSize *= scale;
    }

    public void PanZoom(int layer, float offsx, float offsy, float scale)
    {
        var currentState = GetLayer(layer).CurrentState;
        currentState.targetPos += new Vector2(offsx, offsy);
        currentState.targetSize *= scale;
    }

    public void SetVolume(int layer, float volume)
    {
        Debug.Log("Set volume at layer "+layer+" to " + volume);
        var currentState = GetLayer(layer).CurrentState;
        if (currentState != null)
            currentState.volume = volume;
    }

    void RegisterMethods()
    {
        NWCoreBase.hub.Subscribe(NWCore.instance.nwCoreSettings.wsClient.hubChannel)

            .On("play", msg => {
                Debug.Log("play " + msg.Value<string>("view"));
                Play(msg.Value<int>("layer"),
                       msg.Value<string>("view"),
                       msg["args"] == null || msg["args"].Type == JTokenType.Null ? null : msg["args"],
                       msg["metadata"]);
                })
            .On("stop", msg => Stop(msg.Value<int>("layer")))
            .On("pause", msg => Pause(msg.Value<int>("layer")))
            .On("resume", msg => Resume(msg.Value<int>("layer")))
            .On("seek", msg => Seek(msg.Value<int>("layer"), msg.Value<float>("millis")))
            .On("forward", msg => Forward(msg.Value<int>("layer"), msg.Value<float>("millisOffset")))
            .On("rewind", msg => Rewind(msg.Value<int>("layer"), msg.Value<float>("millisOffset")))

            .On("setPosRect", msg => {
                var layer = msg.Value<int>("layer");
                var rect = msg["rect"].ToObject<float[]>();
                SetPosRect(layer, rect);
             })
            .On("setUVRect", msg => SetUVRect(msg.Value<int>("layer"), msg["rect"].ToObject<float[]>()))

            .On("setVolume", msg => SetVolume(msg.Value<int>("layer"), msg.Value<float>("volume")))

            .On("panPos", msg => PanPos(msg.Value<int>("layer"), msg.Value<float>("x"), msg.Value<float>("y")))
            .On("zoom", msg => Zoom(msg.Value<int>("layer"), msg.Value<float>("scale")))
            .On("panZoom", msg => PanZoom(msg.Value<int>("layer"), msg.Value<float>("x"), msg.Value<float>("y"), msg.Value<float>("scale")));
    }


    void NotifyStateUpdate()
    {
        lastStateUpdate = DateTime.Now;
        needsToUpdateState = false;

        if (state.layers == null)
            state.layers = new List<LayerState>();

        state.layers.Clear();

        foreach (var layer in layers)
        {
            if (layer.CurrentState != null)
                layer.CurrentState.state = GetMediaState(layer.CurrentMedia);
            state.layers.Add(layer.CurrentState);
        }

        NWCoreBase.hub.Publish(NWCore.instance.nwCoreSettings.wsClient.hubChannel, "onStateUpdate", state);
    }

    object GetMediaState(ComponentBase media)
    {
        if (media is IGetState)
            return (media as IGetState).GetState();

        return null;
    }

    #endregion


    private void Awake()
    {
        Instance = this;

        layers = new List<Layer>();
        GetLayer(0);

        mediaGallery = FindObjectOfType<GalleryMediaSource>();

        NWCoreBase.rest.Define(NWCore.instance.nwCoreSettings.wsClient.restChannel).On("getState", "getState of shine unity", (msg) => new RestResponse(state));
    }

    public new void Start()
    {
        settings = AppSettingsScript.Instance.Get<Settings>("Shine", "MediaCompositor");

        base.Start();

        RegisterMethods();

        TransitionSeconds = settings.transitionSeconds;

        // Generate all medias
        mediaQR = FindObjectOfType<QRGeneratorComponent>();
        mediaDesktopDup = ComponentManager.New<MediaDesktopDuplication>(new MediaDesktopDuplicationProps { monitorId = settings.desktopDuplicationMonitorId });
        mediaText = ComponentManager.New<MediaCanvas>(new MediaCanvasProps { canvas = GameObject.Find("Text_View").GetComponent<Canvas>() });

        mediaNull = ComponentManager.New<MediaColor>(new MediaColorProps { color = Color.black });

        mediaSlideshow = ComponentManager.New<MediaSlideshow>(new MediaSlideshowProps { width = 1920, height = 1080, slideFadeTime = 1, slideTime = 4});


        // Register to changes
        ColorsView.Instance.onStateChange = () => needsToUpdateState = true;

        // up to 4 videos at the same time
        videos = new MediaVideo[4];
        for (int i = 0; i < videos.Length; ++i)
        {
            videos[i] = ComponentManager.New<MediaVideo>(new MediaVideoProps { url = null });
        }

        NotifyStateUpdate();

        // up to 10 layers at the same time
        inputs = new ComponentBase[10];

        GetLayer(0);
        //Play(0, "none", new JObject(), new JObject());
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        foreach (var lay in layers)
        {
            if (lay != null && lay.nextMedia != null && lay.nextMedia != lay.CurrentMedia)
            {
                if (!lay.isTransitioning)
                    Transition(lay);
            }
        }

        foreach (var lay in layers)
        {
            var state = lay.CurrentState;
            if (Input.GetKeyDown(KeyCode.X))
                state.targetSize *= 1.1f;
            if (Input.GetKeyDown(KeyCode.Z))
                state.targetSize /= 1.1f;

            if (Input.GetKeyDown(KeyCode.W))
                state.targetPos += new Vector2(0, -0.1f);
            if (Input.GetKeyDown(KeyCode.S))
                state.targetPos += new Vector2(0, 0.1f);
            if (Input.GetKeyDown(KeyCode.A))
                state.targetPos += new Vector2(-0.1f, 0);
            if (Input.GetKeyDown(KeyCode.D))
                state.targetPos += new Vector2(0.1f, 0);
        }

        foreach (var lay in layers)
        {
            var state = lay.CurrentState;
            if (state == null || state.posRect == null)
                continue;

            var curPos = Vector2.Lerp(state.targetPos, new Vector2(state.posRect[0] + (state.posRect[2] / 2), state.posRect[1] + (state.posRect[3] / 2)), 0.9f);
            var curSize = Vector2.Lerp(state.targetSize, new Vector2(state.posRect[2], state.posRect[3]), 0.9f);

            var targetClampedPos = state.targetPos;

            if (state.targetPos.x - curSize.x / 2 < 0)
                targetClampedPos.x = curSize.x / 2;

            if (state.targetPos.x + curSize.x / 2 > 1)
                targetClampedPos.x = 1 - curSize.x / 2;

            if (state.targetPos.y - curSize.y/ 2 < 0)
                targetClampedPos.y = curSize.y / 2;

            if (state.targetPos.y + curSize.y / 2 > 1)
                targetClampedPos.y = 1 - curSize.y / 2;


            if (state.targetSize.x > 1)
            {
                state.targetSize = Vector2.Lerp(state.targetSize, Vector2.one, 0.1f);
                targetClampedPos = Vector2.one * 0.5f;
            }

            state.targetPos = Vector2.Lerp(state.targetPos, targetClampedPos, 0.1f);

            state.posRect[0] = curPos.x - curSize.x / 2;
            state.posRect[1] = curPos.y - curSize.y / 2;
            state.posRect[2] = curSize.x;
            state.posRect[3] = curSize.y;

            state.currentVolume = Mathf.Lerp(state.currentVolume, state.volume, 0.1f);            
        }

        // Recalculate inputs
        var inputIdx = 0;
        foreach (var lay in layers)
        {
            if (lay.mediaA != null && lay.mediaA != mediaNull)
            {
                inputs[inputIdx]= lay.mediaA;
                var posRect = lay.stateA.posRect;
                var uvRect = lay.stateA.uvRect;
                SetPosRectOf(inputIdx, new Rect(posRect[0], posRect[1], posRect[2], posRect[3]));
                SetUVRectOf(inputIdx, new Rect(uvRect[0], uvRect[1], uvRect[2], uvRect[3]));
                ++inputIdx;
            }
            if (lay.mediaB != null && lay.mediaB != mediaNull)
            {
                inputs[inputIdx] = lay.mediaB;
                var posRect = lay.stateB.posRect;
                var uvRect = lay.stateB.uvRect;
                SetPosRectOf(inputIdx, new Rect(posRect[0], posRect[1], posRect[2], posRect[3]));
                SetUVRectOf(inputIdx, new Rect(uvRect[0], uvRect[1], uvRect[2], uvRect[3]));
                ++inputIdx;
            }
        }

        while (inputIdx < inputs.Length)
        {
            inputs[inputIdx] = null;
            ++inputIdx;
        }

        // Set volumes to opacities
        foreach (var layer in layers)
        {
            if (layer == null)
                continue;

            var mediaA = layer.mediaA;
            var mediaB = layer.mediaB;

            if (mediaA as ITexture != null && mediaA != mediaNull)
                mediaA.volume = (mediaA as ITexture).Opacity * layer.stateA.currentVolume;

            if (mediaB as ITexture != null && mediaB != mediaNull)
                mediaB.volume = (mediaB as ITexture).Opacity * layer.stateB.currentVolume;

        }

        // Check if we need periodic update (each second)
        if ((DateTime.Now - lastStateUpdate).TotalSeconds > periodicUpdateSeconds)
        {
            foreach (var lay in layers)
            {
                if (lay.CurrentMedia is MediaVideo && (lay.CurrentMedia as MediaVideo).IsPlaying())
                    needsToUpdateState = true;

                if (lay.CurrentMedia is MediaAudio && (lay.CurrentMedia as MediaAudio).IsPlaying())
                    needsToUpdateState = true;

                if (lay.CurrentMedia is ColorsView && ((lay.CurrentMedia as ColorsView).GetState() as UnityColorsState).isPlaying)
                    needsToUpdateState = true;
            }
        }

        // Check if a layer is none and not transition to set it to null
        for (int i = 0; i < layers.Count; ++i)
        {
            if (layers[i] != null && layers[i].CurrentState != null)
            {
                if (layers[i].CurrentState.view == "none" && !layers[i].isTransitioning)
                {
                    layers[i].CurrentState = null;
                    needsToUpdateState = true;
                }
            }
        }

        // Notify state if needed
        if (needsToUpdateState)
            NotifyStateUpdate();

    }
}

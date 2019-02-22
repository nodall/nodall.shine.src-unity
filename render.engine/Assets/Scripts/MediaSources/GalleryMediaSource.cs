using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Shine.Apps;
using nexcode.nwcore;
using Components;
using System;

public class GalleryMediaSource : TextureComponentBase<ComponentProps>, IGetState
{
    int wwwPort = 10500;

    public static GalleryMediaSource Instance;

    public string imagesFolder;
    public List<string> allImages = new List<string>();
    public List<Texture2D> allTextures = new List<Texture2D>();
    List<GameObject> allObjs = new List<GameObject>();
    public List<Vector2> allObjPositions = new List<Vector2>();
    List<GameObject> mirrorObjs = new List<GameObject>();

    List<string> imagesToAdd = new List<string>();

    public List<string> playList = new List<string>();

    public RenderTexture rt;

    public int numVerticalImages = 3;
    public int numHorizontalImages = 1;
    public float horizontalDistance = 10f;
    public float verticalDistance = 10f;

    public Texture2D mirrorAlphaTex;

    public Texture2D uploadPhotosTexture;

    public Vector2 sizeRects = new Vector2(960, 540);

    public Vector2 camMul = new Vector2(104, 64);
    public Vector2 pointTo = new Vector2(0, 0);
    public bool btnGoTo = false;

    public float controlFov = 150;
    public float controlPosX = 3f;
    public float controlPosY = 0f;
    public float controlSpeedX = 0f;

    float minFov = 10;
    float maxFov = 128;
    float targetFov = 30.23f;

    float lerpFactor = 0.9f;

    Canvas canvas;

    Material spriteDefaultMaterial;

    public bool goToNextUploadedImage;

    enum TouchGestureType
    {
        TAP,
        PRESS,
        SCALE_ROTATE,
        PAN,
        PAN_TWO_FINGERS,
        SWIPE_X,
        SWIPE_Y
    };

    enum TouchGestureStatus
    {
        START,
        UPDATE,
        END
    };

    private void Awake()
    {
        Instance = this;
    }

    private string GetPhotoFromRemotePath(string path)
    {
        var res = path;
        if (path.Contains(":" + wwwPort))
        {
            res = path.Split(new string[] { ":" + wwwPort }, System.StringSplitOptions.RemoveEmptyEntries)[1];
        }
        return res;
    }

    void RegisterCallbacks()
    {
        var hub = NWCoreBase.hub;

        hub.Subscribe("_gallery")
            .On("open", (msg) =>
            {
                Debug.Log("Gallery open");
                GalleryOpen();
            })
            .On("pan", (msg) =>
            {
                var panX = msg.Value<float>("panX");
                var panY = msg.Value<float>("panY");

                GalleryPan(panX, panY);
            })
            .On("scale", (msg) =>
            {
                var scale = msg.Value<float>("scale");
                GalleryScale(scale);
            })
            .On("tap", (msg) =>
            {
                GalleryTap();
            })
            .On("play", (msg) =>
            {
                var list = msg["list"] as JArray;
                GalleryPlay(list);
            })
            .On("stop", (msg) =>
            {
                GalleryStopPlaylist();
            })
            .On("goto", (msg) =>
            {
                GalleryGoToPhoto(msg["photo"].ToString());
            })
            .On("delete", (msg) =>
            {
                var list = msg["list"] as JArray;
                GalleryDelete(list);
            })
            .On("list", (msg) =>
            {
                SendAllImages();
            });


        hub.Subscribe("_gallery")
            .On("loadPlay", (msg) =>
            {
                var list = msg["list"] as JArray;
                GalleryLoadPlay(list);
            })
            .On("loadView", (msg) =>
            {
                var list = msg["list"] as JArray;
                GalleryLoadView(list);
            });


        hub.Subscribe("fileupload")
            .On("onFileUploaded", (msg) =>
            {
                GalleryEnqueueImage(msg["path"].ToString());
            });
    }

    // Use this for initialization
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        rt = new RenderTexture(1920, 1080, 0);

        var cam = GetComponentInChildren<Camera>();
        cam.targetTexture = rt;
        allImages = new List<string>();

        //Debug.Log("UploadPhotos tex: " + AppSettingsScript.Instance.MediasFolder + "/app-photos/upload-photos.png");
        uploadPhotosTexture = new Texture2D(4, 4);
        uploadPhotosTexture.LoadImage(File.ReadAllBytes(AppSettingsScript.Instance.WWWFolder + "/unity/img/upload-photos.png"));

        canvas.transform.Find("_upload_photos").GetComponent<RawImage>().texture = uploadPhotosTexture;

        spriteDefaultMaterial = new Material(Shader.Find("Sprites/Default"));


        RegisterCallbacks();

        CreateGallery();
    }

    public void GalleryOpen()
    {
        foreach (var tex in allTextures) Destroy(tex);
        allTextures.Clear();
        allImages.Clear();
        gameObject.SetActive(true);
        DeleteGalleryElements();
        if (LoadGallery() > 0)
        {
        }
    }

    public void GalleryScale(float scale)
    {
        if (scale > 0)
            controlFov -= 4 * 20 * (scale);
        else
            controlFov -= 3 * 50 * (scale);
    }

    public void GalleryPan(float panX, float panY)
    {
        var factor = Mathf.Lerp(0.1f, 1f, Mathf.InverseLerp(5, 100, controlFov));
        //Debug.Log("factor: " + factor);
        //Debug.Log("[Gallery] pan " + msg.Value<float>("panX") + " " + msg.Value<float>("panY"));

        controlSpeedX += factor * panX;
        controlPosY += factor * 3 * panY;
    }

    public void GalleryTap()
    {
        Debug.Log("[Gallery] Focus tap");
        controlFov = 30.23f;
        controlPosX = Mathf.Round(controlPosX);
        controlPosY = Mathf.Round(controlPosY);
        controlSpeedX = 0f;
        lerpFactor = 0.9f;
    }

    public void GalleryLoadPlay(JArray list)
    {
        playList = new List<string>();
        gameObject.SetActive(true);
        //ShineMediaCompositor.Instance.ShowTexture(this);
        foreach (var itemToken in list)
        {
            var item = GetPhotoFromRemotePath(itemToken.ToString());
            playList.Add(item);
        }

        StopAllCoroutines();
        StartCoroutine(CoLoadPlayList(playList));
    }

    public void GalleryGoToPhoto(string photo)
    {
        photo = GetPhotoFromRemotePath(photo);
        Debug.Log("[gallery] goto " + photo);
        GoToPhoto(photo);
    }

    public void GalleryLoadView(JArray list)
    {
        gameObject.SetActive(true);
        //ShineMediaCompositor.Instance.ShowTexture(this);

        //AppsManagerScript.Instance.ShowLayer("Layer_Gallery");
        //AppsManagerScript.Instance.HideLayer("Layer_Chosen");
        //AppsManagerScript.Instance.HideLayer("Layer_Mood");

        playList = new List<string>();

        foreach (var itemToken in list)
        {
            var item = GetPhotoFromRemotePath(itemToken.ToString());
            playList.Add(item);
        }

        StopAllCoroutines();
        StartCoroutine(CoLoadView(playList));
    }

    public void GalleryPlay(JArray list)
    {
        playList = new List<string>();

        if (list.Count == 0)
        {
            playList.AddRange(allImages);
        }

        foreach (var itemToken in list)
        {
            var item = GetPhotoFromRemotePath(itemToken.ToString());
            playList.Add(item);
        }

        StopAllCoroutines();
        StartPlaylist();
    }

    public void GalleryDelete(JArray list)
    {
        var deleteList = new List<string>();

        foreach (var itemToken in list)
        {
            var item = GetPhotoFromRemotePath(itemToken.ToString());
            deleteList.Add(item);
        }
        foreach (var file in deleteList)
            DeleteImage(file);
    }

    public void GalleryEnqueueImage(string path)
    {
        goToNextUploadedImage = true;
        EnqueueImage(path);
    }

    IEnumerator CoLoadGallery()
    {
        yield return null;
        yield return null;
        LoadGallery();
    }

    public void EnqueueImage(string path)
    {
        lock (imagesToAdd)
        {
            imagesToAdd.Add(path);
        }
    }

    public void AddImage(string path)
    {
        var bytes = File.ReadAllBytes(path);
        var tex = new Texture2D(4, 4);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.LoadImage(bytes);
       
        if (!File.Exists(path + ".thumb.jpg"))
        {
            var smallTex = TextureScale.NewBilinear(tex, tex.width / 2, tex.height / 2);
            while (smallTex.height > 256)
            {
                var newSmallTex = TextureScale.NewBilinear(smallTex, smallTex.width / 2, smallTex.height / 2);
                Destroy(smallTex);
                smallTex = newSmallTex;
            }

            File.WriteAllBytes(path + ".thumb.jpg", smallTex.EncodeToJPG());
            Destroy(smallTex);
        }

        allTextures.Add(tex);
        allImages.Add(path);
        CreateGallery();

        if (goToNextUploadedImage)
        {
            var pos = allObjs[allObjs.Count - 1].GetComponent<RectTransform>().anchoredPosition;
            controlPosX = pos.x / (sizeRects.x + horizontalDistance);
            controlPosY = 1 - (pos.y / (sizeRects.y + verticalDistance));
            goToNextUploadedImage = false;
        }

        SendAllImages();
    }

    public void DeleteImage(string path)
    {

        int idx = 0;
        int idxToDelete = -1;
        foreach (var img in allImages)
        {
            if (img.Contains(path))
            {
                Debug.Log(img + " contains " + img);
                idxToDelete = idx;
            }
            ++idx;
        }
        if (idxToDelete >= 0)
        {
            Debug.Log("Going to delete " + allImages[idxToDelete]);
            File.Delete(allImages[idxToDelete]);
            File.Delete(allImages[idxToDelete] + ".thumb.jpg");

            var texToDelete = allTextures[idxToDelete];

            allImages.RemoveAt(idxToDelete);
            allTextures.RemoveAt(idxToDelete);

            Destroy(texToDelete);

            DeleteGalleryElements();
            CreateGallery();
            SendAllImages();
        }
    }


    public void SendAllImages()
    {
        Debug.LogWarning("[GalleryMedia] Going to send all images");
        var data = new JObject();
        var array = new JArray();

        foreach (var img in allImages)
        {
            Debug.LogWarning("[GalleryMedia] Sending images: " + img);
            var imgId = img.Substring(img.LastIndexOf("/www/") + "/www".Length);

            array.Add("http://"+NetworkScript.Instance.GetLocalIp() + ":" + wwwPort + imgId);
        }

        data.Add("list", array);
        NWCoreBase.hub.Publish("_gallery", "list", data);
    }

    public int LoadGallery()
    {
        DeleteGalleryElements();
        if (!Directory.Exists(imagesFolder))
            Directory.CreateDirectory(imagesFolder);

        var files = Directory.GetFiles(imagesFolder);
        int numFiles = 0;
        foreach (var file in files)
        {
            var fl = file.ToLowerInvariant();
            if (fl.EndsWith(".png") || fl.EndsWith(".jpg") || fl.EndsWith(".jpeg"))
            {
                if (!fl.EndsWith(".thumb.jpg"))
                {
                    EnqueueImage(file);
                    ++numFiles;
                }
            }
        }
        //CreateGallery();

        return numFiles;
    }

    public void DeleteGalleryFiles()
    {
        DeleteGalleryElements();
        var files = Directory.GetFiles(imagesFolder);
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }


    void DeleteGalleryElements()
    {
        foreach (Transform child in canvas.transform)
        {
            if (child.name != "_upload_photos")
            {
                var matToDestroy = child.GetComponentInChildren<RawImage>() != null ? child.GetComponentInChildren<RawImage>().material : null;
                Destroy(child.gameObject);
                if (matToDestroy != spriteDefaultMaterial)
                    Destroy(matToDestroy);
            }
        }

        allObjs.Clear();
        allObjPositions.Clear();
        mirrorObjs.Clear();
    }

    void CreateGallery()
    {
        DeleteGalleryElements();

        var numImages = allTextures.Count;

        if (numImages < 3)
            numVerticalImages = 1;
        else if (numImages < 6)
            numVerticalImages = 2;
        else
            numVerticalImages = 3;

        numHorizontalImages = Mathf.CeilToInt(numImages * 1f / numVerticalImages);

        // Show or hide "upload photos" message
        canvas.transform.Find("_upload_photos").gameObject.SetActive(numImages == 0);

        var px = 0;
        var py = 0;

        var idx = 0;

        GameObject lastObj = null;
        while (idx <= numImages)
        {
            if (py == numVerticalImages)
            {
                var objMirror = Instantiate(lastObj);
                var rm = objMirror.GetComponent<RectTransform>();

                rm.transform.SetParent(canvas.transform, false);

                rm.anchoredPosition -= new Vector2(0, sizeRects.y + verticalDistance * 2);
                //rm.localScale = new Vector3(1, -1, 1);

                var rim = objMirror.GetComponentInChildren<RawImage>();
                var rimTex = rim.texture;
                Debug.Log("Mirror Tex: " + rim.texture.width);

                    rim.uvRect = new Rect(0, 1, 1, -1);
                    rim.material = Instantiate(Resources.Load<Material>("AlphaTextureMaterial"));

                        Debug.Log("Mirror Alpha Tex: " + mirrorAlphaTex.width);

                        if (rim.material.HasProperty("_MainTex"))
                            rim.material.SetTexture("_MainTex", rim.texture);
                        rim.material.mainTexture = rimTex;
                        if (rim.material.HasProperty("_AlphaTex"))
                            rim.material.SetTexture("_AlphaTex", mirrorAlphaTex);

                        mirrorObjs.Add(objMirror);
                px++;
                py = 0;
            }

            if (idx == numImages)
                break;

            // Create RawImage
            var tex = allTextures[idx];
            var obj = new GameObject(allImages[idx]);
            var rect = obj.AddComponent<RectTransform>();

            var imgObj = new GameObject("img");
            var img = imgObj.AddComponent<RawImage>();
            img.material = spriteDefaultMaterial;

            obj.transform.SetParent(canvas.transform, false);
            imgObj.transform.SetParent(obj.transform, false);

            rect.sizeDelta = new Vector2(sizeRects.x, sizeRects.y);
            rect.anchoredPosition = new Vector2(px * (sizeRects.x + horizontalDistance), (1-py) * (sizeRects.y + verticalDistance));

            allObjPositions.Add(rect.anchoredPosition);

            var arf = imgObj.AddComponent<AspectRatioFitter>();
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.aspectRatio = tex.width * 1f / tex.height;

            img.texture = tex;


            allObjs.Add(obj);

            lastObj = obj;
            ++py;
            ++idx;
        }

        StartCoroutine(CoReboundCameraTo(95));
    }

    public Vector2 GoToPhoto(string photo)
    {
        int idx = 0;
        var pos = Vector2.zero;
        foreach (var img in allImages)
        {
            var obj = allObjs[idx];
            var rawImg = obj.GetComponentInChildren<RawImage>();

            if (img.Contains(photo))
            {
                Debug.Log("[gotophoto] found photo in idx " + idx);
                StartCoroutine(CoReboundCameraTo(targetFov, 60, 80));
                pos = obj.GetComponent<RectTransform>().anchoredPosition;
                controlPosX = pos.x / (sizeRects.x + horizontalDistance);
                controlPosY = 1 - (pos.y / (sizeRects.y + verticalDistance));
            }
            ++idx;
        }
        return pos;
    }

    public void StartPlaylist()
    {
        controlPosX = Mathf.Abs(controlPosX);
        controlPosY = 0;
        controlFov = targetFov;



        var selectedObjs = new List<GameObject>();

        bool isFirst = false;
        int idx = 0;

        var pos = Vector2.zero;
        foreach (var img in allImages)
        {
            var isSelected = false;
            var indexPlaylist = 0;
            var obj = allObjs[idx];
            var rawImg = obj.GetComponentInChildren<RawImage>();

            int idxPL = 0;
            foreach (var playImg in playList)
            {
                if (img.Contains(playImg))
                {
                    isSelected = true;
                    indexPlaylist = idxPL;

                    if (!isFirst)
                    {
                        pos = GoToPhoto(playImg);
                        isFirst = true;
                    }
                }
                ++idxPL;
            }

            if (!isSelected)
            {
                rawImg.DOFade(0, 1f);
            }
            else
            {
                selectedObjs.Add(obj);

                if (indexPlaylist > 0)
                    rawImg.DOFade(0f, 3);
                else
                    rawImg.DOFade(1f, 1);

                obj.GetComponent<RectTransform>().DOAnchorPos(pos, 1f); // new Vector2(controlPosX*(sizeRects.x+horizontalDistance), 0f), 1f);
            }

            ++idx;
        }

        selectedObjs.Reverse();
        foreach (var obj in selectedObjs)
        {
            obj.transform.SetAsLastSibling();
        }

        if (selectedObjs.Count > 1)
        {
            foreach (var obj in mirrorObjs)
            {
                obj.GetComponentInChildren<RawImage>().material.DOFade(0, 1f);
            }
        }

        selectedObjs.Reverse();
        StartCoroutine(CoPlayList(selectedObjs));
    }

    public void GalleryStopPlaylist()
    {
        StopAllCoroutines();
        for (int i = 0; i < allObjPositions.Count; ++i)
        {
            var obj = allObjs[i];
            var objPos = allObjPositions[i];

            obj.GetComponentInChildren<RawImage>().DOKill();
            obj.GetComponentInChildren<RawImage>().DOFade(1f, 1f);
            obj.GetComponent<RectTransform>().DOAnchorPos(objPos, 1f);
        }

        foreach (var obj in mirrorObjs)
        {
            obj.GetComponentInChildren<RawImage>().material.DOKill();
            obj.GetComponentInChildren<RawImage>().material.DOFade(1f, 3f);
        }

        StartCoroutine(CoReboundCameraTo(95));

    }


    IEnumerator CoPlayList(List<GameObject> selectedObj)
    {
        int curPhoto = 0;

        if (selectedObj.Count == 0)
        {
        }
        else if (selectedObj.Count == 1)
        {

        }
        else
        {
            while (true) // GameObject.Find("Layer_Gallery").GetComponent<NWLayer>().masterOpacity > 0.1f) //curPhoto < selectedObj.Count-1)
            {
                yield return new WaitForSeconds(6f);
                selectedObj[(curPhoto + 1) % selectedObj.Count].GetComponentInChildren<RawImage>().DOFade(1, 0.5f);
                yield return new WaitForSeconds(0.1f);
                selectedObj[curPhoto].GetComponentInChildren<RawImage>().DOFade(0, 1.5f);
                ++curPhoto;

                curPhoto %= selectedObj.Count;
            }
        }
    }


    IEnumerator CoLoadPlayList(List<string> images)
    {
        allImages.Clear();
        foreach (var tex in allTextures) Destroy(tex);
        allTextures.Clear();
        DeleteGalleryElements();

        while (images.Count > 0)
        {
            var img = images[0];
            images.RemoveAt(0);
            EnqueueImage(img);
            Debug.Log("LoadPlaylist enqueue " + img);
            //yield return null;
        }
        runPlaylist = true;
        yield return null;
    }


    IEnumerator CoLoadView(List<string> images)
    {
        allImages.Clear();
        foreach (var tex in allTextures) Destroy(tex);
        allTextures.Clear();
        DeleteGalleryElements();

        while (images.Count > 0)
        {
            var img = images[0];
            images.RemoveAt(0);
            EnqueueImage(img);
            Debug.Log("LoadPlaylist enqueue " + img);
            //yield return null;
        }
        yield return null;
    }

    IEnumerator CoReboundCameraTo(float camFov)
    {
        var camera = GetComponentInChildren<Camera>();

        controlFov = 120;
        while (camera.fieldOfView < 95)
        {
            yield return null;
        }

        controlFov = camFov;
    }


    IEnumerator CoReboundCameraTo(float targetCamFov, float thresCamFov, float susCamFov)
    {
        var camera = GetComponentInChildren<Camera>();

        controlFov = susCamFov;
        while (camera.fieldOfView < thresCamFov)
        {
            yield return null;
        }

        controlFov = targetCamFov;
    }


    Vector2 lastPointTo;
    bool runPlaylist = false;

    // Update is called once per frame
    public override void OnUpdate()
    {
        if (imagesToAdd.Count > 0)
        {
            lock (imagesToAdd)
            {
                if (imagesToAdd.Count > 0)
                {
                    AddImage(imagesToAdd[0].Replace("\\", "/"));
                    imagesToAdd.RemoveAt(0);
                }
            }
        }

        texture = rt;
        var camera = GetComponentInChildren<Camera>();
        var canvas = GetComponentInChildren<Canvas>().GetComponent<RectTransform>();

        if (lastPointTo != pointTo && btnGoTo)
        {
            var ct = camera.transform;

            var px = (int)pointTo.x;
            var py = (int)pointTo.y;

            var childRect = allObjs[px * numVerticalImages + py].GetComponent<RectTransform>();

            ct.localPosition = new Vector3(childRect.anchoredPosition.x * canvas.localScale.x, childRect.anchoredPosition.y * canvas.localScale.y, 0);

            lastPointTo = pointTo;
            //ct.localPosition = new Vector3(camMul.x * pointTo.x, camMul.y * pointTo.y, 0);
        }



        controlPosX += controlSpeedX;
        controlSpeedX *= 0.8f;

        var angleCam = Mathf.Clamp(20f * controlSpeedX, -48, 48);
        camera.transform.localRotation = Quaternion.Lerp(camera.transform.localRotation, Quaternion.Euler(0, angleCam, 0), 0.02f);

        controlFov = Mathf.Clamp(controlFov, 1, 179);

        if (controlFov > maxFov)
            controlFov = Mathf.Lerp(maxFov, controlFov, 0.98f);

        if (controlFov < minFov)
            controlFov = Mathf.Lerp(minFov, controlFov, 0.98f);

       camera.fieldOfView = Mathf.Lerp(controlFov, camera.fieldOfView, 0.98f);

        var camPos = camera.transform.localPosition;

        if (controlPosX < 0)
            controlPosX = Mathf.Lerp(0, controlPosX, 0.6f);

        if (controlPosX > numHorizontalImages - 1)
            controlPosX = Mathf.Lerp(numHorizontalImages - 1, controlPosX, 0.6f);

        if (controlPosY < 0)
            controlPosY = Mathf.Lerp(0, controlPosY, 0.6f);

        if (controlPosY > numVerticalImages - 1)
            controlPosY = Mathf.Lerp(numVerticalImages - 1, controlPosY, 0.6f);

        var tpx = controlPosX * (sizeRects.x + horizontalDistance) * canvas.localScale.x;
        var tpy = (1-controlPosY) * (sizeRects.y + verticalDistance) * canvas.localScale.y;

        lerpFactor = Mathf.Lerp(0.98f, lerpFactor, 0.98f);
        camera.transform.localPosition = Vector3.Lerp(new Vector3(tpx, tpy), camPos, lerpFactor);

        if (runPlaylist && imagesToAdd.Count == 0)
        {
            runPlaylist = false;
            playList = new List<string>();
            playList.AddRange(allImages);
            StartPlaylist();
        }
    }

    public object GetState()
    {
        return new { };
    }
}

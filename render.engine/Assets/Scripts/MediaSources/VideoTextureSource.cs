using UnityEngine;
using System.Collections;
using RenderHeads.Media.AVProVideo;
using nexcode.nwcore;

public class VideoTextureSource : NWMediaSource {

    [SerializeField]
    private string videoFile;

    public Texture texture;
    public bool hasAlreadyFinished;

    private bool isFinishedPlaying;
    //private AVProWindowsMediaMovie avpro;
    public MediaPlayer avpro;

    public Texture2D blackTexture;

    public bool isUnloading = false;
    public bool isScrubbing = false;
    public float scrubRate, scrubPosition;

    public string VideoFile
    {
        get
        {
            return videoFile;
        }
        set
        {
            if (videoFile != value)
            {
                videoFile = value;
                var filename = videoFile;
                var folder = NWManager.MediaPath;
                Debug.Log("Loading file from: " + folder + filename);
                avpro.m_AutoStart = true;
                avpro.m_AutoOpen = true;
                avpro.m_VideoLocation = MediaPlayer.FileLocation.AbsolutePathOrURL;
                avpro.m_VideoPath = folder + filename;
                avpro.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, folder + filename, false);
                //avpro.Play();
                avpro.Control.Play();
            }
            else
            {
                Rewind();
            }
        }
    }

    public float volume
    {
        get
        {
            return avpro.m_Volume;
        }

        set
        {
            avpro.m_Volume = value;
        }
    }

	// Use this for initialization
	void Start () {
        hasAlreadyFinished = true;
        Initialize();
    }

    IEnumerator CoPauseAndRewindIn(float secs)
    {
        yield return new WaitForSeconds(secs);
        Pause();
        Rewind();
    }


    void Initialize()
    {
        Texture = new Texture2D(256, 256);

        avpro = gameObject.GetComponent<MediaPlayer>();

        if (avpro == null)
        avpro = gameObject.AddComponent<MediaPlayer>();

        avpro.PlatformOptionsWindows.videoApi = RenderHeads.Media.AVProVideo.Windows.VideoApi.DirectShow;

        var filename = System.IO.Path.GetFileName(VideoFile);
        var folder = NWManager.MediaPath; // System.IO.Path.GetDirectoryName(VideoFile);

        var path = videoFile.Substring(0, videoFile.Length - filename.Length);
        if (System.IO.Directory.Exists(path))
        {
            folder = path;
        }
        avpro.m_VideoLocation = MediaPlayer.FileLocation.AbsolutePathOrURL;
        avpro.m_VideoPath = folder + filename;
        avpro.m_AutoStart = true;

        if (VideoFile == "" || VideoFile == null)
        {
            avpro.m_AutoOpen = false;
        }
    }

    // Update is called once per frame
    void Update () {

        if (avpro != null)
        {
            texture = avpro.TextureProducer.GetTexture();
            Texture = texture as Texture2D;
            NeedsFlipY = avpro.TextureProducer.RequiresVerticalFlip();
        }

        if (Texture == null)
        {
            //Debug.Log("TEXTURE is NULL");
            Texture = blackTexture;
            texture = blackTexture;
        }

        /*
        if (isScrubbing)
        {
            scrubPosition += scrubRate;
            if (scrubPosition < 0) scrubPosition = 0;
            if (scrubPosition > avpro.MovieInstance.LastFrame)
                scrubPosition = avpro.MovieInstance.LastFrame;

            if (avpro.MovieInstance.PositionFrames != (uint)scrubPosition)
                avpro.MovieInstance.PositionFrames = (uint)scrubPosition;
        }*/

        if (IsFinishedPlaying() && !hasAlreadyFinished)
        {
            var hub = NWCoreBase.hub;
            hub.Publish("_syscmd", "onVideoEnd");
            hub.PublishToMe("_syscmd", "onVideoEnd");
            hasAlreadyFinished = true;
        }
    }

    public bool IsPlaying()
    {
        return avpro.Control.IsPlaying();
    }

    public bool IsFinishedPlaying()
    {
        return avpro.Control.IsFinished();
    }

    public void Play()
    {
        hasAlreadyFinished = false;
        if (avpro != null)
            avpro.Play();
    }

    public void Pause()
    {
        avpro.Pause();
    }

    public void Rewind()
    {
        if (avpro != null)
            avpro.Rewind(false);
    }
}

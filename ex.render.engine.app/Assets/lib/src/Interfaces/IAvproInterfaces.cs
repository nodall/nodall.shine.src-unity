using System;
using UnityEngine;

namespace nexcode.nwcore
{
    public interface IMediaPlayer
	{
		void Update();
		void Render();
	}

	public interface IMediaControl
	{
		// TODO: CanPreRoll() PreRoll()
		// TODO: audio panning

		bool	OpenVideoFromFile(string path, long offset);

        void    CloseVideo();

        void	SetLooping(bool bLooping);
		bool	IsLooping();

		bool	HasMetaData();
		bool	CanPlay();
		bool	IsPlaying();
		bool	IsSeeking();
		bool	IsPaused();
		bool	IsFinished();
		bool	IsBuffering();

		void	Play();
		void	Pause();
		void	Stop();
		void	Rewind();

		void	Seek(float timeMs);
		void	SeekFast(float timeMs);
		float	GetCurrentTimeMs();

		float	GetPlaybackRate();
		void	SetPlaybackRate(float rate);

		void	MuteAudio(bool bMute);
		bool	IsMuted();
		void	SetVolume(float volume);
		float	GetVolume();

		int		GetCurrentAudioTrack();
		void	SetAudioTrack(int index);

		int		GetCurrentVideoTrack();
		void	SetVideoTrack(int index);

		float	GetBufferingProgress();
		int		GetBufferedTimeRangeCount();
		bool	GetBufferedTimeRange(int index, ref float startTimeMs, ref float endTimeMs);

		ErrorCode GetLastError();

		void	GrabAudio(float[] buffer, int floatCount, int channelCount);
	}

	public interface IMediaInfo
	{
		/// <summary>
		/// Returns media duration in milliseconds
		/// </summary>
		float	GetDurationMs();

		/// <summary>
		/// Returns video width in pixels
		/// </summary>
		int		GetVideoWidth();

		/// <summary>
		/// Returns video height in pixels
		/// </summary>
		int		GetVideoHeight();

		/// <summary>
		/// Returns the frame rate of the media.
		/// </summary>
		float GetVideoFrameRate();

		/// <summary>
		/// Returns the current achieved display rate in frames per second
		/// </summary>
		float	GetVideoDisplayRate();

		/// <summary>
		/// Returns true if the media has a visual track
		/// </summary>
		bool	HasVideo();

		/// <summary>
		/// Returns true if the media has a audio track
		/// </summary>
		bool	HasAudio();

		/// <summary>
		/// Returns the number of audio tracks contained in the media
		/// </summary>
		int GetAudioTrackCount();

		/// <summary>
		/// Returns the current audio track identification
		/// </summary>
		string GetCurrentAudioTrackId();

		/// <summary>
		/// Returns the current audio track bitrate
		/// </summary>
		int GetCurrentAudioTrackBitrate();

		/// <summary>
		/// Returns the number of video tracks contained in the media
		/// </summary>
		int GetVideoTrackCount();

		/// <summary>
		/// Returns the current video track identification
		/// </summary>
		string GetCurrentVideoTrackId();

		/// <summary>
		/// Returns the current video track bitrate
		/// </summary>
		int GetCurrentVideoTrackBitrate();

		/// <summary>
		/// Returns the a description of which playback path is used internally.
		/// This can for example expose whether CPU or GPU decoding is being performed
		/// For Windows the available player descriptions are:
		///		"DirectShow" - legacy Microsoft API but still very useful especially with modern filters such as LAV
		///		"MF-MediaEngine-Software" - uses the Windows 8.1 features of the Microsoft Media Foundation API, but software decoding
		///		"MF-MediaEngine-Hardware" - uses the Windows 8.1 features of the Microsoft Media Foundation API, but GPU decoding
		///	Android just has "MediaPlayer"
		///	macOS / tvOS / iOS just has "AVfoundation"
		/// </summary>
		string GetPlayerDescription();

		/// <summary>
		/// Whether this MediaPlayer instance supports linear color space
		/// If it doesn't then a correction may have to be made in the shader
		/// </summary>
		bool PlayerSupportsLinearColorSpace();


		/*
		string GetMediaDescription();
		string GetVideoDescription();
		string GetAudioDescription();*/
		}

	public interface IMediaProducer
	{
		/// <summary>
		/// Returns the Unity texture containing the current frame image.
		/// The texture pointer will return null while the video is loading
		/// This texture usually remains the same for the duration of the video.
		/// There are cases when this texture can change, for instance: if the graphics device is recreated,
		/// a new video is loaded, or if an adaptive stream (eg HLS) is used and it switches video streams.
		/// </summary>
		Texture GetTexture();

		/// <summary>
		/// Returns a count of how many times the texture has been updated
		/// </summary>
		int		GetTextureFrameCount();

		/// <summary>
		/// Returns the presentation time stamp of the current texture
		/// </summary>
		long GetTextureTimeStamp();

		/// <summary>
		/// Returns true if the image on the texture is upside-down
		/// </summary>
		bool	RequiresVerticalFlip();
	}

	// TODO: complete this?
	public interface IMediaConsumer
	{
	}

	public enum Platform
	{
		Windows,
		MacOSX,
		iOS,
		tvOS,
		Android,
		WindowsPhone,
		WindowsUWP,
		WebGL,
		Count = 8,
		Unknown = 100,
	}

	public enum StereoPacking
	{
		None,
		TopBottom,				// Top is the left eye, bottom is the right eye
		LeftRight,              // Left is the left eye, right is the right eye
	}

	public enum AlphaPacking
	{
		None,
		TopBottom,
		LeftRight,
	}

	public enum ErrorCode
	{
		None = 0,
		LoadFailed = 100,
	}

	public static class Windows
	{
		public enum VideoApi
		{
			MediaFoundation,
			DirectShow,
		};

		// WIP: Experimental feature to allow overriding audio device for VR headsets
		public const string AudioDeviceOutputName_Vive = "HTC VIVE USB Audio";
		public const string AudioDeviceOutputName_Rift = "Rift Audio";
	}


    public interface IAVVideoPlayer
    {        
/*        string Url { get; set; }
        float Volume { get; set; }
        float PlaybackRate { get; set; }
        bool IsMuted { get; set; }
        
        void Load();
        void Play();*/
        void Pause();
/*        void Stop();
        void Rewind(bool pause);
        void Unload();

        void Seek(float ms);
        float GetCurrentTimeMs();
*/
/*
		IMediaInfo Info { get; }
		IMediaControl Control { get; }
		IMediaPlayer Player { get; }*/
        //IMediaProducer TextureProducer { get; }
    }
}
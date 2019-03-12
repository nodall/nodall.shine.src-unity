
public interface IMediaPlaybackControls {

    bool IsPlaying();
    float GetPositionMillis();

    void Pause();
    void Play();
    void Seek(float millis);

}

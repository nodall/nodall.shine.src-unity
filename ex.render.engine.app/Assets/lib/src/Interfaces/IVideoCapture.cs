using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nexcode.nwcore
{
    public class CaptureDevice
    {
        public string name;
        public CaptureDeviceMode[] modes;
    }

    public class CaptureDeviceMode
    {
        public string format;
        public int width;
        public int height;
        public float fps;
    }

    public interface IVideoCapture {

        int DesiredDeviceIndex { get; set; }
        int DesiredDeviceMode { get; set; }

        int NumDevices { get; }

        CaptureDevice GetDevice(int idx);

        Texture Texture { get; }

        float DisplayFrameRate { get; }
        int DisplayWidth { get; }
        int DisplayHeight { get; }
    }

}
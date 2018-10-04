using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

public class DownloadFile : CustomYieldInstruction
{
    bool hasFinished = false;

    public override bool keepWaiting
    {
        get
        {
            return !hasFinished;
        }
    }

    public bool isDownloadOK { get; protected set; }

    public DownloadFile(string url, string destPath)
    {
        isDownloadOK = false;
        hasFinished = false;

        new Thread(() =>
        {
            Debug.Log("Going to download file " + url);
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(url, destPath);
                    isDownloadOK = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Exception " + e.Message);
            }
            finally
            {
                hasFinished = true;
            }

        }).Start();
    }
}

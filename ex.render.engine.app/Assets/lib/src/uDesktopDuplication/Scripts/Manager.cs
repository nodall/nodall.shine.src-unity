﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uDesktopDuplication
{

public class Manager : MonoBehaviour
{
    private static Manager instance_;
    public static Manager instance 
    {
        get { return CreateInstance(); }
    }

    public static Manager CreateInstance()
    {
        if (instance_ != null) return instance_;

        var manager = FindObjectOfType<Manager>();
        if (manager) {
            instance_ = manager;
            return manager;
        }

        var go = new GameObject("uDesktopDuplicationManager");
        instance_ = go.AddComponent<Manager>();
        return instance_;
    }

    private List<Monitor> monitors_ = new List<Monitor>();
    static public List<Monitor> monitors
    {
        get { return instance.monitors_; }
    }

    static public int monitorCount
    {
        get { return Lib.GetMonitorCount(); }
    }

    static public int cursorMonitorId 
    {
        get { return Lib.GetCursorMonitorId(); }
    }

    static public Monitor primary
    {
        get 
        {
            return instance.monitors_.Find(monitor => monitor.isPrimary);
        }
    }

    [Tooltip("Debug mode is not applied while running.")]
    [SerializeField] DebugMode debugMode = DebugMode.File;

    [SerializeField] int desktopDuplicationApiTimeout = 0;
    [SerializeField] float retryReinitializationDuration = 1f;

    private Coroutine renderCoroutine_ = null;
    private bool shouldReinitialize_ = false;
    private float reinitializationTimer_ = 0f;
    private bool isFirstFrame_ = true;

    public static event Lib.DebugLogDelegate onDebugLog = msg => Debug.Log(msg);
    public static event Lib.DebugLogDelegate onDebugErr = msg => Debug.LogError(msg);

    public delegate void ReinitializeHandler();
    public static event ReinitializeHandler onReinitialized;

    public static Monitor GetMonitor(int id)
    {
        if (id < 0 || id >= Manager.monitors.Count) {
            Debug.LogErrorFormat("[uDD::Error] there is no monitor whose id is {0}.", id);
            return Manager.primary;
        }
        return monitors[Mathf.Clamp(id, 0, Manager.monitorCount - 1)];
    }

    void Awake()
    {
        // for simple singleton

        if (instance_ == this) {
            return;
        }

        if (instance_ != null && instance_ != this) {
            Destroy(gameObject);
            return;
        }

        instance_ = this;

        Lib.SetDebugMode(debugMode);
        Lib.SetLogFunc(onDebugLog);
        Lib.SetErrorFunc(onDebugErr);

        Lib.SetTimeout(desktopDuplicationApiTimeout);
        Lib.Initialize();

        CreateMonitors();
    }

    void OnApplicationQuit()
    {
        Lib.Finalize();
        DestroyMonitors();
    }

    void OnEnable()
    {
        renderCoroutine_ = StartCoroutine(OnRender());
        if (!isFirstFrame_) {
            Reinitialize();
        }

        Lib.SetDebugMode(debugMode);
        Lib.SetLogFunc(onDebugLog);
    }

    void OnDisable()
    {
        if (renderCoroutine_ != null) {
            StopCoroutine(renderCoroutine_);
            renderCoroutine_ = null;
        }

        Lib.SetLogFunc(null);
        Lib.SetErrorFunc(null);
    }

    void Update()
    {
        Lib.Update();
        ReinitializeIfNeeded();
        UpdateMessage();
        isFirstFrame_ = false;
    }

    [ContextMenu("Reinitialize")]
    public void Reinitialize()
    {
        Debug.Log("[uDD] Reinitialize");
        Lib.Reinitialize();
        CreateMonitors();
        if (onReinitialized != null) {
            onReinitialized();
        }
    }

    void ReinitializeIfNeeded()
    {
        bool reinitializeNeeded = false;

        for (int i = 0; i < monitors.Count; ++i) {
            var monitor = monitors[i];
            var state = monitor.state;
            if (
                state == MonitorState.NotSet ||
                state == MonitorState.AccessLost || 
                state == MonitorState.AccessDenied ||
                state == MonitorState.SessionDisconnected ||
                state == MonitorState.Unknown
            ) {
                reinitializeNeeded = true;
                break;
            }
        }

        if (Lib.HasMonitorCountChanged()) {
            reinitializeNeeded = true;
        }

        if (!shouldReinitialize_ && reinitializeNeeded) {
            shouldReinitialize_ = true;
            reinitializationTimer_ = 0f;
        }

        if (shouldReinitialize_) {
            if (reinitializationTimer_ > retryReinitializationDuration) {
                Reinitialize();
                shouldReinitialize_ = false;
            }
            reinitializationTimer_ += Time.deltaTime;
        }
    }

    void UpdateMessage()
    {
        var message = Lib.PopMessage();
        while (message != Message.None) {
            Debug.Log("[uDD] " + message);
            switch (message) {
                case Message.Reinitialized:
                    ReinitializeMonitors();
                    break;
                case Message.TextureSizeChanged:
                    RecreateTextures();
                    break;
                default:
                    break;
            }
            message = Lib.PopMessage();
        }
    }

    IEnumerator OnRender()
    {
        for (;;) {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < monitors.Count; ++i) {
                var monitor = monitors[i];
                if (monitor.shouldBeUpdated) {
                    monitor.Render();
                }
                monitor.shouldBeUpdated = false;
            }
        }
    }

    void CreateMonitors()
    {
        DestroyMonitors();
        for (int i = 0; i < monitorCount; ++i) {
            monitors.Add(new Monitor(i));
        }
    }

    void DestroyMonitors()
    {
        for (int i = 0; i < monitors.Count; ++i) {
            monitors[i].DestroyTexture();
        }
        monitors.Clear();
    }

    void ReinitializeMonitors()
    {
        for (int i = 0; i < monitorCount; ++i) {
            if (i == monitors.Count) {
                monitors.Add(new Monitor(i));
            } else {
                monitors[i].Reinitialize();
            }
        }
    }

    void RecreateTextures()
    {
        for (int i = 0; i < monitorCount; ++i) {
            monitors[i].CreateTextureIfNeeded();
        }
    }
}

}
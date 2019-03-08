using System;
using UnityEngine;

public class NetworkScript : MonoBehaviour
{
    #region [ Singleton ]
    public static NetworkScript Instance { get; private set; }
    #endregion

    #region [ public fields ]
    public StringThreadSafeProperty localIP = new StringThreadSafeProperty();
    #endregion

    #region [ public methods ]
    public string GetLocalIp()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)        
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return ip.ToString();
                                    
        return "none";
    }
    public void UpdateLocalIp()
    {
        localIP.Value = GetLocalIp();
    }
    #endregion

    #region [ Monobehaviour methods ]
    void Awake()
    {
        Debug.Log("Awake()");
        localIP.Initialize(GetLocalIp());

        #region [ Singleton ]
        if (Instance == null)
            Instance = this;
        else
            throw new Exception("Only One Instance is allowed");
        #endregion
    }
    private void Update()
    {
        localIP.Update();
    }
    #endregion
}


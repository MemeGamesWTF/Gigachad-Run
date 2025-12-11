using UnityEngine;

public class OGPBridge : MonoBehaviour
{
    public static bool OGPReady { get; private set; }
    public static string LastOGPError { get; private set; }

    private void Awake()
    {
        OGPReady = false;
        LastOGPError = null;
    }

    // Called from JS: unityInstance.SendMessage('OGPBridge', 'OnOGPInit', 'ok');
    public void OnOGPInit(string msg)
    {
        Debug.Log("OGP init: " + msg);
        OGPReady = true;
    }

    // Called from JS on errors
    public void OnOGPError(string error)
    {
        Debug.LogError("OGP Error: " + error);
        LastOGPError = error;
        OGPReady = false;
    }

    // Called after savePoints resolves
    public void OnOGPSavePoints(string total)
    {
        Debug.Log("OGP saved points, total: " + total);
    }
}

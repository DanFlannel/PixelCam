using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebcamFeed : MonoBehaviour
{
    public RenderTexture renderTexture;
    public RawImage tex;
    public bool isfrontFacing;
    public Vector2 cameraDimensions = new Vector2(1920, 1080);
    public Vector2 cameraResolution = new Vector2(1920, 1080);

    private RectTransform rect;
    private WebCamTexture webCam;
    private WebCamDevice[] devices;
    private int deviceIndex = 0;
    private bool hasPermissions;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(permissionsAsk());
    }

    public void StartFeed()
    {
        if (devices.Length == 0)
        {
            Debug.Log("No device found");
            return;
        }

        string frontFacing = GetFontFacingCamera();
        WebCamTexture webcamTexture = new WebCamTexture();
#if UNITY_EDITOR
        webcamTexture = new WebCamTexture((int)cameraDimensions.x, (int)cameraDimensions.y);
#else
        
        if (isfrontFacing) {
            webcamTexture = new WebCamTexture(frontFacing, (int)cameraDimensions.x, (int)cameraDimensions.y);
        }else{
            webcamTexture = new WebCamTexture((int)cameraDimensions.x, (int)cameraDimensions.y);
        }
#endif
        tex.texture = webcamTexture;
        //rect.sizeDelta = new Vector2(webcamTexture.requestedWidth, webcamTexture.requestedHeight);
        webcamTexture.Play();


        //rect.SetAnchor(AnchorPresets.StretchAll);

        if (isfrontFacing)
        {
            //rect.sizeDelta = new Vector2(cameraResolution.x, cameraResolution.y);
        }
        else
        {
            //rect.sizeDelta = new Vector2(cameraResolution.y, cameraResolution.x);
        }

        rect.anchoredPosition = Vector2.zero;
        rect.ForceUpdateRectTransforms();
    }

    public void SwitchDevice()
    {
        webCam.Stop();
        deviceIndex = (deviceIndex + 1 <= devices.Length - 1) ? deviceIndex + 1 : 0;
        webCam.deviceName = devices[deviceIndex].name;
        webCam.Play();
    }

    private string GetFontFacingCamera()
    {
        string selectedDeviceName = "";
        devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                selectedDeviceName = devices[i].name;
                break;
            }
        }
        return selectedDeviceName;
    }

    private IEnumerator permissionsAsk()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        hasPermissions = Application.HasUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            devices = WebCamTexture.devices;
            if (tex == null)
            {
                Debug.LogWarning("No rawimage output for webacmtexture " + this.name);
                yield break;
            }
            rect = tex.GetComponent<RectTransform>();
            StartFeed();
        }
        else
        {
            Debug.Log("webcam not found");
        }
    }
}

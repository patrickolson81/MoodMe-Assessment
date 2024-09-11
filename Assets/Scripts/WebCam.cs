
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class WebCam : MonoBehaviour
{
    public RawImage rawImage; // Assign the UI RawImage component where the video feed will be displayed
    private WebCamTexture webCamTexture;

    void Start()
    {
        // Initialize the camera feed
        webCamTexture = new WebCamTexture();

        // Assign the WebCamTexture to the RawImage's texture
        rawImage.texture = webCamTexture;
        rawImage.material.mainTexture = webCamTexture;

        // Start the camera feed
        webCamTexture.Play();
    }

    void OnDisable()
    {
        // Stop the camera feed when the script is disabled or the object is destroyed
        if (webCamTexture != null)
        {
            webCamTexture.Stop();
        }
    }
}

using UnityEngine;
using System.IO;

public class TilemapScreenshot : MonoBehaviour
{
    public Camera captureCamera;
    public int width = 512;
    public int height = 512;
    public string fileName = "TilemapImage.png";

    [ContextMenu("Capture Tilemap")]
    public void Capture()
    {
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        captureCamera.Render();
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/" + fileName, bytes);

        captureCamera.targetTexture = null;
        RenderTexture.active = null;

    }
}

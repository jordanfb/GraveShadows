using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakePicture : MonoBehaviour
{
    [SerializeField]
    private int length;
    [SerializeField]
    private string filename;
    // Start is called before the first frame update
    void Start()
    {
        TakeNewPicture();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeNewPicture()
    {
        //Camera.main.aspect = 1.0f;

        RenderTexture tmp = new RenderTexture(length, length, 24);
        Camera.main.targetTexture = tmp;
        Camera.main.Render();

        RenderTexture.active = tmp;
        Texture2D virtualPhoto = new Texture2D(length, length);
        virtualPhoto.ReadPixels(new Rect(0, 0, length, length), 0, 0);

        RenderTexture.active = null;
        Camera.main.targetTexture = null;

        byte[] bytes;
        bytes = virtualPhoto.EncodeToPNG();

        System.IO.File.WriteAllBytes(ImageLocation(), bytes);
    }

    private string ImageLocation()
    {
        string s = Application.dataPath + "/Evidence/Sprites/" + filename + ".png";
        Debug.Log(s);
        return s;
    }
}

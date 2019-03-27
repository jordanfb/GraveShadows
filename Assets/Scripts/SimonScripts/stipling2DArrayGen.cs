#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class stipling2DArrayGen : MonoBehaviour
{
    // Start is called before the first frame update

    public Texture inputTexture;
    public int numTones;
    Texture2DArray return2DArray;
    public string fileName;

    void Start()
    {
        int iTexW = inputTexture.width;
        int iTexH = inputTexture.height;
        //CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);
        return2DArray = new Texture2DArray(iTexH, iTexH, numTones, TextureFormat.RGBA32, true);
        for (int i=0; i< numTones; i++) {
            Graphics.CopyTexture(inputTexture, 0, 0, i*iTexH, 0, iTexH, iTexH, return2DArray, i, 0, 0, 0);
        }
        return2DArray.Apply();
        UnityEditor.AssetDatabase.CreateAsset(return2DArray, "Assets/SimonPrototype/Textures/" + fileName+".asset");
    }

}


#endif
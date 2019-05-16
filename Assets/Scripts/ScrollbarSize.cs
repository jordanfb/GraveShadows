using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarSize : MonoBehaviour
{
    public Scrollbar scrollbar;
    // Update is called once per frame
    void Update()
    {
        scrollbar.size = 0; // just update it every frame
    }
}
